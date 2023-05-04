using Godot;
using System;

public partial class FPCamera: Node3D
{
	[Export]
	public float MouseSensitive{ get; set; } = 0.005f;

	[Export]
	private Vector2 _XClamp = new(-65.0f, 80.0f);
	public Vector2 XClamp
	{
		get => _XClamp; 
		set => _XClamp = value; 
	}

	[Export]
	public Node3D Target{ get; set; } = null;

	[Export]
	private Camera3D _Camera{get; set; } = null;
	private FPCharacter _Player = null;

	private Transform3D _PrevTr = new();
	private Transform3D _CurrentTr = new();
	private bool _IsFixedUpdate = false;

	public override void _Notification(long what)
	{
		if(what == NotificationParented)
		{
			_Player = GetParent() as FPCharacter;
		}
	}

	public override void _Ready()
	{
		TopLevel = true;
		Target = Target; // Did you mean to do this.Target = Target?

		if(_Camera == null)
		{
			GD.PushWarning("Camera Not Found");
		}
		else
		{
			GlobalPosition = Target.Position;
		}
		_PrevTr = Target.GlobalTransform;
		_CurrentTr = Target.GlobalTransform;

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);
		if(@event is InputEventMouseMotion)
		{
			InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;

			Vector3 rot = Rotation;
			rot.x -= mouseMotion.Relative.y * MouseSensitive;
			rot.x = Mathf.Clamp(rot.x, Mathf.DegToRad(XClamp.x), Mathf.DegToRad(XClamp.y));

			rot.y -= mouseMotion.Relative.x * MouseSensitive;
			rot.y = Mathf.Wrap(rot.y, 0.0f, Mathf.DegToRad(360.0f));
			
			Rotation = rot;
		}
	}

	public override void _Process(double delta)
	{
		if(_Player == null)
			return;
		
		_Player.YawRotation = this.Rotation.y;
		if(Target == null)
			return;
		
		if(_IsFixedUpdate)
		{
			_PrevTr = _CurrentTr;
			_CurrentTr = Target.GlobalTransform;
			_IsFixedUpdate = false;
		}

		var fraction = Math.Clamp(Engine.GetPhysicsInterpolationFraction(), 0.0f, 1.0f);
		GlobalPosition = _PrevTr.origin.Lerp(_CurrentTr.origin, (float)fraction);
	}

	public override void _PhysicsProcess(double delta)
	{
		_IsFixedUpdate = true;
	}
}
