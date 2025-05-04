using System;
using Godot;

public partial class Enemy : Character
{
    private Path3D _path;
    private int _pathIndex { get; set; } = 0;
    private NavigationAgent3D _navigationAgent;
    private Vector3 _movementTarget
    {
        get { return this._navigationAgent.TargetPosition; }
        set { this._navigationAgent.TargetPosition = value; }
    }
    private Timer _timer;
    private float _maxIdleTime = 4.0f;

    public override void _Ready()
    {
        base._Ready();
        this._stateMachine.ChangeState<EnemyIdleState>();
        this._path = GetNode<Path3D>("Path3D");

        for (int i = 0; i < this._path.Curve.PointCount; i++)
        {
            Vector3 point = this._path.Curve.GetPointPosition(i) + this._path.GlobalPosition;
            this._path.Curve.SetPointPosition(i, point);
        }

        this._navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        this._navigationAgent.PathDesiredDistance = 1f;
        this._navigationAgent.TargetDesiredDistance = 1f;
        
        this._timer = GetNode<Timer>("Timer");
        this._timer.OneShot = true;
        this._timer.Autostart = false;
        this._timer.Timeout += OnTimeout;

        Callable.From(ActorSetup).CallDeferred();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!this._timer.IsStopped())
        {
            return;
        }

        if (this._path != null)
        {
            this._stateMachine.ChangeState<EnemyMoveState>();
            this.MoveToOrigin();
        }
    }

    private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        this._movementTarget = this._path.Curve.GetPointPosition(this._pathIndex);
    }

    private void MoveToOrigin()
    {
        if (this._navigationAgent.IsTargetReached())
        {
            this.OnPathTargetReached();
            return;
        }
        this._characterNode.Velocity = this._characterNode
            .GlobalPosition.DirectionTo(this._navigationAgent.GetNextPathPosition());
        
        this._characterNode.MoveAndSlide();
    }

    private void OnPathTargetReached()
    {
        this._stateMachine.ChangeState<EnemyIdleState>();

        RandomNumberGenerator rng = new RandomNumberGenerator();
        this._timer.WaitTime = rng.RandfRange(0f, this._maxIdleTime);

        this._pathIndex = Mathf.Wrap(
            this._pathIndex + 1, 
            0, 
            this._path.Curve.PointCount
        );

        this._timer.Start();
    }

    private void OnTimeout()
    {
        this._timer.Stop();
        this._movementTarget = this._path.Curve.GetPointPosition(this._pathIndex);
        this._stateMachine.ChangeState<EnemyMoveState>();
    }
}
