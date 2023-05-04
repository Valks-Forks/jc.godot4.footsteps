using Godot;
using Godot.Collections;
using System;

namespace JC.Footsteps;

[Tool]
public partial class FootstepsSurfaceAudio : Resource
{

	[ExportGroup("Clip Landing")]
	[Export] public Array<AudioStream> landingClips;

	[ExportGroup("Clips List")]
	[Export] public Array<AudioStream> clips;
	
	[ExportGroup("Units")]
	float _MinUnitSize = 0.30f;
	[Export] public float MinUnitSize
	{
		get => _MinUnitSize;
		set
		{
			_MinUnitSize = Mathf.Clamp(value, 0.0f, value);
		}
	}

	float _MaxUnitSize = 0.60f;
	[Export] public float MaxUnitSize
	{
		get => _MaxUnitSize;
		set
		{
			_MaxUnitSize = Mathf.Clamp(value, 0.0f, value);
		}
	}

	[ExportGroup("FX")]
	[ExportSubgroup("Panner")]
	float _PanRange = 0.1f;
	[Export] public float PanRange
	{
		get => _PanRange;
		set
		{
			_PanRange = Mathf.Clamp(value, 0.0f, 1.0f);
		}
	}

	[ExportSubgroup("Pitch")]
	float _MinPitchRange = 0.9f;
	[Export]
	public float MinPitchRange
	{
		get => _MinPitchRange;
		set
		{
			_MinPitchRange = Mathf.Clamp(value, 0.01f, value);
		}
	}

	[Export]
	public float MaxPitchRange{ get; set; } = 1.1f;
}
	
