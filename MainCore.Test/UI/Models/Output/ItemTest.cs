using MainCore.Enums;
using MainCore.UI.Models.Output;

namespace MainCore.Test.UI.Models.Output
{
    public class ItemTest
    {
        [Fact]
        public void TribeItem_ImageSource_UsesTribeImage()
        {
            var item = new TribeItem(TribeEnums.Gauls);

            item.Tribe.ShouldBe(TribeEnums.Gauls);
            item.ImageSource.ShouldEndWith("gaul_big.png");
        }

        [Fact]
        public void TribeItem_EveryTribeHasImage()
        {
            foreach (var tribe in Enum.GetValues<TribeEnums>())
            {
                Should.NotThrow(() => TribeItem.GetImageSource(tribe));
            }
        }

        [Fact]
        public void TroopItem_ImageSource_UsesTribeOfTroop()
        {
            var item = new TroopItem(TroopEnums.Phalanx);

            item.ImageSource.ShouldEndWith("gaul.png");
        }

        [Fact]
        public void TroopItem_NoneTroop_UsesNatarImageAndFixedMask()
        {
            var item = new TroopItem(TroopEnums.None);

            item.ImageSource.ShouldEndWith("natar.png");
            item.ImageMask.ShouldBe(new System.Drawing.Rectangle(57, 0, 16, 16));
        }

        [Fact]
        public void TroopItem_ImageMask_OffsetFollowsTroopPositionInTribe()
        {
            var item = new TroopItem(TroopEnums.Legionnaire);

            item.ImageMask.ShouldBe(new System.Drawing.Rectangle(TroopItem.ImageOffset[(int)TroopEnums.Legionnaire % 10], 0, 16, 16));
        }

        [Fact]
        public void TroopItem_EveryTribeTroopHasImageAndMask()
        {
            var troops = Enum.GetValues<TroopEnums>()
                .Where(x => x != TroopEnums.None)
                .Where(x => x != TroopEnums.Hero);

            foreach (var troop in troops)
            {
                Should.NotThrow(() => TroopItem.GetImageSource(troop));
                Should.NotThrow(() => TroopItem.GetImageMask(troop));
            }
        }

        [Fact]
        public void ComboBoxItem_KeepsContentAndName()
        {
            var item = new ComboBoxItem<BuildingEnums>(BuildingEnums.Cranny, "Cranny");

            item.Content.ShouldBe(BuildingEnums.Cranny);
            item.Name.ShouldBe("Cranny");
        }
    }
}
