namespace MainCore.Commands.Navigate
{
    [Handler]
    public static partial class SwitchVillageCommand
    {
        public sealed record Command(VillageId VillageId) : IVillageCommand;

        private static async ValueTask<Result> HandleAsync(
           Command command,
           IChromeBrowser browser,
           CancellationToken cancellationToken
           )
        {
            var villageId = command.VillageId;

            var node = VillagePanelParser.GetVillageNode(browser.Html, villageId);
            if (node is null) return Skip.VillageNotFound;

            if (VillagePanelParser.IsActive(node)) return Result.Ok();

            bool villageChanged(HtmlDocument doc)
            {
                var villageNode = VillagePanelParser.GetVillageNode(doc, villageId);
                return villageNode is not null && VillagePanelParser.IsActive(villageNode);
            }

            var result = await browser.ClickAndWait(node, villageChanged, cancellationToken);
            if (result.IsFailed) return result;

            return Result.Ok();
        }
    }
}