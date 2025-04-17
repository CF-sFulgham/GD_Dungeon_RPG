using Godot;

public interface IPlayerStates
{
    AnimationPlayer _animationPlayerNode { get; set; }
    void PlayAnimation();
}