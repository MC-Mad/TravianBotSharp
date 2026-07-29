using MainCore.UI.Models.Input;
using MainCore.UI.Models.Validators;

namespace MainCore.Test.UI.Models.Validators
{
    public class AccountInputValidatorTest
    {
        private readonly AccountInputValidator _validator = new();

        private static AccountInput CreateValidInput()
        {
            var input = new AccountInput
            {
                Username = "username",
                Server = "https://ts1.x1.international.travian.com",
            };
            input.SetAccesses([new AccessInput { Username = "username", Password = "password" }]);
            return input;
        }

        [Fact]
        public void Validate_CompleteInput_IsValid()
        {
            _validator.Validate(CreateValidInput()).IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Validate_EmptyServer_IsInvalid()
        {
            var input = CreateValidInput();
            input.Server = "";

            _validator.Validate(input).IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Validate_RelativeServerUrl_IsInvalid()
        {
            var input = CreateValidInput();
            input.Server = "ts1.x1.international.travian.com";

            var result = _validator.Validate(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(x => x.ErrorMessage.Contains("Invalid Server url"));
        }

        [Fact]
        public void Validate_WithoutAccess_IsInvalid()
        {
            var input = CreateValidInput();
            input.Accesses.Clear();

            _validator.Validate(input).IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Validate_EmptyUsername_IsInvalid()
        {
            var input = CreateValidInput();
            input.Username = "";

            _validator.Validate(input).IsValid.ShouldBeFalse();
        }
    }
}
