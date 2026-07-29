using MainCore.UI.Models.Output;
using MainCore.UI.ViewModels.UserControls;
using Splat;

namespace MainCore.Test.UI.ViewModels.UserControls
{
    public class ListBoxItemViewModelTest
    {
        private static List<ListBoxItem> Items(params int[] ids)
            => ids.Select(id => new ListBoxItem { Id = id, Content = $"item {id}" }).ToList();

        [Fact]
        public void Load_EmptyList_AddsAllItems()
        {
            var viewModel = new ListBoxItemViewModel();

            viewModel.Load(Items(1, 2, 3));

            viewModel.Count.ShouldBe(3);
            viewModel.Items.Select(x => x.Id).ShouldBe([1, 2, 3]);
        }

        [Fact]
        public void Load_EmptyInput_ClearsItems()
        {
            var viewModel = new ListBoxItemViewModel();
            viewModel.Load(Items(1, 2));

            viewModel.Load([]);

            viewModel.Count.ShouldBe(0);
        }

        [Fact]
        public void Load_ExistingItem_IsUpdatedInPlace()
        {
            var viewModel = new ListBoxItemViewModel();
            viewModel.Load(Items(1));
            var item = viewModel[0];

            viewModel.Load([new ListBoxItem { Id = 1, Content = "updated", Color = SplatColor.Red }]);

            viewModel[0].ShouldBeSameAs(item);
            item.Content.ShouldBe("updated");
            item.Color.ShouldBe(SplatColor.Red);
        }

        [Fact]
        public void Load_ReorderedInput_MovesItemsToNewPosition()
        {
            var viewModel = new ListBoxItemViewModel();
            viewModel.Load(Items(1, 2, 3));

            viewModel.Load(Items(3, 1, 2));

            viewModel.Items.Select(x => x.Id).ShouldBe([3, 1, 2]);
        }

        [Fact]
        public void Load_ShorterInput_RemovesExtraItems()
        {
            var viewModel = new ListBoxItemViewModel();
            viewModel.Load(Items(1, 2, 3));

            viewModel.Load(Items(1, 2));

            viewModel.Items.Select(x => x.Id).ShouldBe([1, 2]);
        }

        [Fact]
        public void Load_KeepsSelectionOnSameItem()
        {
            var viewModel = new ListBoxItemViewModel();
            viewModel.Load(Items(1, 2, 3));
            viewModel.SelectedItem = viewModel[2];

            viewModel.Load(Items(3, 1, 2));

            viewModel.SelectedIndex.ShouldBe(0);
        }
    }
}
