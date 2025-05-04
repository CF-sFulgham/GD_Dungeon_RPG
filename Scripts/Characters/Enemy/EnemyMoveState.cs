using Godot;
using System;

public partial class EnemyMoveState : CharacterBaseState
{
    protected override void PlayAnimation()
    {
        this._animationPlayerNode.Play(GameConstants.ANIMATION_MOVE);
    }
}
