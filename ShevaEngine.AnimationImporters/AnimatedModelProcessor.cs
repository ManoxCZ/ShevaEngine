using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;

namespace AnimationImporters
{
	[ContentProcessor(DisplayName = "Animated Model")]
    public class AnimatedModelProcessor : ModelProcessor
    {        
        [DisplayName("MaxBones")]
        [DefaultValue(SkinnedEffect.MaxBones)]
        public int MaxBones { get; set; } = SkinnedEffect.MaxBones;

		[DisplayName("Generate Keyframes Frequency")]
		[DefaultValue(0)] // (0=no, 30=30fps, 60=60fps)
		public int GenerateKeyframesFrequency { get; set; } = 0;
        
        [DisplayName("Fix BoneRoot from MonoGame importer")]
        [DefaultValue(false)]
        public bool FixRealBoneRoot { get; set; }

		[DefaultValue(MaterialProcessorDefaultEffect.SkinnedEffect)]
		public override MaterialProcessorDefaultEffect DefaultEffect
		{
			get { return base.DefaultEffect; }
			set { base.DefaultEffect = value; }
		}
		

		/// <summary>
		/// Process method.
		/// </summary>
		public override ModelContent Process(NodeContent input, ContentProcessorContext context)
		{
			AnimationsProcessor animationProcessor = new AnimationsProcessor()
			{
				MaxBones = MaxBones,
				GenerateKeyframesFrequency = GenerateKeyframesFrequency,
				FixRealBoneRoot = FixRealBoneRoot,
			};			

			ModelContent model = base.Process(input, context);

			model.Tag = animationProcessor.Process(input, context); ;

			return model;
        }
    }
}
