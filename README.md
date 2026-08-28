# VISCA

## License

Provided under MIT license

## Overview

This repo is for VISCA camera plugin.

## Plugin Valid Communication methods

```c#
Comm
Tcpip
Udp
```

### RS-232 Communications

| Setting      | Value                       |
| ------------ | --------------------------- |
| Baud rate    | 9600 or 38,400 (selectable) |
| Data bits    | 8                           |
| Stop bits    | 1                           |
| Parity       | None                        |
| Flow control | none                        |

### VISCA-over-IP via TCP/IP Communications

| Setting      | Value |
| ------------ | ----- |
| Default IP   |       |
| Default Port | 5500  |
| Username     |       |
| Password     |       |

### VISCA-over-IP via UDP Communications

| Setting      | Value |
| ------------ | ----- |
| Default IP   |       |
| Default Port | 52381 |
| Username     |       |
| Password     |       |

<!-- START Minimum Essentials Framework Versions -->
### Minimum Essentials Framework Versions

- 3.0.0
<!-- END Minimum Essentials Framework Versions -->
<!-- START Config Example -->
### Config Example

```json
{
    "key": "GeneratedKey",
    "uid": 1,
    "name": "GeneratedName",
    "type": "ViscaCamera",
    "group": "Group",
    "properties": {
        "control": "SampleValue",
        "address": "SampleValue",
        "panSpeed": "SampleValue",
        "tiltSpeed": "SampleValue",
        "ZoomSpeed": "SampleValue",
        "FocusSpeed": "SampleValue",
        "PrivacyOnPreset": "SampleValue",
        "PrivacyOffPreset": "SampleValue",
        "pollTimeMs": 0,
        "presets": [
            {
                "name": "SampleString",
                "id": 0
            }
        ]
    }
}
```
<!-- END Config Example -->
<!-- START Supported Types -->

<!-- END Supported Types -->
<!-- START Join Maps -->
### Join Maps

#### Digitals

| Join | Type (RW) | Description |
| --- | --- | --- |
| 1 | R | Tilt Up |
| 2 | R | Tilt Down |
| 3 | R | Pan Left |
| 4 | R | Pan Right |
| 5 | R | Zoom In |
| 6 | R | Zoom Out |
| 7 | R | Camera power on |
| 8 | R | Camera power off |
| 9 | R | Is Online |
| 10 | R | Home |
| 11 | R | Preset select (press), store (hold) |
| 28 | R | Focus Near |
| 29 | R | FocusFar |
| 30 | R | Camera preset stored Feedback |
| 31 | R | Preset store |
| 48 | R | Privacy On |
| 49 | R | Privacy Off |
| 50 | R | Trigger auto focus |

#### Analogs

| Join | Type (RW) | Description |
| --- | --- | --- |
| 1 | R | Pan Speed |
| 2 | R | Tilt Speed |
| 3 | R | Zoom Speed |
| 4 | R | Focus Speed |
| 11 | R | Number of configured presets |
| 11 | R | Preset select by number |
| 12 | R | Preset store by number |
| 50 | R | Returns Socket Status when using VISCA-over-IP |

#### Serials

| Join | Type (RW) | Description |
| --- | --- | --- |
| 1 | R | Name |
| 11 | R | Preset Name |
| 50 | R | Camera device communications |
<!-- END Join Maps -->
<!-- START Interfaces Implemented -->
### Interfaces Implemented

- ICommunicationMonitor
- IRoutingSource
- IHasCameraOff
- IHasCameraPtzControl
- IHasCameraFocusControl
- IHasPowerControlWithFeedback
- IHasCameraPresets
<!-- END Interfaces Implemented -->
<!-- START Base Classes -->
### Base Classes

- EssentialsBridgeableDevice
- JoinMapBaseAdvanced
<!-- END Base Classes -->
<!-- START Public Methods -->
### Public Methods

- public void SendBytes(byte[] bytes)
- public void SendCustomCommand(string cmd)
- public void InitializeCamera()
- public void Poll()
- public void CameraOn()
- public void CameraOff()
- public void PanLeft()
- public void PanRight()
- public void PanStop()
- public void TiltDown()
- public void TiltUp()
- public void TiltStop()
- public void ZoomIn()
- public void ZoomOut()
- public void ZoomStop()
- public void FocusNear()
- public void FocusFar()
- public void FocusStop()
- public void TriggerAutoFocus()
- public void PositionHome()
- public void PresetSelect(int preset)
- public void PresetStore(int preset, string description)
- public void PrivacyOn()
- public void PrivacyOff()
<!-- END Public Methods -->
<!-- START Bool Feedbacks -->
### Bool Feedbacks

- PresetStoredFeedback
- OnlineFeedback
- CameraIsOffFeedback
- AutoFocusFeedback
<!-- END Bool Feedbacks -->
<!-- START Int Feedbacks -->
### Int Feedbacks

- NumberOfPresetsFeedback
- SocketStatusFeedback
- MonitorStatusFeedback
- PanSpeedFeedback
- TiltSpeedFeedback
- ZoomSpeedFeedback
- FocusSpeedFeedback
<!-- END Int Feedbacks -->
<!-- START String Feedbacks -->

<!-- END String Feedbacks -->
