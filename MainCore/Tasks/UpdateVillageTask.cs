using MainCore.Commands.NextExecute;
using MainCore.Tasks.Base;

namespace MainCore.Tasks
{
    [Handler]
    public static partial class UpdateVillageTask
    {
        public sealed class Task : VillageTask
        {
            public Task(AccountId accountId, VillageId villageId) : base(accountId, villageId)
            {
            }

            protected override string TaskName => "Update village";

            public override bool CanStart(AppDbContext context)
            {
                var settingEnable = context.BooleanByName(VillageId, VillageSettingEnums.AutoRefreshEnable);
                if (!settingEnable) return false;

                return true;
            }
        }

        private static async ValueTask<Result> HandleAsync(
            Task task,
            IChromeBrowser browser,
            UpdateBuildingCommand.Handler updateBuildingCommand,
            ToDorfCommand.Handler toDorfCommand,
            NextExecuteUpdateVillageTaskCommand.Handler nextExecuteUpdateVillageTaskCommand,
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

            if (currentDorf != 1)
            {
                var result = await DorfUpdater.ToDorfAndUpdate(1, toDorfCommand, updateBuildingCommand, villageId, cancellationToken);
                if (result.IsFailed) return result;
            }

            await nextExecuteUpdateVillageTaskCommand.HandleAsync(new(task), cancellationToken);
            return Result.Ok();
        }
    }
}