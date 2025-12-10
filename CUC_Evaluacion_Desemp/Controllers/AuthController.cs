using Entidades.AuthModels;
using Negocios;
using Negocios.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CUC_Evaluacion_Desemp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMantenimientosService _servicioMantenimientos;
        private static readonly ConcurrentDictionary<string, (int intentos, DateTime? bloqueo)> intentosFallidos = 
            new ConcurrentDictionary<string, (int, DateTime?)>();
        private readonly int maxIntentos = 5;
        private readonly TimeSpan duracionBloqueo = TimeSpan.FromMinutes(5);
        private readonly int segundosDeEspera = 300;

        public AuthController(IMantenimientosService servicio)
        {
            _servicioMantenimientos = servicio;

        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string cedula = model.Cedula?.Trim();
            if (string.IsNullOrEmpty(cedula))
            {
                ModelState.AddModelError("", "Cédula requerida.");
                return View(model);
            }

            if (intentosFallidos.TryGetValue(cedula, out var estado) && estado.bloqueo.HasValue)
            {
                var tiempoBloqueo = estado.bloqueo.Value;
                if (DateTime.Now < tiempoBloqueo)
                {
                    TempData["DuracionMensajeEmergente"] = (int)(duracionBloqueo.TotalMilliseconds);
                    TempData["MensajeError"] = $"🔒 Usuario bloqueado. Intente nuevamente en {(tiempoBloqueo - DateTime.Now).Seconds} segundos.";
                    return RedirectToAction("Login");
                }
                else
                {
                    intentosFallidos[cedula] = (0, null);
                }
            }

            //Traemos el usuario
            var usuario = _servicioMantenimientos.Funcionario.ConsultarFuncionarioID(model.Cedula);

            if (usuario == null)
            {
                RegistrarIntentoFallido(cedula);
                ModelState.AddModelError("", "Usuario no encontrado.");
                return View(model);
            }

            if (usuario.Estado  == "Inactivo")
            {
                ModelState.AddModelError("", "Usuario inactivo.");
                return View(model);
            }

            if (!PasswordHelper_Service.VerificarPassword(model.Password, usuario.Password))
            {
                RegistrarIntentoFallido(cedula);
                ModelState.AddModelError("", "Contraseña incorrecta.");
                return View(model);
            }

            intentosFallidos.TryRemove(cedula, out _);

            // genera y guarda el code de seguridad
            string codigoSeguridad = _servicioMantenimientos.Funcionario.GenerarCodigoSeguridad();
            _servicioMantenimientos.Funcionario.EstablecerCodigoSeguridad(model.Cedula, codigoSeguridad);

            // Iniciamos las variables de sesion
            Session["CodigoSeguridad"] = codigoSeguridad;
            Session["CedulaTemp"] = usuario.Cedula;
            Session["UserRoleTemp"] = usuario.Rol;

            //capturo la info del func logueado
            FuncionarioLogueado.capturarDatosFunc(usuario);

            await _servicioMantenimientos.Correo_Service.EnviarCodigoSeguridad(usuario.Correo, codigoSeguridad);

            TempData["Cedula"] = usuario.Cedula;
            return RedirectToAction("VerificarCodigo");
        }//fin

        [HttpGet]
        public ActionResult RecuperarPassword()
        {
            return View();
        }//fin 

        [HttpPost]
        public async Task<ActionResult> RecuperarPassword(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = _servicioMantenimientos.Funcionario.ConsultarFuncionarioID(model.Cedula);


            if (usuario == null || usuario.Correo != model.Correo)
            {
                TempData["AlertaSweet"] = "Los datos ingresados no coinciden con ningún usuario registrado.";
                return RedirectToAction("RecuperarPassword");
            }

            // Generar token:
            String token = GenerarClaveAleatoria();

            //Le cambiamos el password y lo guardamos
            usuario.Password = token;
            _servicioMantenimientos.Funcionario.ModificarPasswordFuncionario(usuario);

            // Enviar correo con la clave nueva
            await _servicioMantenimientos.Correo_Service.EnviarPasswordTemporal(usuario.Correo, token);

            TempData["AlertaSweet"] = "Si los datos coinciden, se ha enviado un correo con las instrucciones.";
            return RedirectToAction("Login");
        }//fin 


        [HttpGet]
        public ActionResult VerificarCodigo()
        {
            var cedula = Session["CedulaTemp"]?.ToString();
            if (cedula == null)
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public ActionResult VerificarCodigo(string cedula, string codigoSeguridad)
        {
            if (string.IsNullOrEmpty(cedula) || string.IsNullOrEmpty(codigoSeguridad))
            {
                TempData["MensajeError"] = "Datos incompletos.";
                return RedirectToAction("VerificarCodigo");
            }

            var codigoGuardado = QuitarComillas(Session["CodigoSeguridad"]?.ToString());
            if (codigoGuardado == null)
            {
                TempData["MensajeError"] = "Código expirado, vuelva a iniciar sesión.";
                return RedirectToAction("Login");
            }

            if (codigoSeguridad != codigoGuardado)
            {
                TempData["MensajeError"] = "Código incorrecto.";
                return RedirectToAction("VerificarCodigo");
            }

            var rol = Session["UserRoleTemp"]?.ToString();
            var ced = Session["CedulaTemp"]?.ToString();

            if ( rol == null || ced == null)
            {
                TempData["MensajeError"] = "Sesión no válida, vuelva a iniciar sesión.";
                return RedirectToAction("Login");
            }

            FormsAuthentication.SetAuthCookie(ced, false);
            Session["UserRole"] = rol;
            Session["Cedula"] = ced;

            Session.Remove("CodigoSeguridad");
            Session.Remove("CedulaTemp");
            Session.Remove("UserRoleTemp");

            return RedirectToAction("Index", "Home");
        }

        public ActionResult CerrarSesion()
        {
            // Cerramos cookie de autenticación
            FormsAuthentication.SignOut();

            // Limpiamos toda la sesión
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            // Evitar cache del navegador (por si se devuelve con back)
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            return RedirectToAction("Login", "Auth");
        }//fin 

        #region Metodos Internos
        private void RegistrarIntentoFallido(string cedula)
        {
            intentosFallidos.AddOrUpdate(
                cedula,
                (1, null),
                (key, old) =>
                {
                    var nuevosIntentos = old.intentos + 1;
                    if (nuevosIntentos >= maxIntentos)
                        return (nuevosIntentos, DateTime.Now.Add(duracionBloqueo));
                    return (nuevosIntentos, old.bloqueo);
                });
        }
        private string QuitarComillas(string c)
        {
            if (c == null) return null;
            return c.Trim().Trim('"').Trim();
        }

        private string GenerarClaveAleatoria()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const int length = 10;

            var bytes = new byte[length];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            var resultado = new char[length];

            for (int i = 0; i < length; i++)
            {
                resultado[i] = chars[bytes[i] % chars.Length];
            }

            return new string(resultado);
        }
        
        #endregion
    }//fin controller
}//fin space