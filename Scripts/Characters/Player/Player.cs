using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export]
    private AnimationPlayer _animationPlayerNode;
    [Export]
    private Sprite3D _spriteNode;
    private Vector2 _direction = Vector2.Zero;

    public override void _Ready()
    {
        this._animationPlayerNode = GetNode<AnimationPlayer>("AnimationPlayer");
        this._spriteNode = GetNode<Sprite3D>("Sprite3D");

        this._animationPlayerNode.Play(GameConstants.ANIMATION_IDLE);
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
            this._animationPlayerNode.Play(GameConstants.ANIMATION_MOVE);
            this._spriteNode.FlipH = this._direction.X < 0;
        }
        else
        {
            this._animationPlayerNode.Play(GameConstants.ANIMATION_IDLE);
        }
    }
}
