using Godot;
using Godot.Collections;
using PropElement = Godot.Collections.Dictionary;

//using GodotArray = Godot.Collections.Array<Godot.Collections.Dictionary<Godot.Variant, Godot.Variant>>;

namespace JC.Footsteps;

public partial class Footsteps : Node3D
{
	public override Array<Godot.Collections.Dictionary> _GetPropertyList()
	{
		Array<PropElement> ret = new();

		PropElement pTitle = new()
		{
			{"name", "Footsteps"},
			{"type", (int)Variant.Type.Nil},
			{"usage", (int)PropertyUsageFlags.Category},
		};
		ret.Add(pTitle);

		PropElement pAudio = new()
		{
			{"name", "Audio"},
			{"type", (int)Variant.Type.Nil},
			{"usage", (int)PropertyUsageFlags.Group}
		};
		ret.Add(pAudio);

		PropElement pPlayer = new()
		{
			{"name", "Player"},
			{"type", (int)Variant.Type.Nil},
			{"usage", (int)PropertyUsageFlags.Subgroup}
		};
		ret.Add(pPlayer);

		PropElement pAudioPlayer = new()
		{
			{"name", "AudioPlayer"},
			{"type",(int)Variant.Type.Object},
			{"hint", (int)PropertyHint.NodeType}
		};

		ret.Add(pAudioPlayer);

		PropElement pSurface = new()
		{
			{"name", "Surface"},
			{"type", (int)Variant.Type.Nil},
			{"usage", (int)PropertyUsageFlags.Subgroup}
		};
		ret.Add(pSurface);

		PropElement pSurfaceMetaID = new()
		{
			{"name", "SurfaceMetaID"},
			{"type", (int)Variant.Type.StringName}
		};
		ret.Add(pSurfaceMetaID);

		PropElement pDefaultClips = new()
		{
			{"name", "DefaultClips"},
			{"type", (int)Variant.Type.Object},
			{"hint", (int)PropertyHint.ResourceType}
			//{"hint_string", "FootstepsSurfaceAudio"}
		};
		ret.Add(pDefaultClips);

		PropElement pFX = new()
		{
			{"name", "FX"},
			{"type", (int)Variant.Type.Nil},
			{"usage", (int)PropertyUsageFlags.Subgroup}
		};
		ret.Add(pFX);

		PropElement pBusIndex = new()
		{
			{"name", "BusIndex"},
			{"type", (int)Variant.Type.Int}
		};
		ret.Add(pBusIndex);

		PropElement pEnablePan = new()
		{
			{"name", "EnablePan"},
			{"type", (int)Variant.Type.Bool}
		};
		ret.Add(pEnablePan);

		if (EnablePan)
		{
			PropElement pPanIndex = new()
			{
				{"name", "PanIndex"},
				{"type", (int)Variant.Type.Int}
			};
			ret.Add(pPanIndex);
		}

		PropElement pEnablePitch = new()
		{
			{"name", "EnablePitch"},
			{"type", (int)Variant.Type.Bool}
		};
		ret.Add(pEnablePitch);

		if (EnablePitch)
		{
			PropElement pPitchIndex = new()
			{
				{"name", "PitchIndex"},
				{"type", (int)Variant.Type.Int}
			};
			ret.Add(pPitchIndex);
		}

		return ret;
	}
}
