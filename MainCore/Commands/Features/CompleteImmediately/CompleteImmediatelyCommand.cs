#pragma warning disable S1172

namespace MainCore.Commands.Features.CompleteImmediately
{
    [Handler]
    public static partial class CompleteImmediatelyCommand
    {
        public sealed record Command : ICommand;

        private static async ValueTask<Result> HandleAsync(
            Command command,
            IChromeBrowser browser,
            CancellationToken cancellationToken)
        {
            var oldQueueCount = CompleteImmediatelyParser.CountQueueBuilding(browser.Html);

            if (oldQueueCount == 0) return Result.Ok();

            var completeNowButton = CompleteImmediatelyParser.GetCompleteButton(browser.Html);
            if (completeNowButton is null) return Retry.ButtonNotFound("complete now");

            static bool ConfirmShown(HtmlDocument doc) => CompleteImmediatelyParser.GetConfirmButton(doc) is not null;

            var result = await browser.ClickAndWait(completeNowButton, ConfirmShown, cancellationToken);
            if (result.IsFailed) return result;

            var confirmButton = CompleteImmediatelyParser.GetConfirmButton(browser.Html);
            if (confirmButton is null) return Retry.ButtonNotFound("confirm complete now");

            bool QueueDifferent(HtmlDocument doc) => CompleteImmediatelyParser.CountQueueBuilding(doc) != oldQueueCount;

            result = await browser.ClickAndWait(confirmButton, QueueDifferent, cancellationToken);
            if (result.IsFailed) return result;

            return Result.Ok();
        }
    }
}