using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.Dtos.Account
{
    public class TransactionHistoryRequest
    {
        [Required(ErrorMessage = "Токен обязателен для заполнения.")]
        public string Token { get; set; } = null!;

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Лимит должен быть больше 0.")]
        public int Limit { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Смещение не может быть отрицательным.")]
        public int Offset { get; set; }
    }
}
