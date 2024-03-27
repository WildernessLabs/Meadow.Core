using Meadow;
using Meadow.Cloud;
using Meadow.Logging;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Core.Unit.Tests.UpdateServiceTests
{
    public class ProcessPublishedCommandTests : IDisposable
    {
        private readonly TestLogProvider _log = new();
        private readonly MeadowCloudSettings _settings = new();
        private readonly MeadowCloudConnectionService _connectionService;
        private readonly MeadowCloudCommandService _updateService;

        public ProcessPublishedCommandTests()
        {
            _connectionService = new(_settings);
            _updateService = new(_connectionService);

            Resolver.Services.GetOrCreate<Logger>();
            Resolver.Log.LogLevel = LogLevel.Trace;
            Resolver.Log.AddProvider(_log);
            if (Resolver.Services.Get<IJsonSerializer>() == null)
            {
                Resolver.Services.Add<IJsonSerializer>(new MicroJsonSerializer());
            }
        }

        public void Dispose()
        {
            _log.Disable();
        }

        [Fact]
        public void ProcessPublishedCommand_WithNullUserProperies_ShouldLogError()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .Build();

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert
            Assert.Contains(new LogMessage(LogLevel.Error, "Unable to process published command without a command name."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishedCommand_WithNoCommandNameUserPropery_ShouldLogError()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("property", "value")
                .Build();

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert
            Assert.Contains(new LogMessage(LogLevel.Error, "Unable to process published command without a command name."), _log.LogMessages);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ProcessPublishedCommand_WithEmptyOrWhiteSpaceCommandName_ShouldLogError(string commandName)
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", commandName)
                .Build();

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert
            Assert.Contains(new LogMessage(LogLevel.Error, "Unable to process published command without a command name."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishedCommand_WithUntypedSubscriptionAndNoPayload_ShouldRunAction()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "exampleCommandName")
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe(cmd =>
            {
                // Assert
                Assert.Equal("exampleCommandName", cmd.CommandName);
                Assert.NotNull(cmd.Arguments);
                Assert.Empty(cmd.Arguments);

                Resolver.Log.Info($"Command action was performed.");
            });

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert 
            Assert.Contains(new LogMessage(LogLevel.Information, "Command action was performed."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishCommand_WithUntypedSubscriptionAndInvalidPayload_ShouldLogError()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "exampleCommandName")
                .WithPayload(Encoding.UTF8.GetBytes("[]"))
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe(cmd => Resolver.Log.Info($"Command action was performed."));

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert 
            Assert.DoesNotContain(new LogMessage(LogLevel.Information, "Command action was performed."), _log.LogMessages);
            Assert.Contains(_log.LogMessages, msg => msg.Level == LogLevel.Error);
        }

        [Fact]
        public void ProcessPublishedCommand_WithUntypedSubscription_ShouldRunAction()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "exampleCommandName")
                .WithPayload(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { ArgProperty = true })))
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe(cmd =>
            {
                // Assert
                Assert.Equal("exampleCommandName", cmd.CommandName);

                Assert.NotNull(cmd.Arguments);
                Assert.NotEmpty(cmd.Arguments);
                Assert.Equal("True", cmd.Arguments["ArgProperty"].ToString());

                Resolver.Log.Info($"Command action was performed.");
            });

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert
            Assert.Contains(new LogMessage(LogLevel.Information, "Command action was performed."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishedCommand_WithSubscribingToUntypedSubscriptionTwice_ShouldRunLastAction()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "exampleCommandName")
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe(cmd => Resolver.Log.Info($"Command action 1 was performed."));
            commandService.Subscribe(cmd => Resolver.Log.Info($"Command action 2 was performed."));

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert
            Assert.Contains(new LogMessage(LogLevel.Information, "Command action 2 was performed."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishCommand_WithSubscribingToUntypedSubscriptionAndUnsubcribing_ShouldNotRunAction()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "exampleCommandName")
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe(cmd => Resolver.Log.Info($"Command action was performed."));
            commandService.Unsubscribe();

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert
            Assert.DoesNotContain(new LogMessage(LogLevel.Information, "Command action was performed."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishCommand_WithTypedSubscriptionAndNoPayload_ShouldRunAction()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "testCommand")
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe<TestCommand>(cmd =>
            {
                // Assert
                Assert.NotNull(cmd);
                Assert.False(cmd.ArgProperty);

                Resolver.Log.Info($"Command action was performed.");
            });

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert 
            Assert.Contains(new LogMessage(LogLevel.Information, "Command action was performed."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishCommand_WithTypedSubscriptionAndInvalidPayload_ShouldLogError()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "testCommand")
                .WithPayload(Encoding.UTF8.GetBytes("[]"))
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe<TestCommand>(cmd => Resolver.Log.Info($"Command action was performed."));

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert 
            Assert.DoesNotContain(new LogMessage(LogLevel.Information, "Command action was performed."), _log.LogMessages);
            Assert.Contains(_log.LogMessages, msg => msg.Level == LogLevel.Error);
        }

        [Fact]
        public void ProcessPublishCommand_WithTypedSubscription_ShouldRunAction()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "testCommand")
                .WithPayload(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { ArgProperty = true })))
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe<TestCommand>(cmd =>
            {
                // Assert
                Assert.NotNull(cmd);
                Assert.True(cmd.ArgProperty);

                Resolver.Log.Info($"Command action was performed.");
            });

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert 
            Assert.Contains(new LogMessage(LogLevel.Information, "Command action was performed."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishCommand_WithTypedSubscriptionAndAdditionalData_ShouldRunAction()
        {
            var data = new TestCommandWithExtensionData
            {
                ArgProperty = true,
                AdditionalData = new Dictionary<string, object>
                  {
                      { "AnotherProperty", "anotherValue" }
                  }
            };

            var payload = JsonSerializer.Serialize(data);

            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "testCommandWithExtensionData")
                .WithPayload(Encoding.UTF8.GetBytes(payload))
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe<TestCommandWithExtensionData>(cmd =>
            {
                // Assert
                Assert.NotNull(cmd);
                Assert.True(cmd.ArgProperty);
                Assert.Equal("anotherValue", cmd.AdditionalData["AnotherProperty"].ToString());

                Resolver.Log.Info($"Command action was performed.");
            });

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert 
            Assert.Contains(new LogMessage(LogLevel.Information, "Command action was performed."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishCommand_WithTypedAndUntypedSubscription_ShouldRunBothActions()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "testCommand")
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe(cmd => Resolver.Log.Info($"Command action 1 was performed."));
            commandService.Subscribe<TestCommand>(cmd => Resolver.Log.Info($"Command action 2 was performed."));

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert
            Assert.Contains(new LogMessage(LogLevel.Information, "Command action 1 was performed."), _log.LogMessages);
            Assert.Contains(new LogMessage(LogLevel.Information, "Command action 2 was performed."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishCommand_WithTypedSubscriptionAndUntypedCommandName_ShouldNotRunAction()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "<<<MEADOWCOMMAND>>>")
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe<TestCommand>(cmd => Resolver.Log.Info($"Command action was performed."));

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert
            Assert.DoesNotContain(new LogMessage(LogLevel.Information, "Command action was performed."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishCommand_WithSubscribingToTypedSubscriptionAndUnsubcribing_ShouldNotRunAction()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "testCommand")
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe<TestCommand>(cmd => Resolver.Log.Info($"Command action was performed."));
            commandService.Unsubscribe<TestCommand>();

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert
            Assert.DoesNotContain(new LogMessage(LogLevel.Information, "Command action was performed."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishedCommand_WithSubscribingToTypedSubscriptionTwice_ShouldRunLastAction()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "testCommand")
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe<TestCommand>(cmd => Resolver.Log.Info($"Command action 1 was performed."));
            commandService.Subscribe<TestCommand>(cmd => Resolver.Log.Info($"Command action 2 was performed."));

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert
            Assert.Contains(new LogMessage(LogLevel.Information, "Command action 2 was performed."), _log.LogMessages);
        }

        [Fact]
        public void ProcessPublishedCommand_WithSubscribingToTypedSubscriptionTwiceInDifferentNamespaces_ShouldRunLastAction()
        {
            // Arrange
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("topic")
                .WithUserProperty("commandName", "testCommand")
                .Build();

            ICommandService commandService = _updateService;
            commandService.Subscribe<TestCommand>(cmd => Resolver.Log.Info($"Command action 1 was performed."));
            commandService.Subscribe<AnotherNamespace.TestCommand>(cmd => Resolver.Log.Info($"Command action 2 was performed."));

            // Act
            _updateService.ProcessPublishedCommand(message);

            // Assert
            Assert.DoesNotContain(new LogMessage(LogLevel.Information, "Command action 1 was performed."), _log.LogMessages);
            Assert.Contains(new LogMessage(LogLevel.Information, "Command action 2 was performed."), _log.LogMessages);
        }
    }

    public class TestCommand : IMeadowCommand
    {
        public bool ArgProperty { get; set; }
    }

    public class TestCommandWithExtensionData : IMeadowCommand
    {
        public bool ArgProperty { get; set; }

        public Dictionary<string, object> AdditionalData { get; set; }
    }

    public class TestLogProvider : ILogProvider
    {
        private bool _isEnabled = true;

        public List<LogMessage> LogMessages { get; } = new List<LogMessage>();

        public void Log(LogLevel level, string message, string? _)
        {
            if (!_isEnabled)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
            }

            LogMessages.Add(new LogMessage(level, message));
        }

        public void Disable()
        {
            _isEnabled = false;
        }
    }

    public record LogMessage(LogLevel Level, string Message) { }
}

namespace Core.Unit.Tests.UpdateServiceTests.AnotherNamespace
{
    public class TestCommand : IMeadowCommand
    {
        public bool ArgProperty { get; set; }

        public bool ArgPropertyTwo { get; set; }
    }
}
