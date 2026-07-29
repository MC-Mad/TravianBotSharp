#pragma warning disable S1172

namespace MainCore.Commands.Features.StartAdventure
{
    [Handler]
    public static partial class ExploreAdventureCommand
    {
        public sealed record Command : ICommand;

        private static async ValueTask<Result> HandleAsync(
            Command command,
            IChromeBrowser browser,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            if (!AdventureParser.CanStartAdventure(browser.Html)) return Skip.NoAdventure;

            var adventureButton = AdventureParser.GetAdventureButton(browser.Html);
            if (adventureButton is null) return Retry.ButtonNotFound("adventure");
            logger.Information("Start adventure {Adventure}", AdventureParser.GetAdventureInfo(adventureButton));

            static bool ContinueShow(HtmlDocument doc) => AdventureParser.GetContinueButton(doc) is not null;

            var result = await browser.ClickAndWait(adventureButton, ContinueShow, cancellationToken);
            if (result.IsFailed) return result;

            return Result.Ok();
        }
    }
}