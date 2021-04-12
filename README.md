# Visca Camera Essentials Plugin (c) 2021

## License

Provided under MIT license

## Overview

The Visca Camera plugin provides device control over the Sony's VISCA protocol standard family cameras with regards to the most commonly used and requested attriute and control types.
Plugin is based on ViscaLibrary that implement protocol classes and reference implementation of camera object that this plugin wrap up.

## Cloning Instructions

After forking this repository into your own GitHub space, you can create a new repository using this one as the template.  Then you must install the necessary dependencies as indicated below.

## Dependencies

The [Essentials](https://github.com/PepperDash/Essentials) libraries are required. They referenced via nuget. You must have nuget.exe installed and in the `PATH` environment variable to use the following command. Nuget.exe is available at [nuget.org](https://dist.nuget.org/win-x86-commandline/latest/nuget.exe).

### Installing Dependencies

To install dependencies once nuget.exe is installed, run the following command from the root directory of your repository:
`nuget install .\packages.config -OutputDirectory .\packages -excludeVersion`.
To verify that the packages installed correctly, open the plugin solution in your repo and make sure that all references are found, then try and build it.

### Installing Different versions of PepperDash Core

If you need a different version of PepperDash Core, use the command `nuget install .\packages.config -OutputDirectory .\packages -excludeVersion -Version {versionToGet}`. Omitting the `-Version` option will pull the version indicated in the packages.config file.

### Instructions for Renaming Solution and Files

See the Task List in Visual Studio for a guide on how to start using the templage.  There is extensive inline documentation and examples as well.

For renaming instructions in particular, see the XML `remarks` tags on class definitions

## Controls and Configs

### Config Note

 - Plugin will honor "enabled" property, if missed, it will not start communication with underlying serial port.
 - Poll intervals and Poll commands along with other communication parameters can be altered through `communicationMonitorProperties` config object.
Current list of supported Poll comands: `AE,Aperture,BackLight,BGain,ExpComp,FocusAuto,FocusPosition,Gain,Iris,Mute,PTZPosition,Power,RGain,Shutter,Title,WB,WD,ZoomPosition` 
If not using directly  Visca.Camera object Plugin itself use only following Iquiry commands: `Mute,Power`
 - Plugin implements Increase Speed Behavior when defined `fastSpeedHoldTimeMs`: after pan or tilt command speed will increase after `fastSpeedHoldTimeMs` millisecond to ether defined `xxxSpeedFast` values or maximum defined by VISCA.
```
      {
        "key": "cam-1",
        "uid": 34,
        "name": "Projection Camera",
        "type": "cameravisca2",
        "group": "cam",
        "properties": {
          "control": {
            "comParams": {
              "baudRate": 9600,
              "dataBits": 8,
              "parity": "None",
              "stopBits": 1,
              "protocol": "RS232",
              "softwareHandshake": "None",
              "hardwareHandshake": "None"
            },
            "controlPortNumber": 1,
            "controlPortDevKey": "cam-hdbt-1",
            "method": "Com"
          },
          "id": 1,
          "enabled": true,
          "communicationMonitorProperties": {
            "PollInterval": 10000,
            "TimeToWarning": 20000,
            "TimeToError": 30000,
            "PollString": "AE,Aperture,BackLight,BGain,ExpComp,FocusAuto,FocusPosition,Gain,Iris,Mute,PTZPosition,Power,RGain,Shutter,WB,WD,ZoomPosition"
          },
          "homeCmdSupport": true,
          "fastSpeedHoldTimeMs": 2000,
          "panSpeedSlow": 8,
          "panSpeedFast": 16,
          "tiltSpeedSlow": 8,
          "tiltSpeedFast": 14
        }
      },

```