using Godot;
using System;

public partial class PlayerDashState : CharacterBaseState
{
    protected override void PlayAnimation()
    {
        this._animationPlayerNode.Play(GameConstants.ANIMATION_DASH);
    }
}
