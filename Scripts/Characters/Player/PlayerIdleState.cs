using Godot;
using System;

public partial class PlayerIdleState : PlayerBaseState
{
    protected override void PlayAnimation()
    {
        this._animationPlayerNode.Play(GameConstants.ANIMATION_IDLE);
    }

}
