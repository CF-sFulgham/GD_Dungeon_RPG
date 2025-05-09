using Godot;

public abstract partial class CharacterBaseState : Node
{
    protected AnimationPlayer _animationPlayerNode { get; private set; }

    public override void _Ready()
    {
        this._animationPlayerNode = GetParent().GetNode<AnimationPlayer>("AnimationPlayer");
    }

    protected virtual void PlayAnimation(){}
}