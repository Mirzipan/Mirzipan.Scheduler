# Changelog

## [1.2.0] - 2023-04-15

### Added
- Ticker with registered updates called each frame

### Changed
- Scheduler is now called Updater
- removed IProvideTime and Options from Updater constructor
- Tick method on Updater now takes time as parameter

### Removed
- Options

## [1.1.1] - 2023-02-21

### Fixed
- Fixed milliseconds instead of seconds being used in scheduling

## [1.1.0] - 2023-02-21

### Added
- SetFrameBudget method to change the frame budget on the fly

### Removed
- Schedule methods with TimeSpan (they are useful, but they are better suited for wrappers)
- Option to smear updates over frames is no longer available, it is the default behaviour

### Changed
- Scheduling and ticking logic has been tweaked a little, partially to get rid of the goto

## [1.0.1] - 2023-02-18

### Fixed
- SchedulerTicker was hiding the Singleton Awake method

## [1.0.0] - 2023-02-11

### Added
- Scheduler for deferred updates
- Example usage from MonoBehaviour