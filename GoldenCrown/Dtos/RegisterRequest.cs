using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.Dtos
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Логин обязателен для заполнения.")]
        [MinLength(3, ErrorMessage = "Логин должен состоять минимум из 3 символов.")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Имя обязателено для заполнения.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Пароль обязателен для заполнения.")]
        [MinLength(6, ErrorMessage = "Пароль должен состоять минимум из 6 символов.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Пароль должен содержать хотя бы одну заглавную букву, одну строчную букву и одну цифру.")]
        public string Password { get; set; }
    }
}
