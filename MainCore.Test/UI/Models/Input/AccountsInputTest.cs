using MainCore.UI.Models.Input;

namespace MainCore.Test.UI.Models.Input
{
    public class AccountsInputTest
    {
        [Fact]
        public void ToEntity_MapsAccountWithSingleAccess()
        {
            var input = new AccountsInput
            {
                Username = "test user @.int",
                Server = "https://ts1.x1.international.travian.com",
                Password = "password",
                ProxyHost = "127.0.0.1",
                ProxyPort = 8080,
                ProxyUsername = "proxyUsername",
                ProxyPassword = "proxyPassword",
            };

            var account = input.ToEntity();

            account.Username.ShouldBe("test_user_int");
            account.Server.ShouldBe("https://ts1.x1.international.travian.com");
            var access = account.Accesses.ShouldHaveSingleItem();
            access.Password.ShouldBe("password");
            access.ProxyHost.ShouldBe("127.0.0.1");
            access.ProxyPort.ShouldBe(8080);
            access.ProxyUsername.ShouldBe("proxyUsername");
            access.ProxyPassword.ShouldBe("proxyPassword");
        }
    }
}
