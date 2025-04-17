using Godot;
using System;

public partial class StateMachine : Node
{
    private Node _currentState;
    private Node[] _states;
    private PlayerIdleState _playerIdleState;

    public override void _Ready()
    {
        // Initialize the state machine with the available states
        this._states = new Node[]
        {
            GetParent().GetNode<Player>("Player").GetNode<Node>("PlayerIdleState"),
            GetParent().GetNode<Player>("Player").GetNode<Node>("PlayerMoveState")
        };

        this._currentState = this._states[0];
        // Play animation for the initial state of the Player
        this._playerIdleState = (PlayerIdleState)this._states[0];
        this._playerIdleState.PlayAnimation();
    }

    public void ChangeState<T>()
        where T : Node
    {
        Node newState = null;

        // Check if the new state is the same as the current state
        if (this._currentState is T)
        {
            GD.Print("Already in the requested state.");
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
            GD.Print("Requested state not found.");
            return;
        }

        this._currentState = newState;

        // Start the new state
        this._currentState.Call("PlayAnimation");
    }

}