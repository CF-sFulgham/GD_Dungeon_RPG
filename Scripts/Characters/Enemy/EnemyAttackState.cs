using Godot;
using System;

public partial class EnemyAttackState : CharacterBaseState
{
    protected override void PlayAnimation()
    {
        this._animationPlayerNode.Play(GameConstants.ANIMATION_ATTACK);
    }
}
