using MainCore.Enums;
using MainCore.Models;

namespace MainCore.Test.Enums
{
    public class BuildingEnumsExtensionTest
    {
        [Theory]
        [InlineData(BuildingEnums.CityWall)]
        [InlineData(BuildingEnums.EarthWall)]
        [InlineData(BuildingEnums.Palisade)]
        [InlineData(BuildingEnums.StoneWall)]
        [InlineData(BuildingEnums.MakeshiftWall)]
        public void IsWall_WallBuilding_ReturnsTrue(BuildingEnums building)
        {
            building.IsWall().ShouldBeTrue();
        }

        [Theory]
        [InlineData(BuildingEnums.Site)]
        [InlineData(BuildingEnums.MainBuilding)]
        [InlineData(BuildingEnums.Cropland)]
        public void IsWall_OtherBuilding_ReturnsFalse(BuildingEnums building)
        {
            building.IsWall().ShouldBeFalse();
        }

        [Theory]
        [InlineData(TribeEnums.Romans, BuildingEnums.CityWall)]
        [InlineData(TribeEnums.Teutons, BuildingEnums.EarthWall)]
        [InlineData(TribeEnums.Gauls, BuildingEnums.Palisade)]
        [InlineData(TribeEnums.Egyptians, BuildingEnums.StoneWall)]
        [InlineData(TribeEnums.Huns, BuildingEnums.MakeshiftWall)]
        [InlineData(TribeEnums.Any, BuildingEnums.Site)]
        [InlineData(TribeEnums.Nature, BuildingEnums.Site)]
        public void GetWall_ReturnsWallOfTribe(TribeEnums tribe, BuildingEnums expected)
        {
            tribe.GetWall().ShouldBe(expected);
        }

        [Theory]
        [InlineData(BuildingEnums.Warehouse)]
        [InlineData(BuildingEnums.Granary)]
        [InlineData(BuildingEnums.GreatWarehouse)]
        [InlineData(BuildingEnums.GreatGranary)]
        [InlineData(BuildingEnums.Trapper)]
        [InlineData(BuildingEnums.Cranny)]
        public void IsMultipleBuilding_MultipleBuilding_ReturnsTrue(BuildingEnums building)
        {
            building.IsMultipleBuilding().ShouldBeTrue();
        }

        [Theory]
        [InlineData(BuildingEnums.MainBuilding)]
        [InlineData(BuildingEnums.Marketplace)]
        public void IsMultipleBuilding_SingleBuilding_ReturnsFalse(BuildingEnums building)
        {
            building.IsMultipleBuilding().ShouldBeFalse();
        }

        [Theory]
        [InlineData(BuildingEnums.Bakery, 5)]
        [InlineData(BuildingEnums.Brickyard, 5)]
        [InlineData(BuildingEnums.IronFoundry, 5)]
        [InlineData(BuildingEnums.GrainMill, 5)]
        [InlineData(BuildingEnums.Sawmill, 5)]
        [InlineData(BuildingEnums.Cranny, 10)]
        [InlineData(BuildingEnums.MainBuilding, 20)]
        [InlineData(BuildingEnums.Woodcutter, 20)]
        public void GetMaxLevel_ReturnsLevelCap(BuildingEnums building, int expected)
        {
            building.GetMaxLevel().ShouldBe(expected);
        }

        [Theory]
        [InlineData(BuildingEnums.Woodcutter)]
        [InlineData(BuildingEnums.ClayPit)]
        [InlineData(BuildingEnums.IronMine)]
        [InlineData(BuildingEnums.Cropland)]
        public void IsResourceField_ResourceField_ReturnsTrue(BuildingEnums building)
        {
            building.IsResourceField().ShouldBeTrue();
        }

        [Theory]
        [InlineData(BuildingEnums.Unknown)]
        [InlineData(BuildingEnums.Site)]
        [InlineData(BuildingEnums.Sawmill)]
        [InlineData(BuildingEnums.MainBuilding)]
        public void IsResourceField_NotResourceField_ReturnsFalse(BuildingEnums building)
        {
            building.IsResourceField().ShouldBeFalse();
        }

        [Fact]
        public void GetPrerequisiteBuildings_BuildingWithoutPrerequisite_ReturnsEmptyList()
        {
            BuildingEnums.Woodcutter.GetPrerequisiteBuildings().ShouldBeEmpty();
        }

        [Fact]
        public void GetPrerequisiteBuildings_Bakery_ReturnsAllPrerequisites()
        {
            var prerequisites = BuildingEnums.Bakery.GetPrerequisiteBuildings();

            prerequisites.ShouldBe(new List<PrerequisiteBuilding>
            {
                new(BuildingEnums.Cropland, 10),
                new(BuildingEnums.GrainMill, 5),
                new(BuildingEnums.MainBuilding, 5),
            });
        }

        [Fact]
        public void GetPrerequisiteBuildings_PrerequisiteLevelNeverExceedsMaxLevel()
        {
            foreach (var building in Enum.GetValues<BuildingEnums>())
            {
                foreach (var prerequisite in building.GetPrerequisiteBuildings())
                {
                    prerequisite.Level.ShouldBeInRange(1, prerequisite.Type.GetMaxLevel());
                }
            }
        }

        [Theory]
        [InlineData(BuildingEnums.Sawmill, 2)]
        [InlineData(BuildingEnums.Academy, 1)]
        [InlineData(BuildingEnums.MainBuilding, 0)]
        public void GetBuildingsCategory_ReturnsCategory(BuildingEnums building, int expected)
        {
            building.GetBuildingsCategory().ShouldBe(expected);
        }

        [Theory]
        [InlineData(BuildingEnums.RallyPoint, true)]
        [InlineData(BuildingEnums.Marketplace, true)]
        [InlineData(BuildingEnums.Residence, true)]
        [InlineData(BuildingEnums.Woodcutter, false)]
        public void HasMultipleTabs_ReturnsExpected(BuildingEnums building, bool expected)
        {
            building.HasMultipleTabs().ShouldBe(expected);
        }

        [Fact]
        public void GetCost_LevelOne_ReturnsBaseCost()
        {
            var cost = BuildingEnums.Woodcutter.GetCost(1);

            cost.Take(4).ShouldBe(new long[] { 40, 100, 50, 60 });
        }

        [Fact]
        public void GetCost_LevelTwo_ScalesByGrowthFactorAndRoundsToFive()
        {
            var cost = BuildingEnums.Woodcutter.GetCost(2);

            cost.Take(4).ShouldBe(new long[] { 65, 165, 85, 100 });
        }

        [Fact]
        public void GetCost_UnknownBuilding_ReturnsZero()
        {
            BuildingEnums.Unknown.GetCost(5).ShouldAllBe(x => x == 0);
        }

        [Fact]
        public void GetCost_WonderOfTheWorld_IsCappedAtOneMillion()
        {
            var cost = BuildingEnums.WW.GetCost(100);

            cost.ShouldAllBe(x => x <= 1_000_000);
            cost.Take(3).ShouldContain(1_000_000);
        }
    }
}
