using System.ComponentModel.DataAnnotations;

namespace Entidades.AuthModels
{
    public class ReestablecerPasswordViewModel
    {
        public string NuevaContrasena { get; set; }
        public string ConfirmarContrasena { get; set; }
    }
}
