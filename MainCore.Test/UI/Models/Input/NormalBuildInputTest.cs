using MainCore.Enums;
using MainCore.UI.Models.Input;

namespace MainCore.Test.UI.Models.Input
{
    public class NormalBuildInputTest
    {
        [Fact]
        public void Set_SelectsFirstBuildingAndItsMaxLevel()
        {
            var input = new NormalBuildInput();

            input.Set([BuildingEnums.Cranny, BuildingEnums.MainBuilding]);

            input.Buildings.Count.ShouldBe(2);
            input.SelectedBuilding.ShouldNotBeNull();
            input.SelectedBuilding.Content.ShouldBe(BuildingEnums.Cranny);
            input.Level.ShouldBe(BuildingEnums.Cranny.GetMaxLevel());
        }

        [Fact]
        public void Set_WithLevel_KeepsGivenLevel()
        {
            var input = new NormalBuildInput();

            input.Set([BuildingEnums.MainBuilding], 3);

            input.Level.ShouldBe(3);
        }

        [Fact]
        public void Set_EmptyBuildingList_ClearsSelectedBuilding()
        {
            var input = new NormalBuildInput();
            input.Set([BuildingEnums.MainBuilding]);

            input.Set([]);

            input.Buildings.ShouldBeEmpty();
            input.SelectedBuilding.ShouldBeNull();
        }

        [Fact]
        public void SelectedBuilding_Changed_UpdatesLevelToMaxLevel()
        {
            var input = new NormalBuildInput();
            input.Set([BuildingEnums.MainBuilding, BuildingEnums.Sawmill]);

            input.SelectedBuilding = input.Buildings[1];

            input.Level.ShouldBe(BuildingEnums.Sawmill.GetMaxLevel());
        }

        [Fact]
        public void Get_NoSelectedBuilding_ReturnsSiteWithInvalidLevel()
        {
            var input = new NormalBuildInput();

            input.Get().ShouldBe((BuildingEnums.Site, -1));
        }

        [Fact]
        public void Get_SelectedBuilding_ReturnsBuildingAndLevel()
        {
            var input = new NormalBuildInput();
            input.Set([BuildingEnums.MainBuilding], 7);

            input.Get().ShouldBe((BuildingEnums.MainBuilding, 7));
        }

        [Fact]
        public void Clear_ResetsBuildingsSelectionAndLevel()
        {
            var input = new NormalBuildInput();
            input.Set([BuildingEnums.MainBuilding], 7);

            input.Clear();

            input.Buildings.ShouldBeEmpty();
            input.SelectedBuilding.ShouldBeNull();
            input.Level.ShouldBe(0);
        }
    }
}
