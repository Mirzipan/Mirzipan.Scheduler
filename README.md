# Mirzipan.Scheduler

Simple update scheduler for Unity. Use this if you want to have better control when your updates get called and free up some performance by not having to call Update when not necessary.

## Scheduler

The basic idea is that whatever callback you schedule, the only guarantee is that the callback will not be called before its due time. There is, however, no guarantee that it will be called precisely when scheduled.

### Constructor

```csharp
Scheduler(IProvideTime time, double frameBudget, Options options = Options.SmearUpdates)
```
`time` - provider of time for scheduling purposes

`frameBudget` - the maximum amount of time per one invoke of the `Tick` method

`options` - options which modify the behaviour of scheduler

### Tick

```csharp
void Tick()
```
Starts calling scheduled updates until it exceeds the `frameBudget` specified within constructor.

### Dispose

```csharp
void Dispose()
```
Basic cleanup when getting rid of the scheduler.

### Schedule

```csharp
ScheduleHandle Schedule(DeferredUpdate update, double dueTime, double period = 0d)
```
The specified callback will be called after `dueTime` has elapsed, but not before.
Optionally, a `period` can be specified to make this into a recurring callback.
The callback can be unscheduled by disposing of the `ScheduleHandle` object.

_Notice: Callback will never be called within the same tick as it was scheduled._

### Unschedule

```csharp
void Unschedule(DeferredUpdate update)
```
The specified callback will be unregistered from scheduled updates.

### Clear

All scheduled updated will be unscheduled.

## Options

### SmearUpdates

If the `Tick` is taking longer than the allowed `frameBudget`, the current `Tick` will end all further eligible updates will be moved to the following invoke of `Tick`.
