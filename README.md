[![openupm](https://img.shields.io/npm/v/net.mirzipan.scheduler?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/net.mirzipan.scheduler/) ![GitHub](https://img.shields.io/github/license/Mirzipan/Mirzipan.Scheduler)

# Mirzipan.Scheduler

Simple update scheduler for Unity. Use this if you want to have better control when your updates get called and free up some performance by not having to call Update when not necessary.

## Updater

The basic idea is that whatever callback you schedule, the only guarantee is that the callback will not be called before its due time. There is, however, no guarantee that it will be called precisely when scheduled.

### Constructor

```csharp
Scheduler(double frameBudget)
```
`frameBudget` - the maximum amount of time per one invoke of the `Tick` method

### Tick

```csharp
void Tick(double time)
```
`time` - game time according to which scheduled updates should be invoked.

Starts calling scheduled updates until it exceeds the `frameBudget` specified within constructor.

### Dispose

```csharp
void Dispose()
```
Basic cleanup when getting rid of the scheduler.

### SetFrameBudget

```csharp
void SetFrameBudget(double frameBudget)
```
Sets the new frame budget. Useful when changing target framerate of the application during runtime.

### Schedule

```csharp
public delegate void DeferredUpdate(double elapsedTime);

IDisposable Schedule(double dueTimeInSeconds, DeferredUpdate update)
IDisposable Schedule(double dueTimeInSeconds, double period, DeferredUpdate update)
```
The specified callback will be called after `dueTime` has elapsed, but not before.
Optionally, a `period` can be specified to make this into a recurring callback.
Times can be specified either using `double` (specifies seconds) or `TimeSpan`.
The callback can be unscheduled by disposing of the `IDisposable` object.

_Notice: Callback will never be called within the same tick as it was scheduled._

### Unschedule

```csharp
void Unschedule(DeferredUpdate update)
```
The specified callback will be unregistered from scheduled updates.

### Clear

All scheduled updated will be unscheduled.

## Ticker
Calls registered updates each tick, regardless of time.

### Add
```csharp
IDisposable Add(TickUpdate update)
IDisposable Add(TickUpdate update, int priority)
```
`priority` - priority with which the update is to be called (higher means sooner).

Adds the specified callback to the list of updates to be called during a tick. 

### Remove
```csharp
bool Remove(TickUpdate update)
```
Removes the specified callback from the list of updates to be called during a tick. 