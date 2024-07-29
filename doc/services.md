# Meadow Core Services

## MeadowCloudConnectionService
## MeadowCloudCommandService
## MeadowCloudUpdateService
## SensorService

## ReliabilityService

Part of platform stability, beyond not crashing, is the ability to detect crashes and to manage the device restart whenever possible.

The Meadow `ReliabilityService` provides applications an aggregation of data and behaviors related to device reliability.

### Handling crashes

Crashes generally fall into two categories: stable and unstable.  

An unstable crash leaves the device oeprating system or the .NET runtime in an unstable state and the application has no opportunity to coordinate a shutdown.  Examples might be an unhandled applicatin exception or an out-of-memory condition.  In these cases the device will restart without providing the application any pre-restart warning (generally the app is dead at this point and couldn't do anything anyway).  When the application comes back up, it has a variety of ways to know the crash occurred and to get data about the crash.  These are discussed in more detail in the next section.

A stable crash is when a subsystem for the device has failed, but the application can continue to run, even if it's in a degraded state.  An example of this is the network subsystem crashing.  In this case the application may not be able to execute all of it's features (like sending data to the Cloud) but it can still execute.  In these cases, the application is notified of the crash and provided a recommendation on whether the device should be restarted to recover full capabilities.  The application can then do things like save state or go through an ordered shutdown procedure before restarting.

Stable crashes are handled by subscribng to the `RelaibilityService.MeadowSystemError` event.  The handler will have the following signature:

```
void OnMeadowSystemError(MeadowSystemErrorInfo error, bool recommendReset, out bool forceReset)
```

Information about the error will be passed to the application along with a boolean flag that will be *true* if the device should be reset to recover.  The application can then set the `forceReset` parameter to *true* to let the `ReliabilityService` reset the device, or alternatively return *false* and handle resetting the device itself.

If no `MeadowSystemError` handler is registered by the application, the `ReliabilityService` will automatically follow the system reset recommendation and reset automatically where appropriate.


### Booting after a crash

It is often useful to know if your application is starting after a crash.  The `ReliabilityService` provides information on this state through the following properties and methods:

- `LastBootWasFromCrash` will return true if the app is booting after a crash.  This property signals if there is crash data available (i.e. if you don't clear crash data, it will continue to return true)
- `IsCrashDataAvailable` will return true if any crash data is available.  Crash data can come from multiple sources (app crash, runtime failure, OS failure, coprocessor failure, etc)
- `GetCrashData()` will retrieve all crash data.
- `ClearCrashData()` erases stored crash data, returning the devcie to a clean state
- `LastResetReason` returns a `ResetReason` enumeration corresponding to the reason for the last devcie reset
- `SystemResetCount` returns the total number of lifetime resets the device has seen
- `SystemPowerCycleCount` returns the total number of cold boots (non-reset power ups) the device has seen
