using Godot;
using System;

public partial class PlayerDashState : Node, IPlayerStates
{
    public AnimationPlayer _animationPlayerNode { get; set; }

    public override void _Ready()
    {
        this._animationPlayerNode = GetParent().GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void PlayAnimation()
    {
        this._animationPlayerNode.Play(GameConstants.ANIMATION_DASH);
    }
}
