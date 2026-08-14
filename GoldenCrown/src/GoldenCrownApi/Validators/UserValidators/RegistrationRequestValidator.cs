using FluentValidation;
using GoldenCrownApi.Dtos.UserDtos;

namespace GoldenCrownApi.Validators.UserValidators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("Логин обязателен для заполнения.")
                .MinimumLength(3)
                .WithMessage("Логин должен состоять минимум из 3 символов.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Имя обязателено для заполнения.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Пароль обязателен для заполнения.")
                .MinimumLength(6)
                .WithMessage("Пароль должен состоять минимум из 6 символов.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$")
                .WithMessage("Пароль должен содержать хотя бы одну заглавную букву, одну строчную букву и одну цифру.");
        }
    }
}
