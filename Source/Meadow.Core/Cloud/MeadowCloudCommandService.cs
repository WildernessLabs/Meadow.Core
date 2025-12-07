using Meadow.Cloud;
using MQTTnet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Meadow;

/// <summary>
/// Provides functionality for subscribing to and processing commands received from the Meadow Cloud service.
/// </summary>
/// <remarks>This service allows clients to subscribe to commands of specific types or to generic Meadow commands.
/// Commands are received via the associated <see cref="MeadowCloudConnectionService"/> and dispatched to the
/// appropriate subscribers.</remarks>
public class MeadowCloudCommandService : ICommandService
{
    private const string UntypedCommandTypeName = "<<<MEADOWCOMMAND>>>";

    private readonly ConcurrentDictionary<string, (Type CommandType, Action<object> Action)> _commandSubscriptions = new();
    private readonly MeadowCloudConnectionService _connectionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeadowCloudCommandService"/> class.
    /// </summary>
    /// <remarks>This constructor subscribes to the MQTT topic "{OID}/commands/{ID}" and attaches an event
    /// handler  to process incoming MQTT messages via the <see
    /// cref="MeadowCloudConnectionService.MqttMessageReceived"/> event.</remarks>
    /// <param name="meadowCloudService">The <see cref="IMeadowCloudService"/> instance used to manage the connection and handle MQTT messages.</param>
    public MeadowCloudCommandService(IMeadowCloudService meadowCloudService)
    {
        var connectionService = meadowCloudService as MeadowCloudConnectionService;

        if (connectionService != null)
        {
            _connectionService = connectionService;
            _connectionService.MqttMessageReceived += OnMqttMessageReceived;
            _connectionService.AddSubscription("{OID}/commands/{ID}");
        }
        else
        {
            throw new ArgumentException("meadowCloudService must be of type MeadowCloudConnectionService", nameof(meadowCloudService));
        }
    }

    private void OnMqttMessageReceived(object sender, MqttApplicationMessage e)
    {
        if (e.Topic.EndsWith($"/commands/{Resolver.Device.Information.UniqueID}", StringComparison.OrdinalIgnoreCase))
        {
            Resolver.Log.Info("Meadow command received", "cloud");
            ProcessPublishedCommand(e);
        }
    }

    /// <summary>
    /// Subscribes to receive notifications for commands of type <see cref="MeadowCommand"/>.
    /// </summary>
    /// <remarks>This method allows you to register a handler for processing commands of type <see
    /// cref="MeadowCommand"/>.  Only one subscription is allowed per command type; calling this method again for the
    /// same command type will overwrite the previous subscription.</remarks>
    /// <param name="action">The callback to invoke when a <see cref="MeadowCommand"/> is received. The callback takes a single parameter of
    /// type <see cref="MeadowCommand"/>.</param>
    public void Subscribe(Action<MeadowCommand> action)
    {
        _commandSubscriptions[UntypedCommandTypeName] = (CommandType: typeof(MeadowCommand), Action: x => action((MeadowCommand)x));
    }

    void ICommandService.Subscribe<T>(Action<T> action)
    {
        var commandTypeName = typeof(T).Name;
        _commandSubscriptions[commandTypeName.ToUpperInvariant()] = (CommandType: typeof(T), Action: x => action((T)x));
    }

    /// <summary>
    /// Unsubscribes from the current command, removing the associated subscription.
    /// </summary>
    /// <remarks>This method removes the subscription for the command identified by the current instance. 
    /// After calling this method, the command will no longer receive notifications or updates.</remarks>
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
                arguments = message.PayloadSegment.Count > 0
                    ? Resolver.JsonSerializer.Deserialize<Dictionary<string, object>>(message.PayloadSegment.Array)
                    : null;
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to deserialize command arguments: {ex.Message}");
                return;
            }

            var command = new MeadowCommand(commandName, arguments);
            value.Action(command);
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
                command = message.PayloadSegment.Count > 0
                    ? Resolver.JsonSerializer.Deserialize(message.PayloadSegment.Array, value.CommandType) ?? Activator.CreateInstance(subscription.Value.commandType)
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
