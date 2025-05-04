using System.Dynamic;
using Godot;
public abstract partial class Character : CharacterBody3D
{

    protected Sprite3D _spriteNode { get; private set; }
    protected CharacterBody3D _characterNode { get; private set; }
    protected Vector2 _direction { get; set; } = Vector2.Zero;
    protected float _speed { get; set; } = 5.0f;
    protected StateMachine _stateMachine { get; private set; }

    public override void _Ready()
    {
        this._spriteNode = GetNode<Sprite3D>("Sprite3D");
        this._characterNode = this;
        this._stateMachine = GetTree()
            .Root
            .GetNode<Node>("Main")
            .GetNode<StateMachine>("StateMachine");
    }
}