using MainCore.Commands.UI.AddAccountViewModel;
using MainCore.UI.Models.Input;
using MainCore.UI.Models.Output;
using MainCore.UI.ViewModels.Abstract;
using MainCore.UI.ViewModels.UserControls;
using Microsoft.Extensions.DependencyInjection;

namespace MainCore.UI.ViewModels.Tabs
{
    [RegisterSingleton<AddAccountViewModel>]
    public partial class AddAccountViewModel : TabViewModelBase
    {
        public AccountInput AccountInput { get; } = new();
        public AccessInput AccessInput { get; } = new();

        private readonly IValidator<AccessInput> _accessInputValidator;
        private readonly IValidator<AccountInput> _accountInputValidator;

        private readonly IDialogService _dialogService;
        private readonly IWaitingOverlayViewModel _waitingOverlayViewModel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AddAccountViewModel(IValidator<AccessInput> accessInputValidator, IDialogService dialogService, IValidator<AccountInput> accountInputValidator, IWaitingOverlayViewModel waitingOverlayViewModel, IServiceScopeFactory serviceScopeFactory)
        {
            _accessInputValidator = accessInputValidator;
            _dialogService = dialogService;
            _accountInputValidator = accountInputValidator;
            _waitingOverlayViewModel = waitingOverlayViewModel;
            _serviceScopeFactory = serviceScopeFactory;

            this.WhenAnyValue(vm => vm.SelectedAccess)
                .WhereNotNull()
                .Subscribe(x => x.CopyTo(AccessInput));

            DeleteAccessCommand.Subscribe(x => SelectedAccess = null);
            AddAccountCommand.Where(x => x).Subscribe(x =>
            {
                AccountInput.Clear();
                AccessInput.Clear();
            });
        }

        [ReactiveCommand]
        private async Task AddAccess()
        {
            if (!await _dialogService.Validate(_accessInputValidator, AccessInput)) return;

            if (string.IsNullOrEmpty(AccountInput.Username))
            {
                AccountInput.Username = AccessInput.Username;
            }

            AccountInput.Accesses.Add(AccessInput.Clone());
        }

        [ReactiveCommand]
        private async Task EditAccess()
        {
            if (SelectedAccess is null) return;

            if (!await _dialogService.Validate(_accessInputValidator, AccessInput)) return;

            AccessInput.CopyTo(SelectedAccess);
        }

        [ReactiveCommand]
        private void DeleteAccess()
        {
            if (SelectedAccess is null) return;
            AccountInput.Accesses.Remove(SelectedAccess);
        }

        [ReactiveCommand]
        private async Task<bool> AddAccount()
        {
            if (!await _dialogService.Validate(_accountInputValidator, AccountInput)) return false;

            await _waitingOverlayViewModel.Show("adding account");

            using var scope = _serviceScopeFactory.CreateScope();
            var addAccountCommand = scope.ServiceProvider.GetRequiredService<AddAccountCommand.Handler>();
            var (_, isFailed, errors) = await addAccountCommand.HandleAsync(new(AccountInput.ToDto()));
            await _waitingOverlayViewModel.Hide();

            if (isFailed)
            {
                await _dialogService.ShowError(string.Join(Environment.NewLine, errors.Select(failure => failure.Message.ToString())));
                return false;
            }

            await _dialogService.ShowInformation("Added account");
            return true;
        }

        [Reactive]
        private AccessInput? _selectedAccess;
    }
}