using FluentValidation;
using GoldenCrown.Dtos.UserDtos;

namespace GoldenCrown.Validators.UserValidators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("Логин обязателен для заполнения.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Пароль обязателен для заполнения.");
        }
    }
}
