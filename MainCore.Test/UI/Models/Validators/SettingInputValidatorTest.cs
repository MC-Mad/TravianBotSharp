using MainCore.Enums;
using MainCore.UI.Models.Input;
using MainCore.UI.Models.Validators;
using MainCore.UI.ViewModels.UserControls;

namespace MainCore.Test.UI.Models.Validators
{
    public class SettingInputValidatorTest
    {
        private readonly AccountSettingInputValidator _accountValidator = new();
        private readonly VillageSettingInputValidator _villageValidator = new();

        private static AccountSettingInput CreateValidAccountSettingInput()
        {
            var input = new AccountSettingInput();
            input.Tribe.Set(TribeEnums.Romans);
            input.ClickDelay.Set(500, 1000);
            input.TaskDelay.Set(500, 1000);
            input.FarmInterval.Set(300, 600);
            return input;
        }

        private static VillageSettingInput CreateValidVillageSettingInput()
        {
            var input = new VillageSettingInput();
            input.Tribe.Set(TribeEnums.Romans);
            input.TrainTroopRepeatTime.Set(30, 60);
            input.BarrackAmount.Set(1, 10);
            input.StableAmount.Set(1, 10);
            input.GreatBarrackAmount.Set(1, 10);
            input.GreatStableAmount.Set(1, 10);
            input.WorkshopAmount.Set(1, 10);
            return input;
        }

        [Fact]
        public void AccountSetting_ValidInput_IsValid()
        {
            _accountValidator.Validate(CreateValidAccountSettingInput()).IsValid.ShouldBeTrue();
        }

        [Fact]
        public void AccountSetting_AnyTribe_IsInvalid()
        {
            var input = CreateValidAccountSettingInput();
            input.Tribe.Set(TribeEnums.Any);

            var result = _accountValidator.Validate(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(x => x.ErrorMessage == "Tribe should be specific");
        }

        public static TheoryData<Func<AccountSettingInput, RangeInputViewModel>> AccountRanges => new()
        {
            x => x.ClickDelay,
            x => x.TaskDelay,
            x => x.FarmInterval,
        };

        [Theory]
        [MemberData(nameof(AccountRanges))]
        public void AccountSetting_MinGreaterThanMax_IsInvalid(Func<AccountSettingInput, RangeInputViewModel> rangeSelector)
        {
            var input = CreateValidAccountSettingInput();
            rangeSelector(input).Set(100, 10);

            _accountValidator.Validate(input).IsValid.ShouldBeFalse();
        }

        [Theory]
        [MemberData(nameof(AccountRanges))]
        public void AccountSetting_NegativeMin_IsInvalid(Func<AccountSettingInput, RangeInputViewModel> rangeSelector)
        {
            var input = CreateValidAccountSettingInput();
            rangeSelector(input).Set(-1, 100);

            _accountValidator.Validate(input).IsValid.ShouldBeFalse();
        }

        [Fact]
        public void VillageSetting_ValidInput_IsValid()
        {
            _villageValidator.Validate(CreateValidVillageSettingInput()).IsValid.ShouldBeTrue();
        }

        [Fact]
        public void VillageSetting_AnyTribe_IsInvalid()
        {
            var input = CreateValidVillageSettingInput();
            input.Tribe.Set(TribeEnums.Any);

            var result = _villageValidator.Validate(input);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(x => x.ErrorMessage == "Tribe should be specific");
        }

        public static TheoryData<Func<VillageSettingInput, RangeInputViewModel>> VillageRanges => new()
        {
            x => x.TrainTroopRepeatTime,
            x => x.BarrackAmount,
            x => x.StableAmount,
            x => x.GreatBarrackAmount,
            x => x.GreatStableAmount,
            x => x.WorkshopAmount,
        };

        [Theory]
        [MemberData(nameof(VillageRanges))]
        public void VillageSetting_MinGreaterThanMax_IsInvalid(Func<VillageSettingInput, RangeInputViewModel> rangeSelector)
        {
            var input = CreateValidVillageSettingInput();
            rangeSelector(input).Set(100, 10);

            _villageValidator.Validate(input).IsValid.ShouldBeFalse();
        }

        [Theory]
        [MemberData(nameof(VillageRanges))]
        public void VillageSetting_NegativeMin_IsInvalid(Func<VillageSettingInput, RangeInputViewModel> rangeSelector)
        {
            var input = CreateValidVillageSettingInput();
            rangeSelector(input).Set(-1, 100);

            _villageValidator.Validate(input).IsValid.ShouldBeFalse();
        }
    }
}
