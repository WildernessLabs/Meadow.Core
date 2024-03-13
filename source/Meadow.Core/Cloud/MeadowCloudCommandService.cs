using Meadow.Cloud;
using MQTTnet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Meadow;

internal class MeadowCloudCommandService : ICommandService
{
    private const string UntypedCommandTypeName = "<<<MEADOWCOMMAND>>>";

    private readonly ConcurrentDictionary<string, (Type CommandType, Action<object> Action)> _commandSubscriptions = new();
    private readonly MeadowCloudConnectionService _connectionService;

    public MeadowCloudCommandService(MeadowCloudConnectionService connectionService)
    {
        _connectionService = connectionService;
        _connectionService.MqttMessageReceived += OnMqttMessageReceived;
        _connectionService.AddSubscription("{OID}/commands/{ID}");
    }

    private void OnMqttMessageReceived(object sender, MqttApplicationMessage e)
    {
        if (e.Topic.EndsWith($"/commands/{Resolver.Device.Information.UniqueID}", StringComparison.OrdinalIgnoreCase))
        {
            Resolver.Log.Info("Meadow command received", "cloud");
            ProcessPublishedCommand(e);
        }
    }

    public void Subscribe(Action<MeadowCommand> action)
    {
        _commandSubscriptions[UntypedCommandTypeName] = (CommandType: typeof(MeadowCommand), Action: x => action((MeadowCommand)x));
    }

    void ICommandService.Subscribe<T>(Action<T> action)
    {
        var commandTypeName = typeof(T).Name;
        _commandSubscriptions[commandTypeName.ToUpperInvariant()] = (CommandType: typeof(T), Action: x => action((T)x));
    }

    public void Unsubscribe()
    {
        _commandSubscriptions.TryRemove(UntypedCommandTypeName, out _);
    }

    void ICommandService.Unsubscribe<T>()
    {
        var commandTypeName = typeof(T).Name;
        _commandSubscriptions.TryRemove(commandTypeName.ToUpperInvariant(), out _);
    }

    internal void ProcessPublishedCommand(MqttApplicationMessage message)
    {
        if (message.UserProperties == null)
        {
            Resolver.Log.Error("Unable to process published command without a command name.");
            return;
        }

        var properties = message.UserProperties.ToDictionary(x => x.Name.ToUpperInvariant(), x => x.Value);

        if (!properties.TryGetValue("COMMANDNAME", out string commandName) ||
            string.IsNullOrWhiteSpace(commandName))
        {
            Resolver.Log.Error("Unable to process published command without a command name.");
            return;
        }

        // First attempt to run the untyped command subscription, Action<MeadowCommand>, if available.
        if (_commandSubscriptions.TryGetValue(UntypedCommandTypeName, out (Type CommandType, Action<object> Action) value))
        {
            Resolver.Log.Trace($"Processing generic Meadow command with command name '{commandName}'...", "cloud");

            IReadOnlyDictionary<string, object>? arguments;
            try
            {
                arguments = message.Payload != null
                    ? Resolver.JsonSerializer.Deserialize<IReadOnlyDictionary<string, object>>(message.Payload)
                    : null;
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to deserialize command arguments: {ex.Message}");
                return;
            }

            var command = new MeadowCommand(commandName, arguments);
            value.Action(command);
            return;
        }

        (Type commandType, Action<object> action)? subscription = null;

        if (_commandSubscriptions.TryGetValue(commandName.ToUpperInvariant(), out value))
        {
            subscription = value;
        }
        else if (_commandSubscriptions.TryGetValue($"{commandName.ToUpperInvariant()}COMMAND", out value))
        {
            subscription = value;
        }

        // Then attempt to run the typed command subscription, Action<T> where T : ICommand, new(),
        // if available. Also prevent user from running the untyped command subscription.
        if (subscription != null)
        {
            Resolver.Log.Trace($"Processing Meadow command of type '{subscription.Value.commandType.Name}'...", "cloud");

            object command;
            try
            {
                command = message.Payload != null
                    ? Resolver.JsonSerializer.Deserialize(message.Payload, value.CommandType) ?? Activator.CreateInstance(subscription.Value.commandType)
                    : Activator.CreateInstance(subscription.Value.commandType);
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to deserialize command arguments: {ex.Message}");
                return;
            }

            subscription.Value.action.Invoke(command);
        }
    }
}
