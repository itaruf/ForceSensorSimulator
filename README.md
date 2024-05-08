## AIST-VR-engineer-test
![Version](https://img.shields.io/badge/version-1.0.0-blue)
[![Youtube](https://img.shields.io/badge/demo-youtube-%23db1818)](https://youtu.be/ks2A9hrcGHI)

<!-- How to run the project -->
### How to run the project:
  - Version: Unity (2021.3.37f1) LTS
  - Platform: Windows

There are 2 main scenes: Test_Setup and Test_Transition_Setup (in the _Scenes folder).

### Test_Setup
Contains the following relevant game objects:
  - 2 Force Sensor + Arrow: display the arrow representing the force data information provided by the external library.
  - 1 Arrow Settings Panel: UI where the force sensors/arrows' global settings (e.g: force threshold, color selection for low and high magnitude values, etc.) are centralized.
  - 1 Scene Settings Manager: from where we can call the YAML parser at editor time, or at run time.

### Test_Transition_Setup
Contains the following relevant game objects:
   - 1 Scene Settings Manager

An empty scene showcasing the YAML parser, i.e. the extraction of data from a YAML file, the addition of missing game objects and the adjustments of some of their properties (e.g: position, rotation, colors, parent, etc.), at editor time or at run time.
