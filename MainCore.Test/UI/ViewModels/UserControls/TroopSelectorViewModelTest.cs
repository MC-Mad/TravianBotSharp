using MainCore.Enums;
using MainCore.UI.ViewModels.UserControls;

namespace MainCore.Test.UI.ViewModels.UserControls
{
    public class TroopSelectorViewModelTest
    {
        [Theory]
        [InlineData(BuildingEnums.Barracks, TribeEnums.Romans, TroopEnums.Legionnaire)]
        [InlineData(BuildingEnums.GreatBarracks, TribeEnums.Teutons, TroopEnums.Clubswinger)]
        [InlineData(BuildingEnums.Stable, TribeEnums.Gauls, TroopEnums.Pathfinder)]
        [InlineData(BuildingEnums.GreatStable, TribeEnums.Huns, TroopEnums.Spotter)]
        [InlineData(BuildingEnums.Workshop, TribeEnums.Egyptians, TroopEnums.EgyptianRam)]
        public void Set_TroopOfBuilding_IsSelectable(BuildingEnums building, TribeEnums tribe, TroopEnums troop)
        {
            var viewModel = new TroopSelectorViewModel();

            viewModel.Set(troop, building, tribe);

            viewModel.Items[0].Troop.ShouldBe(TroopEnums.None);
            viewModel.Items.ShouldContain(x => x.Troop == troop);
            viewModel.Get().ShouldBe(troop);
        }

        [Fact]
        public void Set_TroopNotOfBuilding_FallsBackToNone()
        {
            var viewModel = new TroopSelectorViewModel();

            viewModel.Set(TroopEnums.EquitesCaesaris, BuildingEnums.Barracks, TribeEnums.Romans);

            viewModel.Get().ShouldBe(TroopEnums.None);
        }

        [Fact]
        public void Set_BuildingWithoutTroop_OnlyContainsNone()
        {
            var viewModel = new TroopSelectorViewModel();

            viewModel.Set(TroopEnums.Legionnaire, BuildingEnums.MainBuilding, TribeEnums.Romans);

            viewModel.Items.Select(x => x.Troop).ShouldBe([TroopEnums.None]);
            viewModel.Get().ShouldBe(TroopEnums.None);
        }

        [Fact]
        public void ChangeTribe_TroopNotAvailableInNewTribe_ResetsSelection()
        {
            var viewModel = new TroopSelectorViewModel();
            viewModel.Set(TroopEnums.Legionnaire, BuildingEnums.Barracks, TribeEnums.Romans);

            viewModel.ChangeTribe(BuildingEnums.Barracks, TribeEnums.Gauls);

            viewModel.Items.Select(x => x.Troop).ShouldBe([TroopEnums.None, TroopEnums.Phalanx, TroopEnums.Swordsman]);
            viewModel.Get().ShouldBe(TroopEnums.None);
        }

        [Fact]
        public void ChangeTribe_TroopStillAvailable_KeepsSelection()
        {
            var viewModel = new TroopSelectorViewModel();
            viewModel.Set(TroopEnums.None, BuildingEnums.Barracks, TribeEnums.Romans);

            viewModel.ChangeTribe(BuildingEnums.Barracks, TribeEnums.Gauls);

            viewModel.Get().ShouldBe(TroopEnums.None);
        }

        [Fact]
        public void Get_WithoutSetup_ReturnsNone()
        {
            new TroopSelectorViewModel().Get().ShouldBe(TroopEnums.None);
        }
    }
}
