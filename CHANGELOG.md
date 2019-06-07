# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
 - Added console command for printing stored camera presets
 
### Changed
 - Panasonic AW - Better handling of null port, more helpful exception logging

## [4.0.0] - 2019-01-10
### Added
 - Added port configuration features to camera devices

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
