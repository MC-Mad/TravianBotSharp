using Immediate.Handlers.Shared;
using MainCore.Behaviors;
using MainCore.Constraints;
using MainCore.Entities;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;

namespace MainCore.Test.Behaviors
{
    public sealed record LoggedCommand(long[] Resource, string Name) : ICommand;

    public sealed record AccountLoggedCommand(AccountId AccountId, VillageId VillageId) : IAccountVillageCommand
    {
        public void Deconstruct(out AccountId accountId, out VillageId villageId) => (accountId, villageId) = (AccountId, VillageId);
    }

    public sealed record UpdateSomethingCommand : ICommand;

    public sealed class NoopBehavior<TRequest> : Behavior<TRequest, object>
    {
        public override async ValueTask<object> HandleAsync(TRequest request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new object();
        }
    }

    public class CommandLoggingBehaviorTest : IDisposable
    {
        private readonly TestCorrelatorSinkId _testCorrelatorSinkId = new();
        private readonly Logger _logger;

        public CommandLoggingBehaviorTest()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator(_testCorrelatorSinkId)
                .CreateLogger();
        }

        public void Dispose()
        {
            _logger.Dispose();
        }

        private async Task<List<LogEvent>> Handle<TRequest>(TRequest request) where TRequest : ICommand
        {
            var behavior = new CommandLoggingBehavior<TRequest, object>(_logger);
            behavior.SetInnerHandler(new NoopBehavior<TRequest>());

            using var context = TestCorrelator.CreateContext();
            await behavior.HandleAsync(request, CancellationToken.None);
            return TestCorrelator.GetLogEventsForSinksFromCurrentContext(_testCorrelatorSinkId).ToList();
        }

        [Fact]
        public async Task CommandWithoutProperty_LogsNameWithoutMainCorePrefix()
        {
            var logEvents = await Handle(new Command());

            var logEvent = logEvents.ShouldHaveSingleItem();
            var name = (logEvent.Properties["Name"] as ScalarValue)!.Value;
            name.ShouldBe("Constraints.Command");
        }

        [Fact]
        public async Task CommandWithProperties_LogsPropertiesExceptConstraintIds()
        {
            var logEvents = await Handle(new LoggedCommand([1, 2, 3], "name"));

            var logEvent = logEvents.ShouldHaveSingleItem();
            var dict = (logEvent.Properties["Dict"] as DictionaryValue)!;
            var values = dict.Elements.ToDictionary(x => (string)x.Key.Value!, x => ((ScalarValue)x.Value).Value);

            values["Resource"].ShouldBe("1,2,3");
            values["Name"].ShouldBe("name");
        }

        [Fact]
        public async Task CommandWithConstraintIds_DoesNotLogThem()
        {
            var logEvents = await Handle(new AccountLoggedCommand(new AccountId(1), new VillageId(2)));

            var logEvent = logEvents.ShouldHaveSingleItem();
            logEvent.Properties.Keys.ShouldNotContain("Dict");
        }

        [Fact]
        public async Task ExcludedCommand_IsNotLogged()
        {
            var logEvents = await Handle(new UpdateSomethingCommand());

            logEvents.ShouldBeEmpty();
        }
    }
}
