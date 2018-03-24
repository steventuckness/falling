using Godot;
using System;

public class ActRespawn : State<Act> {
    private SubPixelFloat move;

    public override void OnEnter(float delta, Act owner) {
        owner.GetPlayer().Show();
        move = new SubPixelFloat();
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
        if (playerPosition.DistanceTo(spawnPosition) <= 1) {
            return act.statePlay;
        }

        Vector2 lerpedPosition = playerPosition.LinearInterpolate(spawnPosition, 5f * delta);
        Vector2 diff = this.move.Update(lerpedPosition - playerPosition);

        // Move the player back to the spawn position
        player.SetPosition(playerPosition + diff);

        return null;
    }
}
