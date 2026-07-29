using MainCore.Enums;
using MainCore.UI.ViewModels.UserControls;

namespace MainCore.Test.UI.ViewModels.UserControls
{
    public class InputViewModelTest
    {
        [Fact]
        public void RangeInput_GetReturnsSetValues()
        {
            var viewModel = new RangeInputViewModel();

            viewModel.Set(10, 20);

            viewModel.Get().ShouldBe((10, 20));
        }

        [Fact]
        public void AmountInput_GetReturnsSetValue()
        {
            var viewModel = new AmountInputViewModel();

            viewModel.Set(42);

            viewModel.Get().ShouldBe(42);
        }

        [Fact]
        public void ResourceInput_GetReturnsSetValues()
        {
            var viewModel = new ResourceInputViewModel();

            viewModel.Set(1, 2, 3, 4);

            viewModel.Get().ShouldBe((1, 2, 3, 4));
        }

        [Fact]
        public void TribeSelector_ExcludesNonPlayableTribes()
        {
            var viewModel = new TribeSelectorViewModel();

            viewModel.Items.Select(x => x.Tribe).ShouldNotContain(TribeEnums.Nature);
            viewModel.Items.Select(x => x.Tribe).ShouldNotContain(TribeEnums.Natars);
            viewModel.Get().ShouldBe(TribeEnums.Any);
        }

        [Fact]
        public void TribeSelector_GetReturnsSetTribe()
        {
            var viewModel = new TribeSelectorViewModel();

            viewModel.Set(TribeEnums.Egyptians);

            viewModel.Get().ShouldBe(TribeEnums.Egyptians);
        }
    }
}
