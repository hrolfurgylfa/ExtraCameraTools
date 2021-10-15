# Extra Camera Tools

This plugin adds a few extra camera tools, it allows you to add save points for your camera to return to at any time, it sets a limiter so that when you zoom in the hand tool always slows down (can be disabled), and it adds a slider to let you change the speed of the hand tool manually without moving the camera.

<img src="https://i.imgur.com/Vntb2s6.png" alt="An image of how the tool looks" width="600" height="auto"/>

## Installation

You can install the latest version as a Unity package from [here](https://github.com/hrolfurgylfa/ExtraCameraTools/releases), or you can clone this repository into any folder under Assets/Editor/

This tool only works on Unity 2019 and higher.

## Usage

To disable the plugin, go to Window > Toggle Extra Camera Tools.

Adding and removing locations is just the plus and minus as expected. To rename your saved locations, you can double-click the name. This can be changed to a single click, triple-click, or any other amount of clicks.

When zooming in all the way, the tool switches to the set "Max Zoom Move Speed" to make sure you can continue moving, you can adjust the speed of this in the settings under the name "Max Zoom Move Speed" or you can disable this behavior by unchecking "Disable Negative Scroll"

## Why?

I mainly developed this tool because of Unity's default behavior when zooming in, by default you can go past 0 into negative numbers, and then when you use the hand tool again, it changes to the absolute value of that, this means that the speed of the hand tool does not always decrease when you zoom in.

Here is a demonstration of that:

<img src="https://cdn.discordapp.com/attachments/566806197815214110/898373082115342366/ezgif-7-72e12f37ba87.gif" alt="A demonstration of Unity's default zoom in behavior" width="600" height="auto"/>

As can be seen, by only zooming in, the speed of the hand tool increased instead of decreasing.
