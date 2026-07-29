using MainCore.Enums;
using MainCore.UI.Models.Input;

namespace MainCore.Test.UI.Models.Input
{
    public class ResourceBuildInputTest
    {
        [Fact]
        public void Constructor_SelectsAllResourcesPlanWithLevelTen()
        {
            var input = new ResourceBuildInput();

            input.Plans.Select(x => x.Content).ShouldBe(
            [
                ResourcePlanEnums.AllResources,
                ResourcePlanEnums.OnlyCrop,
                ResourcePlanEnums.ExcludeCrop,
            ]);
            input.Get().ShouldBe((ResourcePlanEnums.AllResources, 10));
        }

        [Fact]
        public void Get_ReturnsSelectedPlanAndLevel()
        {
            var input = new ResourceBuildInput
            {
                Level = 5,
            };
            input.SelectedPlan = input.Plans[1];

            input.Get().ShouldBe((ResourcePlanEnums.OnlyCrop, 5));
        }
    }
}
