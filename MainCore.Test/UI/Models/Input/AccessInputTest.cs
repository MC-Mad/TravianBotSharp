using MainCore.DTO;
using MainCore.Entities;
using MainCore.UI.Models.Input;

namespace MainCore.Test.UI.Models.Input
{
    public class AccessInputTest
    {
        private static AccessInput CreateFilledInput() => new()
        {
            Id = new AccessId(1),
            Username = "username",
            Password = "password",
            ProxyHost = "127.0.0.1",
            ProxyPort = 8080,
            ProxyUsername = "proxyUsername",
            ProxyPassword = "proxyPassword",
            Useragent = "useragent",
            LastUsed = new DateTime(2024, 1, 1),
        };

        [Fact]
        public void Clear_ResetsAllFields()
        {
            var input = CreateFilledInput();

            input.Clear();

            input.Id.ShouldBe(AccessId.Empty);
            input.Username.ShouldBeEmpty();
            input.Password.ShouldBeEmpty();
            input.ProxyHost.ShouldBeEmpty();
            input.ProxyPort.ShouldBe(0);
            input.ProxyUsername.ShouldBeEmpty();
            input.ProxyPassword.ShouldBeEmpty();
            input.Useragent.ShouldBeEmpty();
            input.LastUsed.ShouldBe(DateTime.MinValue);
        }

        [Fact]
        public void CopyTo_CopiesAllFieldsToTarget()
        {
            var input = CreateFilledInput();
            var target = new AccessInput();

            input.CopyTo(target);

            target.Id.ShouldBe(input.Id);
            target.Username.ShouldBe(input.Username);
            target.Password.ShouldBe(input.Password);
            target.ProxyHost.ShouldBe(input.ProxyHost);
            target.ProxyPort.ShouldBe(input.ProxyPort);
            target.ProxyUsername.ShouldBe(input.ProxyUsername);
            target.ProxyPassword.ShouldBe(input.ProxyPassword);
            target.Useragent.ShouldBe(input.Useragent);
            target.LastUsed.ShouldBe(input.LastUsed);
        }

        [Fact]
        public void Clone_ReturnsNewInstanceWithSameValues()
        {
            var input = CreateFilledInput();

            var clone = input.Clone();

            clone.ShouldNotBeSameAs(input);
            clone.Username.ShouldBe(input.Username);
            clone.Password.ShouldBe(input.Password);
            clone.ProxyPort.ShouldBe(input.ProxyPort);
            clone.LastUsed.ShouldBe(input.LastUsed);

            clone.Username = "other";
            input.Username.ShouldBe("username");
        }

        [Fact]
        public void ToDto_MapsAllFields()
        {
            var input = CreateFilledInput();

            var dto = input.ToDto();

            dto.Id.ShouldBe(input.Id);
            dto.Username.ShouldBe(input.Username);
            dto.Password.ShouldBe(input.Password);
            dto.ProxyHost.ShouldBe(input.ProxyHost);
            dto.ProxyPort.ShouldBe(input.ProxyPort);
            dto.ProxyUsername.ShouldBe(input.ProxyUsername);
            dto.ProxyPassword.ShouldBe(input.ProxyPassword);
            dto.Useragent.ShouldBe(input.Useragent);
            dto.LastUsed.ShouldBe(input.LastUsed);
        }

        [Fact]
        public void ToInput_IsInverseOfToDto()
        {
            var dto = CreateFilledInput().ToDto();

            var input = dto.ToInput();

            input.ToDto().ShouldBeEquivalentTo(dto);
        }
    }
}
