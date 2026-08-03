using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.Dtos.Account
{
    public class TransactionHistoryRequest
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Лимит должен быть больше 0.")]
        public int Limit { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Смещение не может быть отрицательным.")]
        public int Offset { get; set; }
    }
}
