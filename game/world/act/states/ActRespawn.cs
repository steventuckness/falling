using Godot;
using System;

public class ActRespawn : State<Act> {
    public override void OnEnter(float delta, Act owner) {
    }
    public override void OnExit(float delta, Act act) {
        act.GetPlayer().Respawn();
    }
    public override State<Act> Update(float delta, Act act, float timeInState) {
        if(timeInState < act.respawnTime) {
            return null;
        }

        Node2D spawn = act.GetSpawn();
        PlayerNode player = act.GetPlayer();

        Vector2 spawnPosition = spawn.GetPosition();
        Vector2 playerPosition = player.GetPosition();

        // If the player is now close enough to the spawn point, we're good
        if (playerPosition.DistanceTo(spawnPosition) < 0.1f) {
            return act.statePlay;
        }

        // Move the player back to the spawn position
        player.SetPosition(
            playerPosition.LinearInterpolate(spawnPosition, 5f * delta)
        );

        return null;
    }
}
