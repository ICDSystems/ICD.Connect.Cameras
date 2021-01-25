# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [5.1.1] - 2021-01-25
### Changed
 - Fixed issue where default comspec settings for VISCA cameras wasn't being applied

## [5.1.0] - 2021-01-15
### Added
 - Camera drivers report power state feedback

## [5.0.2] - 2020-09-24
### Changed
 - Fixed a bug where default camera activities were not being initialized

## [5.0.1] - 2020-07-14
### Changed
 - Fixed a bug with camera activity telemetry

## [5.0.0] - 2020-06-18
### Changed
 - MockCameraDevice now implements IMockDevice
 - Using new logging context
 - Refactoring for logging, telemetry, device controls, etc

## [4.6.0] - 2020-06-10
### Changed
 - Implemented StartSettings for Vaddio and Visca cameras to start communications

## [4.5.0] - 2020-03-20
### Added
 - Initial commit of ICD.Connect.Cameras.Windows project
 - Added WindowsUsbCameraDevice for representing static USB cameras

### Changed
 - Fixed web requests to use new web port response.
 - Differentiating between the stop methods in the pan-tilt and zoom camera control consoles.

## [4.4.0] - 2020-02-21
### Added
 - Added StoreHome method to cameras

## [4.3.0] - 2020-02-20
### Added
 - Vaddio driver supports returning to home position

## [4.2.0] - 2019-11-18
### Added
 - Added web proxy settings to Panasonic camera driver

## [4.1.0] - 2019-10-07
### Added
 - Added console command for printing stored camera presets
 
### Changed
 - Panasonic AW - Better handling of null port, more helpful exception logging

## [4.0.0] - 2019-01-10
### Added
 - Added port configuration features to camera devices

## [3.5.1] - 2020-02-18
### Changed
 - Fixed a bug where cameras would not stop tilting up

## [3.5.0] - 2020-02-14
### Changed
 - Substantial refactoring of cameras into a single interface

## [3.4.1] - 2019-06-07
### Changed
 - Vaddio Roboshot cameras now log errors 

## [3.4.0] - 2019-03-15
### Added
 - Added console features for camera devices

## [3.3.2] - 2018-11-08
### Changed
 - Fail gracefully when loading a device and the port is not available

## [3.3.1] - 2018-10-04
### Changed
 - Vaddio Roboshot has default username, password and ptz speeds

## [3.3.0] - 2018-09-25
### Added
 - Vaddio Roboshot driver

## [3.2.2] - 2018-09-14
### Changed
 - Performance improvements for camera routing

## [3.2.1] - 2018-07-02
### Changed
 - Various Visca camera driver fixes

## [3.2.0] - 2018-06-04
### Changed
 - Serial devices use ConnectionStateManager for maintaining connection to remote endpoints
 
## [3.1.0] - 2018-05-09
### Changed
 - Camera preset events are re-raised from the device

## [3.0.0] - 2018-04-23
### Added
 - Adding API attributes to cameras and camera controls

### Changed
 - Removed suffix from assembly name
 - Using new API event args
