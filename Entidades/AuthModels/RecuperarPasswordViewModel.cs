using System.ComponentModel.DataAnnotations;

namespace Entidades.AuthModels
{
    public class RecuperarPasswordViewModel
    {
        public string Cedula { get; set; }
        public string Correo { get; set; }
    }
}
