using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class FPCharacter : CharacterBody3D
{
	// Speed.
	[Export]
	public float WalkSpeed{ get; set; } = 5.0f;

	[Export]
	public float SprintSpeed{ get; set; } = 7.0f;

	// Jump.
	[Export]
	public float JumpForce{ get; set; } = 10.0f;
	
	// Rotation.
	[Export]
	public float RotationSpeed{ get; set; } = 12.0f;
	public float YawRotation{ get; set; } = 0.0f;
	
	// Gravity.
	[Export]
	public float GravityModifier{ get; set; } = 2.0f;

	// private global variables.
	private float _InternalJumpForce = 0.0f;
	private float _SpeedMul = 0.0f;
	private Vector3 _Velocity = Vector3.Zero;

	// Direction.
	private Vector3 _InputDir = Vector3.Zero;
	public Vector3 InputDir => _InputDir;

	private Vector3 _MoveDir = Vector3.Zero;
	public Vector3 MoveDir => _MoveDir;

	private Vector3 _OldStrafeMoveDir;
	private Vector3 _StrafeMoveDir;

	// States.
	public bool IsSprinting{ get; private set; } = false;
	public bool IsJumping{ get; private set; } = false;

	private float GravityForce => 9.8f * GravityModifier;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MaxSlides = 6;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		_InputDir.x = Input.GetActionStrength("move_right") - 
			Input.GetActionStrength("move_left");
		
		_InputDir.y = 0.0f;

		_InputDir.z = Input.GetActionStrength("move_back") - 
			Input.GetActionStrength("move_front");

		// Sprint.
		IsSprinting = Input.GetActionStrength("sprint") > 0.0f;
	}

    public override void _PhysicsProcess(double delta)
    {
		// Add gravity.
		if (!IsOnFloor())
			AddGravityForce(GravityForce, (float)delta);

		// Move dir.
		_MoveDir.x = InputDir.x;
		_MoveDir.z = InputDir.z;
		_MoveDir = _MoveDir.Rotated(Vector3.Up, YawRotation).Normalized();
		_MoveDir.y = 0.0f;

		ComputeSpeedMul();

		if(IsOnFloor())
		{
			_Velocity.x = _MoveDir.x * _SpeedMul;
			_Velocity.z = _MoveDir.z * _SpeedMul;
		}

        // Jump.
		IsJumping = IsOnFloor() && Input.IsActionJustPressed("jump");

		if(IsOnFloor())
		{
			if(IsJumping)
				Jump();
		}

		Yaw((float)delta);
		InternalMoveAndSlide(ref _Velocity);
    }

	private void InternalMoveAndSlide(ref Vector3 vel)
	{
		Velocity = vel;
		MoveAndSlide();

		// Prevent Jitter.
		vel = Velocity;
	}

	private void ComputeSpeedMul()
	{
		_SpeedMul = IsSprinting ? SprintSpeed : WalkSpeed;
		_InternalJumpForce = (JumpForce + Velocity.Length()) * 0.7f;
	}

	private void Jump() => _Velocity.y = _InternalJumpForce;

	private void AddGravityForce(float force, float delta) => 
		_Velocity.y -= force * delta;

	private void Yaw(float delta)
	{
		Vector3 smd = Vector3.Forward.Rotated(Vector3.Up, YawRotation).Normalized(); 
		_OldStrafeMoveDir = _StrafeMoveDir;

		Vector2 look;
		look.x = smd.z;
		look.y = smd.x;

		_StrafeMoveDir = Lerp(_StrafeMoveDir, smd, delta * 45.0f); 
		Basis = Basis.FromEuler(Vector3.Up * look.Angle());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Vector3 Lerp(Vector3 from, Vector3 to, float t) => 
		(1.0f - t) * from + t * to;
}
