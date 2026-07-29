using MainCore.Common.Extensions;

namespace MainCore.Test.Common.Extensions
{
    public class StringExtensionTest
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("username", "username")]
        [InlineData("user name", "user_name")]
        [InlineData("user@name.int", "usernameint")]
        [InlineData("!@#$%^&*()", "")]
        public void Sanitize_RemovesNonAlphanumericAndReplacesSpace(string input, string expected)
        {
            input.Sanitize().ShouldBe(expected);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("ts1.x1.international.travian.com", "")]
        [InlineData("https://ts1.x1.international.travian.com", "https://ts1.x1.international.travian.com")]
        [InlineData("https://ts1.x1.international.travian.com/dorf1.php", "https://ts1.x1.international.travian.com")]
        [InlineData("http://ts1.x1.international.travian.com:8080/dorf1.php?newdid=1", "http://ts1.x1.international.travian.com")]
        public void GetServerUrl_ReturnsSchemeAndHostOnly(string input, string expected)
        {
            input.GetServerUrl().ShouldBe(expected);
        }
    }
}
