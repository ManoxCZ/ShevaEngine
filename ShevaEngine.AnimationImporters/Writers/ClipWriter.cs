using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;


namespace AnimationImporters
{
	[ContentTypeWriter]
    class ClipWriter : ContentTypeWriter<ClipContent>
    {
		/// <summary>
		/// Write.
		/// </summary>
		protected override void Write(ContentWriter output, ClipContent value)
        {
			output.Write(value.Duration);					
            output.Write(value.Keyframes.Length);
			output.Write(value.Keyframes[0].Length);

			foreach (Matrix[] matrices in value.Keyframes)
			{ 
				for (int i = 0; i < matrices.Length; i++)
					output.Write(matrices[i]);
            }            
        }

		/// <summary>
		/// Get runtime type.
		/// </summary>
		public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
			switch (targetPlatform)
			{
				case TargetPlatform.Windows:
				case TargetPlatform.WindowsStoreApp:
				case TargetPlatform.Android:
				case TargetPlatform.DesktopGL:
					return "ShevaEngine.Core.Clip, ArcticSurvivor";
			}

			throw new NotImplementedException("Can't find Runtime type");			
        }

		/// <summary>
		/// Get runtime header.
		/// </summary>
		public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
			switch (targetPlatform)
			{
				case TargetPlatform.Windows:
				case TargetPlatform.WindowsStoreApp:
				case TargetPlatform.Android:
				case TargetPlatform.DesktopGL:
					return "ShevaEngine.Core.ClipReader, ArcticSurvivor";
			}

			throw new NotImplementedException("Can't find Runtime type");			
        }
    }    
}
