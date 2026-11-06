# Railroader Raildriver Support

This mod adds support for the Raildriver controller in Railroader for the experimental MoW update (version 0.6). It is based on the [RailroaderRD](https://github.com/rileyzzz/RailroaderRD) mod for the prior version, but ports the core code and adds several enhancements. The prior project is unmaintained, but if it revives this code can be merged back in.

# Installation

There is an optional dependency on the [ModSettingsHub](https://github.com/Exp232/ModSettingsHub) mod. If you have it installed, you'll be able to use the universal mod settings window to calibrate as described below. If not, you will need to use the Ctrl+F7 hotkey to bring up the calibration window.

Make sure your version of Railroader is at least 0.6 or later. Currently, 0.6 is an experimental build and to enable it, you need to go to your game properties in Steam, click the Betas tab, then select the appropriate build.

At that point, you can download this mod and extract it into the mods directory as explained in the mod installation [documentation](https://docs.railroadergame.com/modding/installing-mods/). The directory structure should look like `Mods -> RailroaderRD -> <all the mod files>`. Move things around as needed if it is wonky.

Once the files are installed in the proper location, run Railroader and you must select the option in the main menu to enable DLL mods. You can then go to the mods menu and click to enable the `Railroader Raildriver Support` mod.

# Calibration

After loading up a game with the new mod enabled, you will need to calibrate. Calibration data is automatically saved to the mod directory so you should only need to do this once.

Go to the mod settings menu (or Ctrl+F7 if you skipped the ModSettingsHub dependency). There you will find a button for each control position that needs to be calibrated. Go through this screen systematically moving the specified control to the required position and clicking the button. If you do it right, you should be able to move the control through its range of motion and see each indicator bar move along with it.

# Controls and Usage

Make sure to plug your Raildriver in before starting the game. If the raildriver is disconnected, you will need to quit and restart the game to reconnect it.

The digital display normally displays your speed. If it never changes from `rd` after you start, it means it was never properly connected. If it says `LOC`, that means that you do not have a locomotive selected. If it says `AE`, that means the current locomotive is set to be driven by an Auto-Engineer and so controls are disabled.

Beneath the digital display, the E-Stop rocker on the top-right of the bank of switches is the 'quill' or soft whistle/horn. The circle button in the bottom-right is the bell toggle. And the tall lever to the right of the bank of buttons is the full horn.

The four levers are in order: Reverser, Throttle, Train Brake, Independent Brake.

The reverser lever: up is forward, the detente in the middle is neutral, and the bottom is reverse.

The throttle lever: the top half (dynamic braking) is not used. 0 throttle is just below the middle notch. 100 throttle is at the bottom.

The train brake: full brake engagement is at the top just below the emergency stop notch. Disengaged is the bottom.

The independent brake: full brake engagement is at the top, disengaged is at the bottom. This lever also moves to the right to trigger bail off.

The upper-right rotary and lower-right rotary controls are used for lights. The upper-right is off/dim/full for forward lights and the bottom-right is off/dim/full for rear lights.


