using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.Dtos.Account
{
    public class TransferRequest
    {
        [Required(ErrorMessage = "Токен обязателен для заполнения.")]
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = "Логин получателя обязателен для заполнения.")]
        public string ReceiverLogin { get; set; } = null!;

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Сумма должна быть больше 0.")]
        public decimal Amount { get; set; }
    }
}
