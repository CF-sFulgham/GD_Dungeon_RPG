using Godot;
using System;

public partial class Player : CharacterBody3D
{
    private Sprite3D _spriteNode;
    private Vector2 _direction = Vector2.Zero;
    private StateMachine _stateMachine;

    public override void _Ready()
    {
        this._spriteNode = GetNode<Sprite3D>("Sprite3D");
        this._stateMachine = GetParent().GetNode<StateMachine>("StateMachine");
    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = new Vector3(_direction.X, 0, _direction.Y) * 5.0f;
        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        this._direction = Input.GetVector(
            GameConstants.INPUT_LEFT, 
            GameConstants.INPUT_RIGHT, 
            GameConstants.INPUT_UP,
            GameConstants.INPUT_DOWN
            );

        if (this._direction != Vector2.Zero)
        {
            this._spriteNode.FlipH = this._direction.X < 0;
            this._stateMachine.ChangeState<PlayerMoveState>();
        }
        else
        {
            this._stateMachine.ChangeState<PlayerIdleState>();
        }
    }
}
