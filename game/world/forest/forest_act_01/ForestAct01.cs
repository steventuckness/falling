using Godot;
using System;

public class ForestAct01 : Node2D {
    private Player p;
    private Node2D spawnPoint;
    private Node2D spawnCam;
    private Vector2 deathPoint;
    private float cameraTimeout = 2f;
    private enum CameraState {
        WATCHING,
        WAITING,
        MOVING_TO_SPAWN
    };
    private CameraState cameraState;

    public override void _Ready() {
        this.cameraState = CameraState.WATCHING;

        this.p = (Player)this.GetNode("player");
        this.spawnCam = (Node2D)this.GetNode("SpawnCam");
        this.spawnPoint = (Node2D)this.GetNode("Spawn");
    }

    public override void _PhysicsProcess(float delta) {
        switch (this.cameraState) {
            case CameraState.WATCHING:
                if (this.p.IsDead()) {
                    Camera2D playerCam = (Camera2D) this.p.GetNode("Camera2D");
                    Camera2D cam = (Camera2D)this.spawnCam.GetNode("Camera2D");

                    // To prevent jitter, we use the camera's position, not the player!
                    this.deathPoint = playerCam.GetCameraPosition();
                    this.spawnCam.SetGlobalPosition(this.deathPoint);

                    cam.MakeCurrent();

                    this.cameraState = CameraState.WAITING;
                }
                break;
            case CameraState.WAITING:
                this.cameraTimeout -= delta;
                if (this.cameraTimeout <= 0) {
                    this.cameraState = CameraState.MOVING_TO_SPAWN;
                    this.cameraTimeout = 2f;
                }
                break;
            case CameraState.MOVING_TO_SPAWN:
                Vector2 currentPos = this.spawnCam.GetGlobalPosition();
                Vector2 spawnPos = this.spawnPoint.GetGlobalPosition();

                currentPos.x = Mathf.Lerp(currentPos.x, spawnPos.x, 2f * delta);
                currentPos.y = Mathf.Lerp(currentPos.y, spawnPos.y, 2f * delta);

                this.spawnCam.SetGlobalPosition(currentPos);
                break;
        }
    }
}
