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
    private Area3D _chaseArea;
    private bool _isChasing = false;
    private Timer _chaseTimer;
    private float _chaseSpeed = 3.0f;
    private CharacterBody3D _enemyPlayerTarget { get; set; }
    private Area3D _attackArea;
    private bool _isAttacking = false;

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
        
        this._timer = GetNode<Timer>("PathPauseTimer");
        this._timer.OneShot = true;
        this._timer.Autostart = false;
        this._timer.Timeout += OnTimeout;

        this._chaseArea = GetNode<Area3D>("ChaseArea");
        this._enemyPlayerTarget = null;
        this._chaseArea.BodyEntered += OnChaseBodyEntered;
        this._chaseArea.BodyExited += OnChaseBodyExited;

        this._chaseTimer = GetNode<Timer>("ChaseTimer");
        this._chaseTimer.OneShot = false;
        this._chaseTimer.Autostart = false;
        this._chaseTimer.WaitTime = 0.5f;
        this._chaseTimer.Timeout += OnChaseTimeout;

        this._attackArea = GetNode<Area3D>("AttackArea");
        this._attackArea.BodyEntered += OnAttackBodyEntered;
        this._attackArea.BodyExited += OnAttackBodyExited;
              
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
            if (this._isAttacking)
            {
                this._stateMachine.ChangeState<EnemyAttackState>();
            }
            else 
            {
                this._stateMachine.ChangeState<EnemyMoveState>();
            }
            
            if (!this._isChasing && this._enemyPlayerTarget == null)
            {
                this.MoveToOrigin();
            }
            else
            {
                this.MoveToEnemyPlayerTarget();
            }
        }
    }

    private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        this._movementTarget = this._path.Curve.GetPointPosition(this._pathIndex);
    }

    private void MoveToEnemyPlayerTarget()
    {
        this._characterNode.Velocity = this._characterNode
            .GlobalPosition.DirectionTo(this._enemyPlayerTarget.GlobalPosition) * this._chaseSpeed;
        this._characterNode.MoveAndSlide();
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

    private void OnChaseBodyEntered(Node3D body)
    {
        this._isChasing = true;
        this._enemyPlayerTarget = (CharacterBody3D)body;
        this._chaseTimer.Start();
        if (!this._timer.IsStopped())
        {
            this._timer.Stop();
        }
    }

    private void OnChaseBodyExited(Node3D body)
    {
        this._chaseTimer.Stop();
        this._isChasing = false;
        this._enemyPlayerTarget = null;
    }

    private void OnChaseTimeout()
    {
        this._spriteNode.FlipH = this._enemyPlayerTarget.GlobalPosition.X < this.GlobalPosition.X;
        this._movementTarget = this._enemyPlayerTarget.GlobalPosition;
    }

    private void OnAttackBodyExited(Node3D body)
    {
        this._isAttacking = false;
    }

    private void OnAttackBodyEntered(Node3D body)
    {
        this._isAttacking = true;
    }
}
