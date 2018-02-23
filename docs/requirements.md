# 1. Buttons

1. There shall exist a record button
1. There shall exist a jump button
1. There shall exist a select next clone button
1. There shall exist a select previous clone button
1. There shall exist (technically "dpad up") a show clone selection screen button
1. There shall exist a pause button
1. There shall exist a clone delete button

# 2. Player

1. The player shall jump
1. The player shall walk left and right
1. Recording of the player movements should start once the record button is pressed
    1. Once the record button is pressed again, the recording stops, and a new clone is created where you originally started the recording.

# 3. Clone

1. The clone repeats the same movements of the player, as recorded by requirement 2.3
1. The clone shall dissapear once the player starts recording another clone of the same color
1. The clone shall push the player around and not the opposite

# 4. Clone Selection Menu

1. Is triggered by the clone selection button
1. Contains *just* a list of clones of different colors (simple boxes for now), no background or nothing
1. The current selected clone is marked with an indicator
1. If the clone selection button is released, the screen dissapears, and the player now becomes of the same color as the selected clone
1. The next and previous clone buttons will move the indicator left and right in order to select clones

# 5. Obstacles

1. Spikes
1. Door
    1. Can be connected to more than one switch in order to open
1. Switches
    1. The switch shall be able to be placed anywhere in the level and connected
    to one or more doors
1. Moving platforms

# 6. Start screen

1. Shall have a press any button to continue
1. After pressing a button, it goes to the level selection screen

# 7. Save game mechanics

1. The game shall store only the levels that were cleared along with stats
1. Stats are: level completion time, number of clones created
1. The game is "saved" every time the level is cleared
1. There is only one save game

# 8. Level

1. Each level should have a limited amount of colors you can pick

# 9. Level selection screen

1. Shall list all levels
1. Only levels that have been cleared (reading from the savegame) are available for selection.
1. A level can be selected by moving arrow keys or dpad
1. The press of a button selects a level

# 10. Level done screen

1. Shall list the time that it took to complete the level
1. Shall list the number of clones that it took to complete the level

# 11. Pause screen

1. Shall have an option to go back to level select screen
1. Shall have an option to resume
