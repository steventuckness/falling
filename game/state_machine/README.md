# State Machine

```cs
public class Player : Node2D {
    private sm = new StateMachine<Player>();
    public idle = new Idle();

    public override void _Ready() {
        this.sm.Init(this.idle);
    }

    public override void _PhysicsProcess(float delta) {
        this.sm.Update(delta, this);
    }
}
```
