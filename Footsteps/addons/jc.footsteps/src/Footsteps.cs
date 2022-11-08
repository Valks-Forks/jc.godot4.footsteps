using Godot;
using Godot.Collections;
using System;

namespace JC.Footsteps
{
	[Tool]
	public partial class Footsteps: Node3D
	{
		// Audio player.
		public AudioStreamPlayer3D AudioPlayer{ get; set; } = null;

		// Default audio clips.
		public FootstepsSurfaceAudio DefaultClips{ get; set; } = null;

		// Surfaces.
		const String kDefaultSurfaceMetaID = "surface";

		String _SurfaceMetaID = kDefaultSurfaceMetaID;

		/// <summary> Surface ID for Metadata. </summary>
		public String SurfaceMetaID
		{
			get => _SurfaceMetaID;
			set
			{
				if(value == "")
					_SurfaceMetaID = kDefaultSurfaceMetaID;
				else
					_SurfaceMetaID = value;
			}
		}

		[ExportGroup("Audio")]
		[ExportSubgroup("Surface")]
		[Export] public Array<FootstepsSurface> surfaces;

		// Surface interval.
		[ExportGroup("Step")]

		/// <summary> Interval between each step. </summary>
		[Export] public float StepInterval{ get; set; } = 2.0f;

		// FX.
		// Audio Bus.
		/// <summary> Index of the audio bus. </summary>
		public uint BusIndex{ get; set; } = 1;

		// Enable pan fx.
		bool _EnablePan = false;

		/// <summary> Enable pan FX </summary>
		public bool EnablePan
		{
			get => _EnablePan;
			set
			{
				_EnablePan = value;
				NotifyPropertyListChanged();
			}
		}

		// Pan FX index.
		/// <summary> Index of the pan fx. </summary>
		public uint PanIndex{ get; set; } = 0;

		//
		bool _EnablePitch = false;

		/// <summary> Enable pitch fx. </summary>
		public bool EnablePitch
		{
			get => _EnablePitch;
			set
			{
				_EnablePitch = value;
				NotifyPropertyListChanged();
			}
		}

		/// <summary> Index of the pitch fx. </summary>
		public uint PitchIndex{ get; set; } = 1;

		// Step.
		bool _OnAirStore = false;
		float _Step;
		float _DistanceTravelled;

		// Current clips.
		AudioStream _CurrentLandingClip = null;
		Texture _CurrentSurfaceTexture = null;

		// Nodes.
		CharacterBody3D _Character = null;
		RandomNumberGenerator _Random = new RandomNumberGenerator();

		//
		bool _DefaultSurface = true;
		float _PhysicsDelta;
		float _PanRange;
		float _MinUnitSize,   _MaxUnitSize;
		float _MinPitchRange, _MaxPitchRange;
		bool _EnablePanner = false;

		public override void _EnterTree()
		{
			_Character = GetParent() as CharacterBody3D;
		}

		public override void _Ready()
		{
			if(Engine.IsEditorHint())
				return;
			
			_EnablePanner = false;
			Play();
		}

		public override void _PhysicsProcess(double delta)
		{
			if(Engine.IsEditorHint())
				return;
			
			_PhysicsDelta = (float)delta;
			if(_Character == null)
				return;
			
			if(!_Character.IsOnFloor())
				_OnAirStore = true;
			
			if(_Character.IsOnFloor() && _OnAirStore)
			{
				OnLanding();
				_OnAirStore = false;
			}

			// Step.
			if(_Character.IsOnFloor())
			{
				_DistanceTravelled += _Character.Velocity.Length() * (float)delta;
				if(_DistanceTravelled > _Step)
				{
					_EnablePanner = true;
					Play();
					_DistanceTravelled = 0.0f;
					_Step = StepInterval;
				}
			}
		}

		private Texture GetGroundTexture()
		{
			KinematicCollision3D slideCollision = null;
			for(int i = 0; i < _Character.GetSlideCollisionCount(); i++)
			{
				slideCollision = _Character.GetSlideCollision(i);
			}

			if(slideCollision == null)
				return null;

			for(int i = 0; i < slideCollision.GetCollisionCount(); i++)
			{
				var collider = slideCollision.GetCollider(i);
				if(collider is PhysicsBody3D) 
				{
					if(collider.HasMeta(SurfaceMetaID))
					{
						Texture meta = (Texture)collider.GetMeta(SurfaceMetaID);
						if(meta != null)
							return meta;
					}
				}
			}
			return null;
		}

		private void Play()
		{
			if(AudioPlayer == null)
				return;
			
			_CurrentSurfaceTexture = GetGroundTexture();
			if(_CurrentSurfaceTexture != null)
			{
				PlaySurfaceClips();
			}
			else
			{
				_DefaultSurface = true;
				PlayDefaultClips();
			}
		}

		private void OnLanding()
		{
			if(AudioPlayer == null)
				return;
			
			_EnablePanner = false;
			Play();
			PlayAudioPlayer(_CurrentLandingClip);
		}

		private void PlayAudioPlayer(AudioStream clip)
		{
			float unit = _Random.RandfRange(_MinUnitSize, _MaxUnitSize);
			AudioPlayer.UnitSize = unit;
			AudioPlayer.Stream = clip;

			// Panner.
			if(EnablePan)
			{
				AudioEffectPanner fx = (AudioEffectPanner)AudioServer.GetBusEffect((int)BusIndex, (int)PanIndex);
				if(fx != null)
				{
					if(_EnablePanner)
					{
						fx.Pan = _PanRange - fx.Pan - _PanRange;
						if(fx.Pan == 0.0f)
						{
							fx.Pan = _PanRange;
						}
					}
					else
						fx.Pan = 0.0f;
				}
			}

			// Pitch.
			if(EnablePitch)
			{
				AudioEffectPitchShift fx = (AudioEffectPitchShift)AudioServer.GetBusEffect((int)BusIndex, (int)PitchIndex);
				if(fx != null)
				{
					fx.PitchScale = _Random.RandfRange(_MinPitchRange, _MaxPitchRange);
				}
			}
			AudioPlayer.Play();
		}

		private void PlaySurfaceClips()
		{
			if(surfaces.Count > 0)
			{
				foreach(var surface in surfaces)
				{
					if(surface.Exists(_CurrentSurfaceTexture))
					{
						_DefaultSurface = false;
						_MinUnitSize = surface.MinUnitSize;
						_MaxUnitSize = surface.MaxUnitSize;
						_PanRange = surface.PanRange;
						_MinPitchRange = surface.MinPitchRange;
						_MaxPitchRange = surface.MaxPitchRange;

						if(surface.landingClips.Count > 0)
							_CurrentLandingClip = surface.landingClips[
								_Random.RandiRange(0, surface.landingClips.Count - 1)
							];

						PlayAudioPlayer(
							surface.clips[_Random.RandiRange(0, surface.clips.Count - 1)
						]);
					}
				}
			}
			else
			{
				_DefaultSurface = true;
				PlayDefaultClips();

			}
		}

		private void PlayDefaultClips()
		{
			if(DefaultClips == null)
				return;
			
			_MinUnitSize = DefaultClips.MinUnitSize;
			_MaxUnitSize = DefaultClips.MaxUnitSize;
			_PanRange = DefaultClips.PanRange;
			_MinPitchRange = DefaultClips.MinPitchRange;
			_MaxPitchRange = DefaultClips.MaxPitchRange;
			
			if(DefaultClips.landingClips.Count > 0)
				_CurrentLandingClip = DefaultClips.landingClips[
					_Random.RandiRange(0, DefaultClips.landingClips.Count - 1)
				];

			PlayAudioPlayer(
				DefaultClips.clips[_Random.RandiRange(0, DefaultClips.clips.Count - 1)]
			);
		}
	}
}
