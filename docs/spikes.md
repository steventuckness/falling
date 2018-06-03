# Spikes

Spikes are a tilemap, which have the graphics, plus an Area2D which detects the collisions. The Area2D has `game/world/DeathArea.cs` attached. Any `Area2D` that has that script will automatically kill the player if the player enters it.

The tileset to be used is at `/world/hazards/Tileset_Spikes.res` and the tileset size must be set to `16x16`.

**Example**

```
Spikes (Node2D)
  |_ Tilemap 
    |_ Tile Set = /world/hazards/Tileset_Spikes.res
    |_ Size = (16, 16)
  |_ Area2D (DeathArea.cs)
```
