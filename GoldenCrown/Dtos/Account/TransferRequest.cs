using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.Dtos.Account
{
    public class TransferRequest
    {
        [Required(ErrorMessage = "Токен обязателен для заполнения.")]
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = "Логин получателя обязателен для заполнения.")]
        public string ReceiverLogin { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть больше 0.")]
        public decimal Amount { get; set; }
    }
}
