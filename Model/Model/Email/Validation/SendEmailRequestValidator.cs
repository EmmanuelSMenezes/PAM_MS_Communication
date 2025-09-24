using FluentValidation;

namespace Domain.Model
{
  public class SendEmailRequestValidator : AbstractValidator<SendEmailRequest>
  {
    public SendEmailRequestValidator()
    {
      RuleFor(v => v.Body)
      .NotEmpty().WithMessage("O Corpo do Email é obrigatório.")
      .NotNull().WithMessage("O Corpo do Email é obrigatório.");

      RuleFor(v => v.Subject)
      .NotEmpty().WithMessage("O Assunto do Email é obrigatório.")
      .NotNull().WithMessage("O Assunto do Email é obrigatório.");

      RuleFor(v => v.Email)
      .NotEmpty().WithMessage("Endereço de Email é obrigatório.")
      .NotNull().WithMessage("Endereço de Email é obrigatório.")
      .EmailAddress().WithMessage("Email inválido.");
    }
  }
}
