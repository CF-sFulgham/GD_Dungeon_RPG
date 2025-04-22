using Godot;
using System;

public partial class EnemyIdleState : CharacterBaseState
{
    protected override void PlayAnimation()
    {
        this._animationPlayerNode.Play(GameConstants.ANIMATION_IDLE);
    }

}
