using MainCore.Commands.UI.Misc;
using MainCore.UI.Models.Input;
using MainCore.UI.Models.Output;
using MainCore.UI.ViewModels.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterSingleton<AccountSettingViewModel>]
    public partial class AccountSettingViewModel : AccountTabViewModelBase
    {
        public AccountSettingInput AccountSettingInput { get; } = new();

        private readonly IDialogService _dialogService;
        private readonly IValidator<AccountSettingInput> _accountsettingInputValidator;
        private readonly ICustomServiceScopeFactory _serviceScopeFactory;

        public AccountSettingViewModel(IDialogService dialogService, IValidator<AccountSettingInput> accountsettingInputValidator, ICustomServiceScopeFactory serviceScopeFactory)
        {
            _dialogService = dialogService;
            _accountsettingInputValidator = accountsettingInputValidator;
            _serviceScopeFactory = serviceScopeFactory;

            LoadSettingsCommand.Subscribe(AccountSettingInput.Set);
        }

        protected override async Task Load(AccountId accountId)
        {
            await LoadSettingsCommand.Execute(accountId);
        }

        [ReactiveCommand]
        private async Task Save()
        {
            if (!await _dialogService.Validate(_accountsettingInputValidator, AccountSettingInput)) return;

            await SaveSetting();

            await _dialogService.ShowInformation("Settings saved.");
        }

        [ReactiveCommand]
        private async Task Import()
        {
            var settings = await _dialogService.ImportJson<Dictionary<AccountSettingEnums, int>>();
            if (settings is null) return;

            AccountSettingInput.Set(settings);
            if (!await _dialogService.Validate(_accountsettingInputValidator, AccountSettingInput)) return;

            await SaveSetting();

            await _dialogService.ShowInformation("Settings imported.");
        }

        [ReactiveCommand]
        private async Task Export()
        {
            var exported = await _dialogService.ExportJson(GetSetting());
            if (!exported) return;

            await _dialogService.ShowInformation("Settings exported.");
        }

        [ReactiveCommand]
        private Dictionary<AccountSettingEnums, int> LoadSettings(AccountId accountId)
        {
            return GetSetting();
        }

        private Dictionary<AccountSettingEnums, int> GetSetting()
        {
            using var scope = _serviceScopeFactory.CreateScope(AccountId);
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var settings = context.AccountsSetting
              .Where(x => x.AccountId == AccountId.Value)
              .ToDictionary(x => x.Setting, x => x.Value);
            return settings;
        }

        private async Task SaveSetting()
        {
            using var scope = _serviceScopeFactory.CreateScope(AccountId);
            var saveAccountSettingCommand = scope.ServiceProvider.GetRequiredService<SaveAccountSettingCommand.Handler>();
            await saveAccountSettingCommand.HandleAsync(new(AccountId, AccountSettingInput.Get()));
        }
    }
}