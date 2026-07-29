using MainCore.Behaviors;
using MainCore.Entities;
using MainCore.Tasks.Base;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;

namespace MainCore.Test.Behaviors
{
    public sealed class LoggedTask(AccountId accountId) : AccountTask(accountId)
    {
        protected override string TaskName => "Logged task";
    }

    public class TaskNameLoggingBehaviorTest : IDisposable
    {
        private readonly TestCorrelatorSinkId _testCorrelatorSinkId = new();
        private readonly Logger _logger;

        public TaskNameLoggingBehaviorTest()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator(_testCorrelatorSinkId)
                .CreateLogger();
        }

        public void Dispose()
        {
            _logger.Dispose();
        }

        [Fact]
        public async Task TaskNameLoggingBehaviorShouldLogTaskNameBeforeAndAfterHandling()
        {
            // Arrange
            var behavior = new TaskNameLoggingBehavior<LoggedTask, object>(_logger);
            behavior.SetInnerHandler(new NoopBehavior<LoggedTask>());
            var task = new LoggedTask(new AccountId(1));

            // Act
            using var testCorrelatorContext = TestCorrelator.CreateContext();
            await behavior.HandleAsync(task, CancellationToken.None);

            // Assert
            var logEvents = TestCorrelator.GetLogEventsForSinksFromCurrentContext(_testCorrelatorSinkId).ToList();
            logEvents.Count.ShouldBe(2);
            logEvents.ShouldAllBe(x => ((ScalarValue)x.Properties["TaskName"]).Value!.Equals("Logged task"));
            logEvents[0].RenderMessage().ShouldBe("Task \"Logged task\" is started");
            logEvents[1].RenderMessage().ShouldBe("Task \"Logged task\" is finished");
        }
    }
}
