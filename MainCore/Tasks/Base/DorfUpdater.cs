namespace MainCore.Tasks.Base
{
    public static class DorfUpdater
    {
        public static async Task<Result> Update(
            UpdateBuildingCommand.Handler updateBuildingCommand,
            VillageId villageId,
            CancellationToken cancellationToken)
        {
            var (_, isFailed, errors) = await updateBuildingCommand.HandleAsync(new(villageId), cancellationToken);
            if (isFailed) return Result.Fail(errors);

            return Result.Ok();
        }

        public static async Task<Result> ToDorfAndUpdate(
            int dorf,
            ToDorfCommand.Handler toDorfCommand,
            UpdateBuildingCommand.Handler updateBuildingCommand,
            VillageId villageId,
            CancellationToken cancellationToken)
        {
            var result = await toDorfCommand.HandleAsync(new(dorf), cancellationToken);
            if (result.IsFailed) return result;

            return await Update(updateBuildingCommand, villageId, cancellationToken);
        }
    }
}
