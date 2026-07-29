namespace MainCore.Commands.UI.MainLayoutViewModel
{
    [Handler]
    public static partial class LoginCommand
    {
        public sealed record Command(AccountId AccountId, AccessDto Access) : IAccountCommand;

        private static async ValueTask<Result> HandleAsync(
            Command command,
            ITaskManager taskManager,
            ITimerManager timerManager,
            ILogger logger,
            IRxQueue rxQueue,
            OpenBrowserCommand.Handler openBrowserCommand,
            CancellationToken cancellationToken
            )
        {
            var (accountId, access) = command;

            logger.Information("Using connection {Proxy} to start chrome", access.Proxy);

            Result result;
            try
            {
                taskManager.SetStatus(accountId, StatusEnums.Starting);
                result = await openBrowserCommand.HandleAsync(new(accountId, access), cancellationToken);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Cannot open browser");
                taskManager.SetStatus(accountId, StatusEnums.Offline);
                return Result.Fail(new Error("Cannot open browser").CausedBy(ex));
            }

            if (result.IsFailed)
            {
                taskManager.SetStatus(accountId, StatusEnums.Offline);
                return result;
            }

            timerManager.Start(accountId);
            taskManager.SetStatus(accountId, StatusEnums.Online);
            rxQueue.Enqueue(new AccountInit(accountId));
            return Result.Ok();
        }
    }
}
