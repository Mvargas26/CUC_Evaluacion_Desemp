using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
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

        public async Task EnviarCodigoSeguridad(string correo, string codigo)
        {
            try
            {
                var clienteCorreo = new SmtpClient(_smtpServer)
                {
                    Port = _smtpPort,
                    Credentials = new NetworkCredential(_correoUsuario, _correoPass),
                    EnableSsl = true
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(_correoUsuario),
                    Subject = "Código de seguridad: " + codigo,
                    Body = "Tu código de seguridad es " + codigo,
                    IsBodyHtml = false
                };

                mensaje.To.Add(correo);

                await clienteCorreo.SendMailAsync(mensaje);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al enviar el correo de seguridad.", ex);
            }
        }

        public async Task EnviarPasswordTemporal(string correo, string passwordTemporal)
        {
            try
            {
                var clienteCorreo = new SmtpClient(_smtpServer)
                {
                    Port = _smtpPort,
                    Credentials = new NetworkCredential(_correoUsuario, _correoPass),
                    EnableSsl = true
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(_correoUsuario),
                    Subject = "Solicitud Clave temporal",
                    Body = "Usted solicitó una clave temporal, ingrese: " + passwordTemporal,
                    IsBodyHtml = false
                };

                mensaje.To.Add(correo);

                await clienteCorreo.SendMailAsync(mensaje);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al enviar el correo de seguridad.", ex);
            }
        }
    }

}//fin space
