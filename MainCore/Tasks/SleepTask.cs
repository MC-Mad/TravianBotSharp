using MainCore.Commands.Features;
using MainCore.Commands.NextExecute;
using MainCore.Tasks.Base;

namespace MainCore.Tasks
{
    [Handler]
    public static partial class SleepTask
    {
        public sealed class Task : AccountTask
        {
            public Task(AccountId accountId) : base(accountId)
            {
            }

            protected override string TaskName => "Sleep";
        }

        private static async ValueTask<Result> HandleAsync(
            Task task,
            SleepCommand.Handler sleepCommand,
            GetValidAccessCommand.Handler getAccessQuery,
            OpenBrowserCommand.Handler openBrowserCommand,
            NextExecuteSleepTaskCommand.Handler nextExecuteSleepTaskCommand,
            CancellationToken cancellationToken)
        {
            var result = await sleepCommand.HandleAsync(new(task.AccountId), cancellationToken);
            if (result.IsFailed) return result;

            var (_, isFailed, access, errors) = await getAccessQuery.HandleAsync(new(task.AccountId), cancellationToken);
            if (isFailed) return Result.Fail(errors);

            result = await openBrowserCommand.HandleAsync(new(task.AccountId, access), cancellationToken);
            if (result.IsFailed) return result;

            await nextExecuteSleepTaskCommand.HandleAsync(new(task), cancellationToken);
            return Result.Ok();
        }
    }
}