using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.Dtos
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Логин обязателен для заполнения.")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Пароль обязателен для заполнения.")]
        public string Password { get; set; }
    }
}
