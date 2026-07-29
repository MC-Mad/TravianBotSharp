#pragma warning disable S1172

namespace MainCore.Commands.Features.StartAdventure
{
    [Handler]
    public static partial class ToAdventurePageCommand
    {
        public sealed record Command : ICommand;

        private static async ValueTask<Result> HandleAsync(
            Command command,
            IChromeBrowser browser,
            CancellationToken cancellationToken)
        {
            var adventure = AdventureParser.GetHeroAdventureButton(browser.Html);
            if (adventure is null) return Retry.ButtonNotFound("hero adventure");

            var result = await browser.ClickAndWaitPageChanged(adventure, "adventures", AdventureParser.IsAdventurePage, cancellationToken);
            if (result.IsFailed) return result;

            return Result.Ok();
        }
    }
}