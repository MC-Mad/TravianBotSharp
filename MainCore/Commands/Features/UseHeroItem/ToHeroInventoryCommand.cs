#pragma warning disable S1172

namespace MainCore.Commands.Features.UseHeroItem
{
    [Handler]
    public static partial class ToHeroInventoryCommand
    {
        public sealed record Command : ICommand;

        private static async ValueTask<Result> HandleAsync(
            Command command,
            IChromeBrowser browser,
            CancellationToken cancellationToken)
        {
            var avatar = InventoryParser.GetHeroAvatar(browser.Html);
            if (avatar is null) return Retry.ButtonNotFound("avatar hero");

            var result = await browser.ClickAndWaitPageChanged(avatar, "hero", InventoryParser.IsInventoryPage, cancellationToken);
            if (result.IsFailed) return result;

            return Result.Ok();
        }
    }
}