# Doors & Switches

Doors and switches act similarly to circuits.

**Interfaces**
* `ISignal` - Interface which represents something that outputs a `true | false` signal.
* `ISwitch` - Interface which represents something that can be turned on or off. It it also an `ISignal`.
    * `On -> true`
    * `Off -> false`
* `IDoor` - Interface which represents something that can be opened or closed.

**Classes**
* `DoorBase` - Base class from which different types of doors can inherit.
* `SwitchBase` - Base class from which different types of interactive switches can inherit.
* `SignalGate` - This takes a group is input signals and can `and | or` them together to generate a single output.
* `DoorSignalGate` - This takes a series of `ISignal` and `IDoor` and coordinates the output of the `ISignal` objects in order to trigger the `Open()` or `Close()` method on the linked `IDoor` objects.

## Guide

In order to use the Godot editor to link your switches and doors, you'll need to make use of the grouping functionality. Here are the list of steps.

1. Create a `DoorBase` object in the editor.
    1. In the `Node` properties, add a group (for example: `Door_1`).
1. Create a `SignalGate` (or subclass) object in the editor.
    1. In the `Node` properties, add a group (for example: `Switch_1`).
1. Create a `DoorSwitchControl` object in the editor.
    1. In the `Inspector` properities...
        1. Fill the `Input Group` property with the name of your switch group (`Switch_1`).
        1. Fill the `Door Group` property with the name of your door group (`Door_1`).

## Notes

* Attempted to use signals coming from `SwitchBase`, but there is a chance that `SwitchGroup` objects are `_Ready()` before each switch, which means you cannot immediately connect to signals.
    * This seems to result in a weird pattern where you have to check whether you've connected signals in the `_Process()` loop.
    * I wasn't a huge fan of this, so I'm bypassing that for now by relying on polling instead.        
