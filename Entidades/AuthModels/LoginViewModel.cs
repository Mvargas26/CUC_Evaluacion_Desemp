using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Entidades.AuthModels
{
    public class LoginViewModel
    {
        public string Cedula { get; set; }

        public string Password { get; set; }

        public string Rol { get; set; }

        public string Correo { get; set; }
    }
 
}
