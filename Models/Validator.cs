using FluentValidation;

namespace ForexFintechAPI.Models;
public class PartnerDtoValidator : AbstractValidator<PartnerDto>, IValidator
{
    public PartnerDtoValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Please specify a name");
        RuleFor(p => p.PartnerCode).NotEmpty().WithMessage("Please enter the code");
        RuleFor(p => p.Email).EmailAddress().WithMessage("Invalid email address");
        RuleFor(p => p.Country).NotEmpty().WithMessage("Please enter the country of the Partner");
    }
}
public class ExchangeManipulationDtoValidator : AbstractValidator<ExchangeManipulationDataDto>, IValidator
{
    public ExchangeManipulationDtoValidator()
    {
        RuleFor(p => p.SourceCurrency).NotEmpty().WithMessage("Please specify source currency");
        RuleFor(p => p.TargetCurrency).NotEmpty().WithMessage("Please specify target currency");
        RuleFor(p => p.Margin).NotEmpty().WithMessage("Please enter the margin to be given to the Partner");
    }
}
public class LoginDtoValidator : AbstractValidator<LoginDto>, IValidator
{
    public LoginDtoValidator()
    {
        RuleFor(p => p.Email).EmailAddress().WithMessage("Please enter valid E-mail Address");
        RuleFor(p => p.Password).NotEmpty().WithMessage("Please enter valid password");
    }
}
public class RegisterDtoValidator : AbstractValidator<RegisterDto>, IValidator
{
    public RegisterDtoValidator()
    {
        RuleFor(p => p.Email).MaximumLength(50).EmailAddress();
        RuleFor(p => p.Password).NotEmpty().WithMessage("Please enter valid password");
        RuleFor(p => p.Password).MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                                .When(x => !string.IsNullOrEmpty(x.Password));
        RuleFor(p => p.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required.")
                                       .When(x =>string.IsNullOrEmpty(x.Password));
        RuleFor(p => p.ConfirmPassword).Equal(x => x.Password)
                                       .When(x => !string.IsNullOrEmpty(x.Password))
                                       .WithMessage("Password and confirm password do not match.");
    }
}



