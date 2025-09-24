using FluentValidation;

namespace Domain.Model
{
  public class SendSMSRequestValidator : AbstractValidator<SendSMSRequest>
  {
    public SendSMSRequestValidator()
    {
      RuleFor(v => v.Body)
      .NotEmpty().WithMessage("O Corpo do SMS é obrigatório.")
      .NotNull().WithMessage("O Corpo do SMS é obrigatório.");

      RuleFor(v => v.ToPhoneNumber)
      .NotEmpty().WithMessage("O Número de Destino é obrigatório.")
      .NotNull().WithMessage("O Número de Destino é obrigatório.");
    }
  }
}
