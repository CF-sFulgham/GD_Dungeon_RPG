using Godot;
using System;

public partial class StateMachine : Node
{
    private Node _currentState;
    private Node[] _states;
    private Node _player;
    private Node _enemy;

    public override void _Ready()
    {
        this._player = GetParent().GetNode<Player>("Player");
        this._enemy = GetParent().GetNode<Node>("Enemies")
            .GetNode<Enemy>("Enemy");

        // Initialize the state machine with the available states
        this._states = new Node[]
        {
            this._player.GetNode<Node>("PlayerIdleState"),
            this._player.GetNode<Node>("PlayerMoveState"),
            this._player.GetNode<Node>("PlayerDashState"),
            this._enemy.GetNode<Node>("EnemyIdleState"),
            this._enemy.GetNode<Node>("EnemyMoveState"),
            this._enemy.GetNode<Node>("EnemyAttackState"),
        };
    }

    public void ChangeState<T>()
        where T : Node
    {
        Node newState = null;

        // Check if the new state is the same as the current state
        if (this._currentState is T)
        {
            // GD.Print("Already in the requested state.");
            return;
        }

        // Stop the current state
        // this._currentState.Call("StopAnimation");

        // Find the new state
        foreach (var state in this._states)
        {
            if (state is T)
            {
                newState = state;
                break;
            }
        }

        // If the new state was not found, return
        if (newState == null)
        {
            // GD.Print("Requested state not found.");
            return;
        }

        this._currentState = newState;

        // Start the new state
        this._currentState.Call("PlayAnimation");
    }

}