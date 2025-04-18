using Godot;

public abstract partial class PlayerBaseState : Node
{
    protected AnimationPlayer _animationPlayerNode { get; private set; }

    public override void _Ready()
    {
        this._animationPlayerNode = GetParent().GetNode<AnimationPlayer>("AnimationPlayer");
    }

    protected virtual void PlayAnimation(){}
}