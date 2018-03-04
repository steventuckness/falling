using System;
using Godot;

public class Falling : State<Player> {
    
    private float groundImpactSpeed = 0.0f; // px / sec
    private bool heldJumpWhileEntering = false;
    private long timeWhenGlideLastPressed = 0; // milliseconds

    
    public override void OnEnter(float delta, Player owner) {
        owner.PlayAnimation(Player.Animation.Falling);
        this.heldJumpWhileEntering = Input.IsActionPressed("key_jump");
    }

    public override void OnExit(float delta, Player owner) {
    }

    public override State<Player> Update(float delta, Player player, float timeInState) {        
        player.DetectDirectionChange();
        if(player.IsOnFloor()) {
            return player.stateIdle;
        }
        player.AirControl(delta);
        player.ApplyGravity(delta);
        player.Move(0f);
        return null;
    }
}
