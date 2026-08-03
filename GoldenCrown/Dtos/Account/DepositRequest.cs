using System.ComponentModel.DataAnnotations;

namespace GoldenCrown.Dtos.Account
{
    public class DepositRequest
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть больше 0.")]
        public decimal Amount { get; set; }
    }
}
