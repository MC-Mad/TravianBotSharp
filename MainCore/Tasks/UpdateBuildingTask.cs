using MainCore.Tasks.Base;

namespace MainCore.Tasks
{
    [Handler]
    public static partial class UpdateBuildingTask
    {
        public sealed class Task : VillageTask
        {
            public Task(AccountId accountId, VillageId villageId) : base(accountId, villageId)
            {
            }

            protected override string TaskName => "Update building";
        }

        private static async ValueTask<Result> HandleAsync(
            Task task,
            IChromeBrowser browser,
            UpdateBuildingCommand.Handler updateBuildingCommand,
            ToDorfCommand.Handler toDorfCommand,
            CancellationToken cancellationToken)
        {
            var url = browser.CurrentUrl;
            var villageId = task.VillageId;

            var currentDorf = url.GetCurrentDorf();
            if (currentDorf != 0)
            {
                var result = await DorfUpdater.Update(updateBuildingCommand, villageId, cancellationToken);
                if (result.IsFailed) return result;
            }
            else
            {
                var result = await DorfUpdater.ToDorfAndUpdate(2, toDorfCommand, updateBuildingCommand, villageId, cancellationToken);
                if (result.IsFailed) return result;
            }

            var otherDorf = currentDorf == 1 ? 2 : 1;
            var otherDorfResult = await DorfUpdater.ToDorfAndUpdate(otherDorf, toDorfCommand, updateBuildingCommand, villageId, cancellationToken);
            if (otherDorfResult.IsFailed) return otherDorfResult;

            return Result.Ok();
        }
    }
}