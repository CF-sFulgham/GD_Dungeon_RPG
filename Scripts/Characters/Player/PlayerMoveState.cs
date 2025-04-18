using Godot;
using System;

public partial class PlayerMoveState : PlayerBaseState
{
    protected override void PlayAnimation()
    {
        this._animationPlayerNode.Play(GameConstants.ANIMATION_MOVE);
    }

}
