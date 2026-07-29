using MainCore.Commands.UI.Misc;
using MainCore.UI.Models.Input;
using MainCore.UI.Models.Output;
using MainCore.UI.ViewModels.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace MainCore.UI.ViewModels.Tabs.Villages
{
    [RegisterSingleton<VillageSettingViewModel>]
    public partial class VillageSettingViewModel : VillageTabViewModelBase
    {
        public VillageSettingInput VillageSettingInput { get; } = new();

        private readonly IDialogService _dialogService;
        private readonly ICustomServiceScopeFactory _serviceScopeFactory;
        private readonly IValidator<VillageSettingInput> _villageSettingInputValidator;

        public VillageSettingViewModel(IDialogService dialogService, IValidator<VillageSettingInput> villageSettingInputValidator, ICustomServiceScopeFactory serviceScopeFactory)
        {
            _dialogService = dialogService;
            _villageSettingInputValidator = villageSettingInputValidator;
            _serviceScopeFactory = serviceScopeFactory;

            LoadSettingCommand.Subscribe(VillageSettingInput.Set);
        }

        public async Task SettingRefresh(VillageId villageId)
        {
            if (!IsActive) return;
            if (villageId != VillageId) return;
            await LoadSettingCommand.Execute(villageId);
        }

        protected override async Task Load(VillageId villageId)
        {
            await LoadSettingCommand.Execute(villageId);
        }

        [ReactiveCommand]
        private async Task Save()
        {
            if (!await _dialogService.Validate(_villageSettingInputValidator, VillageSettingInput)) return;

            await SaveSetting();

            await _dialogService.ShowInformation("Settings saved.");
        }

        [ReactiveCommand]
        private async Task Import()
        {
            var settings = await _dialogService.ImportJson<Dictionary<VillageSettingEnums, int>>();
            if (settings is null) return;

            VillageSettingInput.Set(settings);
            if (!await _dialogService.Validate(_villageSettingInputValidator, VillageSettingInput)) return;

            await SaveSetting();

            await _dialogService.ShowInformation("Settings imported");
        }

        [ReactiveCommand]
        private async Task Export()
        {
            var exported = await _dialogService.ExportJson(GetSetting());
            if (!exported) return;

            await _dialogService.ShowInformation("Settings exported");
        }

        [ReactiveCommand]
        private Dictionary<VillageSettingEnums, int> LoadSetting(VillageId villageId)
        {
            return GetSetting();
        }

        private Dictionary<VillageSettingEnums, int> GetSetting()
        {
            using var scope = _serviceScopeFactory.CreateScope(AccountId);
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var settings = context.VillagesSetting
               .Where(x => x.VillageId == VillageId.Value)
               .ToDictionary(x => x.Setting, x => x.Value);
            return settings;
        }

        private async Task SaveSetting()
        {
            using var scope = _serviceScopeFactory.CreateScope(AccountId);
            var saveVillageSettingCommand = scope.ServiceProvider.GetRequiredService<SaveVillageSettingCommand.Handler>();
            await saveVillageSettingCommand.HandleAsync(new(AccountId, VillageId, VillageSettingInput.Get()));
        }
    }
}