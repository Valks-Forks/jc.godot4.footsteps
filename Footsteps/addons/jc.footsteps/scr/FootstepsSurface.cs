using Godot;
using Godot.Collections;
using System;

namespace JC.Footsteps
{
	[Tool]
	public partial class FootstepsSurface: FootstepsSurfaceAudio
	{
		[ExportGroup("Surface List")]
		
		[Export]
		public Array<Texture> surfaces; 

		public bool Exist(Texture texture)
		{
			if(texture == null)
				return false;
			
			if(surfaces.Contains(texture))
				return true;
			
			return false;
		}
	}
}
