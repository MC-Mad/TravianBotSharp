using MainCore.Enums;

namespace MainCore.Test.Enums
{
    public class TroopExtensionTest
    {
        [Theory]
        [InlineData(TroopEnums.Legionnaire, TribeEnums.Romans)]
        [InlineData(TroopEnums.RomanSettler, TribeEnums.Romans)]
        [InlineData(TroopEnums.Clubswinger, TribeEnums.Teutons)]
        [InlineData(TroopEnums.TeutonSettler, TribeEnums.Teutons)]
        [InlineData(TroopEnums.Phalanx, TribeEnums.Gauls)]
        [InlineData(TroopEnums.GaulSettler, TribeEnums.Gauls)]
        [InlineData(TroopEnums.Rat, TribeEnums.Nature)]
        [InlineData(TroopEnums.Elephant, TribeEnums.Nature)]
        [InlineData(TroopEnums.Pikeman, TribeEnums.Natars)]
        [InlineData(TroopEnums.Settler, TribeEnums.Natars)]
        [InlineData(TroopEnums.SlaveMilitia, TribeEnums.Egyptians)]
        [InlineData(TroopEnums.EgyptianSettler, TribeEnums.Egyptians)]
        [InlineData(TroopEnums.Mercenary, TribeEnums.Huns)]
        [InlineData(TroopEnums.HunSettler, TribeEnums.Huns)]
        public void GetTribe_TroopOfTribe_ReturnsTribe(TroopEnums troop, TribeEnums expected)
        {
            troop.GetTribe().ShouldBe(expected);
        }

        [Theory]
        [InlineData(TroopEnums.None)]
        [InlineData(TroopEnums.Hero)]
        public void GetTribe_TriblessTroop_ReturnsAny(TroopEnums troop)
        {
            troop.GetTribe().ShouldBe(TribeEnums.Any);
        }

        [Fact]
        public void GetTribe_EveryTroopIsInsideExactlyOneTribeRange()
        {
            var troops = Enum.GetValues<TroopEnums>()
                .Where(x => x != TroopEnums.None)
                .Where(x => x != TroopEnums.Hero);

            foreach (var troop in troops)
            {
                troop.GetTribe().ShouldNotBe(TribeEnums.Any, $"{troop} doesn't belong to any tribe");
            }
        }
    }
}
