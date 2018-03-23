# Spikes

Spikes are a tilemap, which have the graphics, plus an Area2D which detects the collisions. The Area2D has `DeathArea.cs` attached. Any `Area2D` that has that script will automatically kill the player if the player enters it.

**Example**

```
Spikes (Node2D)
  |_ Tilemap 
  |_ Area2D (DeathArea.cs)
```
