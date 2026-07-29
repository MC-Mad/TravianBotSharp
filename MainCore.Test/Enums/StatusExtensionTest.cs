using MainCore.Enums;
using Splat;

namespace MainCore.Test.Enums
{
    public class StatusExtensionTest
    {
        [Theory]
        [InlineData(StatusEnums.Online)]
        [InlineData(StatusEnums.Starting)]
        [InlineData(StatusEnums.Pausing)]
        [InlineData(StatusEnums.Stopping)]
        [InlineData(StatusEnums.Offline)]
        [InlineData(StatusEnums.Paused)]
        public void GetColor_KnownStatus_ReturnsColor(StatusEnums status)
        {
            var expected = status switch
            {
                StatusEnums.Online => SplatColor.Green,
                StatusEnums.Starting or StatusEnums.Pausing or StatusEnums.Stopping => SplatColor.Orange,
                StatusEnums.Paused => SplatColor.Red,
                _ => SplatColor.Black,
            };

            status.GetColor().ShouldBe(expected);
        }

        [Fact]
        public void GetColor_UndefinedStatus_ReturnsBlack()
        {
            ((StatusEnums)100).GetColor().ShouldBe(SplatColor.Black);
        }
    }
}
