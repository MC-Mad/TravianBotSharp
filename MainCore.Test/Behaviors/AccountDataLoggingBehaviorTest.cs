using Immediate.Handlers.Shared;
using MainCore.Behaviors;
using MainCore.Constraints;
using MainCore.Entities;
using MainCore.Services;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;

namespace MainCore.Test.Behaviors
{
    public sealed record AccountRequest(AccountId AccountId) : IAccountConstraint;

    public sealed class LoggingBehavior<TRequest>(ILogger logger) : Behavior<TRequest, object>
    {
        public override async ValueTask<object> HandleAsync(TRequest request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            logger.Information("inner handler");
            return new object();
        }
    }

    public class AccountDataLoggingBehaviorTest : IDisposable
    {
        private readonly TestCorrelatorSinkId _testCorrelatorSinkId = new();
        private readonly Logger _logger;

        public AccountDataLoggingBehaviorTest()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator(_testCorrelatorSinkId)
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        public void Dispose()
        {
            _logger.Dispose();
        }

        private async Task<LogEvent> Handle(IDataService dataService, AccountRequest request)
        {
            var behavior = new AccountDataLoggingBehavior<AccountRequest, object>(dataService);
            behavior.SetInnerHandler(new LoggingBehavior<AccountRequest>(_logger));

            using var context = TestCorrelator.CreateContext();
            await behavior.HandleAsync(request, CancellationToken.None);
            return TestCorrelator.GetLogEventsForSinksFromCurrentContext(_testCorrelatorSinkId)[0];
        }

        private static IDataService CreateDataService(AccountId accountId, bool isLoggerConfigured) => new DataService
        {
            AccountId = accountId,
            AccountData = "account data",
            IsLoggerConfigured = isLoggerConfigured,
        };

        [Fact]
        public async Task RequestOfCurrentAccount_EnrichesLogWithAccountData()
        {
            var dataService = CreateDataService(new AccountId(1), isLoggerConfigured: false);

            var logEvent = await Handle(dataService, new AccountRequest(new AccountId(1)));

            ((ScalarValue)logEvent.Properties["Account"]).Value.ShouldBe("account data");
            ((ScalarValue)logEvent.Properties["AccountId"]).Value.ShouldBe(new AccountId(1).ToString());
        }

        [Fact]
        public async Task RequestOfCurrentAccount_ResetsLoggerConfiguredFlagAfterHandling()
        {
            var dataService = CreateDataService(new AccountId(1), isLoggerConfigured: false);

            await Handle(dataService, new AccountRequest(new AccountId(1)));

            dataService.IsLoggerConfigured.ShouldBeFalse();
        }

        [Fact]
        public async Task RequestOfOtherAccount_DoesNotEnrichLog()
        {
            var dataService = CreateDataService(new AccountId(1), isLoggerConfigured: false);

            var logEvent = await Handle(dataService, new AccountRequest(new AccountId(2)));

            logEvent.Properties.Keys.ShouldNotContain("Account");
        }

        [Fact]
        public async Task LoggerAlreadyConfigured_DoesNotEnrichLogAgain()
        {
            var dataService = CreateDataService(new AccountId(1), isLoggerConfigured: true);

            var logEvent = await Handle(dataService, new AccountRequest(new AccountId(1)));

            logEvent.Properties.Keys.ShouldNotContain("Account");
        }
    }
}
