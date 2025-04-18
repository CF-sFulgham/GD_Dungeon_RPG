using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody3D
{
    private Sprite3D _spriteNode;
    private Vector2 _direction = Vector2.Zero;
    private float _speed = 5.0f;
    private StateMachine _stateMachine;
    private bool _isDashing = false;
    private double _dashInterval = 0.25f;
    private int _dashCount = 0;
    private Stopwatch _stopWatch = new Stopwatch();
    private bool _isTimerRunning = false;
    private Timer _dashTimer { get; set; }

    public override void _Ready()
    {
        this._spriteNode = GetNode<Sprite3D>("Sprite3D");
        this._stateMachine = GetParent().GetNode<StateMachine>("StateMachine");
        this._stateMachine.ChangeState<PlayerIdleState>();
    }

    public override void _Process(double delta)
    {
        if(this._stopWatch.Elapsed.TotalSeconds > this._dashInterval)
        {
            this.StopTimer();
            this._dashCount = 0;
            // GD.Print("Restarted");
            return;
        }

        if (Input.IsActionJustReleased(GameConstants.INPUT_RIGHT) ||
            Input.IsActionJustReleased(GameConstants.INPUT_LEFT))
        {
            if (!this._isTimerRunning) this.StartTimer();
            this.Dash();
        }
    }

    public void StartTimer()
    {
        this._isTimerRunning = true;
        this._stopWatch.Start();
        this._dashCount++;
        // GD.Print("Started Watch");
    }

    public void StopTimer()
    {
        this._dashCount = 0;
        this._isTimerRunning = false;
        this._stopWatch.Stop();
        this._stopWatch.Reset();
        // GD.Print("Stopped Watch");
    }

    public override void _PhysicsProcess(double delta)
    {
        if(this._isDashing)
        {
            this._speed = 10.0f;
            if (Velocity == Vector3.Zero)
            {
                Velocity = this._spriteNode.FlipH ?
                    new Vector3(-1, 0, this._direction.Y) * this._speed :
                    new Vector3(1, 0, this._direction.Y) * this._speed;
            }
        }
        else
        {
            this._speed = 5.0f;
            Velocity = new Vector3(this._direction.X, 0, this._direction.Y) * this._speed;
        }

        MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if(this._isDashing)
        {
            return;
        }

        this._direction = Input.GetVector(
            GameConstants.INPUT_LEFT, 
            GameConstants.INPUT_RIGHT, 
            GameConstants.INPUT_UP,
            GameConstants.INPUT_DOWN
        );

        if (this._direction != Vector2.Zero)
        {
            this._spriteNode.FlipH = this._direction.X < 0;
            this.Move();
        }
        else
        {
            this.Idle();
        }
    }

    public void Move()
    {
        this._stateMachine.ChangeState<PlayerMoveState>();
    }

    public void Idle()
    {
        this._stateMachine.ChangeState<PlayerIdleState>();
    }

    public void Dash()
    {
        if (this._dashCount == 2)
        {
            // GD.Print("Elapsed: ", this._stopWatch.Elapsed.TotalSeconds);
            if (!this._isDashing && this._stopWatch.Elapsed.TotalSeconds <= this._dashInterval)
            {
                this._isDashing = true;
                // GD.Print("Dashing");
                this._stateMachine.ChangeState<PlayerDashState>();
                this._dashTimer = new Timer();
                this._dashTimer.WaitTime = 0.4f;
                this._dashTimer.OneShot = true;
                this._dashTimer.Timeout += () =>
                {
                    this._isDashing = false;
                    this._stateMachine.ChangeState<PlayerIdleState>();
                    this._dashTimer.Stop();
                    this._dashTimer.QueueFree();
                    this._dashTimer = null;
                    // GD.Print("Dash Timeout");
                };
                AddChild(this._dashTimer);
                this._dashTimer.Start();
            }
           
            this.StopTimer();
            return;
        }

        this._dashCount++;
        // GD.Print("Run Tap Count: ", this._dashCount);
    }
}
