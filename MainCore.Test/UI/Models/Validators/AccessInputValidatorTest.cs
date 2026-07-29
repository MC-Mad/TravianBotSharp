using MainCore.UI.Models.Input;
using MainCore.UI.Models.Validators;

namespace MainCore.Test.UI.Models.Validators
{
    public class AccessInputValidatorTest
    {
        private readonly AccessInputValidator _validator = new();

        private static AccessInput CreateValidInput() => new()
        {
            Username = "username",
            Password = "password",
        };

        [Fact]
        public void Validate_MinimalInput_IsValid()
        {
            _validator.Validate(CreateValidInput()).IsValid.ShouldBeTrue();
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData("username", "")]
        public void Validate_MissingCredential_IsInvalid(string username, string password)
        {
            var input = new AccessInput
            {
                Username = username,
                Password = password,
            };

            _validator.Validate(input).IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Validate_ProxyPortWithoutHost_IsInvalid()
        {
            var input = CreateValidInput();
            input.ProxyPort = 8080;

            var result = _validator.Validate(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(x => x.PropertyName == nameof(AccessInput.ProxyHost));
        }

        [Fact]
        public void Validate_ProxyHostWithoutPort_IsInvalid()
        {
            var input = CreateValidInput();
            input.ProxyHost = "127.0.0.1";

            var result = _validator.Validate(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(x => x.PropertyName == nameof(AccessInput.ProxyPort));
        }

        [Fact]
        public void Validate_ProxyPasswordWithoutUsername_IsInvalid()
        {
            var input = CreateValidInput();
            input.ProxyPassword = "proxyPassword";

            var result = _validator.Validate(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(x => x.PropertyName == nameof(AccessInput.ProxyUsername));
        }

        [Fact]
        public void Validate_ProxyUsernameWithoutPassword_IsInvalid()
        {
            var input = CreateValidInput();
            input.ProxyUsername = "proxyUsername";

            var result = _validator.Validate(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(x => x.PropertyName == nameof(AccessInput.ProxyPassword));
        }

        [Fact]
        public void Validate_CompleteProxy_IsValid()
        {
            var input = CreateValidInput();
            input.ProxyHost = "127.0.0.1";
            input.ProxyPort = 8080;
            input.ProxyUsername = "proxyUsername";
            input.ProxyPassword = "proxyPassword";

            _validator.Validate(input).IsValid.ShouldBeTrue();
        }
    }
}
