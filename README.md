# MavpixelGUI
*Under Construction..*

MavpixelGUI is the Windows configurator application for Mavpixel.

Mavpixel brings Cleanflight-style LED strip functionality to the APM project.

Mavpixel is a LED strip controller designed as a companion for APM, Pixhawk and other Mavlink-compatible flight controllers. Based on simple, inexpensive hardware, Mavpixel is a well-featured complete LED lighting solution for larger UAVs.

For more information on the Mavpixel board including installation and wiring see the [Mavpixel Github](http://github.com/prickle/Mavpixel)

**Using MavpixelGUI**

MavpixelGUI is provided with layout and controls familiar to users of [Cleanflight](http://github.com/cleanflight/cleanflight). It acts as a minimal ground station and can connect to any available serial or network port or through another ground station in either Mavlink or CLI mode as appropriate. It also includes a firmware flasher for preparing and keeping Mavpixel boards up to date.

Because Mavpixel is based on Cleanflight LED control, much of the [Cleanflight LED strip documentation](https://github.com/cleanflight/cleanflight/blob/master/docs/LedStrip.md) is quite relevant, including the CLI commands. This information can be used to provide a perspective on the LED control system used by Mavpixel and MavpixelGUI.

Note that changes made in the GUI are activated on the Mavpixel only after the Send button at the bottom of the window is pressed.

**Connecting to Mavpixel**

MavpixelGUI can connect to Mavpixel in many ways, using existing channels or through new ones, either offline or while running.

Mavpixel has two serial ports, either of which can be used by MavpixelGUI directly. Connecting using the FTDI programmer gives quick offline access. A secondary configuration port is also available as described below.

When Mavpixel is connected to a vehicle the flight controller can forward it's messages over the Mavlink connection, allowing MavpixelGUI to connect through the flight controller's USB or network port. 

Mission Planner and other ground stations can also forward Mavlink messages to the network giving MavpixelGUI the ability to connect over the network, through a running ground station, back up to the vehicle, and on to a live Mavpixel for full remote configuration access. For more details see below.

*More to come..*
