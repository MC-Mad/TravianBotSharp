using MainCore.UI.Models.Output;
using System.Text.Json;

namespace MainCore.Common.Extensions
{
    public static class DialogServiceExtension
    {
        public static async Task ShowError(this IDialogService dialogService, string message)
        {
            await dialogService.MessageBox.Handle(new MessageBoxData("Error", message));
        }

        public static async Task ShowWarning(this IDialogService dialogService, string message)
        {
            await dialogService.MessageBox.Handle(new MessageBoxData("Warning", message));
        }

        public static async Task ShowInformation(this IDialogService dialogService, string message)
        {
            await dialogService.MessageBox.Handle(new MessageBoxData("Information", message));
        }

        public static async Task<bool> AskConfirm(this IDialogService dialogService, string message)
        {
            return await dialogService.ConfirmBox.Handle(new MessageBoxData("Warning", message));
        }

        public static async Task<bool> Validate<T>(this IDialogService dialogService, IValidator<T> validator, T instance)
        {
            var result = await validator.ValidateAsync(instance);
            if (result.IsValid) return true;

            await dialogService.ShowError(result.ToString());
            return false;
        }

        public static async Task<T?> ImportJson<T>(this IDialogService dialogService)
        {
            var path = await dialogService.OpenFileDialog.Handle(Unit.Default);
            if (string.IsNullOrEmpty(path)) return default;

            try
            {
                var jsonString = await File.ReadAllTextAsync(path);
                return JsonSerializer.Deserialize<T>(jsonString)!;
            }
            catch
            {
                await dialogService.ShowWarning("Invalid file.");
                return default;
            }
        }

        public static async Task<bool> ExportJson<T>(this IDialogService dialogService, T content)
        {
            var path = await dialogService.SaveFileDialog.Handle(Unit.Default);
            if (string.IsNullOrEmpty(path)) return false;

            var jsonString = JsonSerializer.Serialize(content);
            await File.WriteAllTextAsync(path, jsonString);
            return true;
        }
    }
}
