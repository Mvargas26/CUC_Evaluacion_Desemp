using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Negocios.Services
{
    public class Correo_Service
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _correoUsuario;
        private readonly string _correoPass;

        public Correo_Service()
        {
            _smtpServer = ConfigurationManager.AppSettings["SMTP_Server"];
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SMTP_Port"]);
            _correoUsuario = ConfigurationManager.AppSettings["Correo_Usuario"];
            _correoPass = ConfigurationManager.AppSettings["Correo_Pass"];
        }

        private SmtpClient CrearCliente()
        {
            return new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_correoUsuario, _correoPass),
                EnableSsl = true
            };
        }

        public async Task EnviarCodigoSeguridad(string correo, string codigo)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var clienteCorreo = new SmtpClient(_smtpServer))
                {
                    clienteCorreo.Port = _smtpPort;
                    clienteCorreo.Credentials = new NetworkCredential(_correoUsuario, _correoPass);
                    clienteCorreo.EnableSsl = true;
                    clienteCorreo.Timeout = 20000;

                    using (var mensaje = new MailMessage())
                    {
                        mensaje.From = new MailAddress(_correoUsuario);
                        mensaje.To.Add(correo);
                        mensaje.Subject = "Código de seguridad";
                        mensaje.Body = "Tu código de seguridad es: " + codigo;
                        mensaje.IsBodyHtml = false;

                        await clienteCorreo.SendMailAsync(mensaje);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al enviar el código de seguridad por correo.", ex);
            }
        }

        public async Task EnviarPasswordTemporal(string correo, string passwordTemporal)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var clienteCorreo = CrearCliente())
                {
                    clienteCorreo.Timeout = 20000;

                    using (var mensaje = new MailMessage())
                    {
                        mensaje.From = new MailAddress(_correoUsuario);
                        mensaje.To.Add(correo);
                        mensaje.Subject = "Clave temporal";
                        mensaje.Body = "Usted solicitó una clave temporal: " + passwordTemporal;
                        mensaje.IsBodyHtml = false;

                        await clienteCorreo.SendMailAsync(mensaje);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al enviar la clave temporal por correo.", ex);
            }
        }

    }
}
