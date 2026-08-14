using FluentValidation;
using GoldenCrownApi.Dtos.UserDtos;

namespace GoldenCrownApi.Validators.UserValidators
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
