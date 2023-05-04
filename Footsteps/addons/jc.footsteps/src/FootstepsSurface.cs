using Godot;
using Godot.Collections;
using System;

namespace JC.Footsteps;

[Tool]
public partial class FootstepsSurface: FootstepsSurfaceAudio
{
	[ExportGroup("Surface List")]
	[Export]
	public Array<Texture> surfaceTextures; 

	/*public bool Exists(Texture texture)
	{
		if(texture == null)
			return false;
		
		if(surfaceTextures.Contains(texture))
			return true;
		
		return false;
	}*/
}
