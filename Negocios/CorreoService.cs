﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Negocios
{
    public class CorreoService
    {
        public async Task EnviarCodigoSeguridad(string correo, string codigo)
        {
            try
            {
                var clienteCorreo = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("x", "x"),  // correo del remitente
                    EnableSsl = true
                };

                // crea el mensaje del correo
                var mensaje = new MailMessage
                {
                    From = new MailAddress("michaelvargas059@gmail.com"),
                    Subject = "Código de seguridad: " + codigo,
                    Body = $"Tu código de seguridad es {codigo}",
                    IsBodyHtml = false
                };

                mensaje.To.Add(correo);

                await clienteCorreo.SendMailAsync(mensaje);

                Console.WriteLine("Correo enviado exitosamente.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al enviar el correo de seguridad.", ex);
            }
        }
    }//fn class
}//fin space
