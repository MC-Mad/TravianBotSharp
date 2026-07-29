using MainCore.Enums;
using MainCore.Models;

namespace MainCore.Test.Models
{
    public class ModelTest
    {
        [Theory]
        [InlineData(5, 3, 2, 5)]
        [InlineData(1, 7, 2, 7)]
        [InlineData(1, 2, 9, 9)]
        [InlineData(0, 0, 0, 0)]
        public void BuildingItem_Level_ReturnsHighestOfCurrentQueueAndJobLevel(int current, int queue, int job, int expected)
        {
            var building = new BuildingItem
            {
                CurrentLevel = current,
                QueueLevel = queue,
                JobLevel = job,
            };

            building.Level.ShouldBe(expected);
        }

        [Fact]
        public void NormalBuildPlan_ToString_ContainsHumanizedBuildingName()
        {
            var plan = new NormalBuildPlan
            {
                Location = 26,
                Level = 10,
                Type = BuildingEnums.MainBuilding,
            };

            plan.ToString().ShouldBe("Main building at slot 26 to level 10");
        }

        [Fact]
        public void ResourceBuildPlan_ToString_ContainsHumanizedPlanName()
        {
            var plan = new ResourceBuildPlan
            {
                Level = 5,
                Plan = ResourcePlanEnums.ExcludeCrop,
            };

            plan.ToString().ShouldBe("Exclude crop to level 5");
        }

        [Fact]
        public void PrerequisiteBuilding_ToString_ContainsHumanizedBuildingName()
        {
            var prerequisite = new PrerequisiteBuilding(BuildingEnums.GrainMill, 5);

            prerequisite.ToString().ShouldBe("Grain mill level 5");
        }
    }
}
