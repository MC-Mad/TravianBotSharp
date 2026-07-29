using MainCore.UI.Models.Input;

namespace MainCore.UI.Models.Validators
{
    public class AccountInputValidator : AbstractValidator<AccountInput>
    {
        public AccountInputValidator()
        {
            RuleFor(x => x.Server)
                .NotEmpty()
                .WithName("Server url");

            RuleFor(x => x.Server)
                .Must(IsHttpUrl).When(x => !string.IsNullOrEmpty(x.Server))
                .WithMessage("Invalid Server url, please follow the pattern [https://ts1.x1.international.travian.com]");

            RuleFor(x => x.Accesses)
                .NotEmpty()
                .WithName("Access list");

            RuleFor(x => x.Username)
                .NotEmpty()
                .WithName("Nick name");
        }

        private static bool IsHttpUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}