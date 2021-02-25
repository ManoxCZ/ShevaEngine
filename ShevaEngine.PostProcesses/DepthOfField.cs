using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;


namespace ShevaEngine.PostProcesses
{
    public class DepthOfField : BasePostProcess
    {
        protected override string EffectFilename => nameof(DepthOfField);
        public float FocusDistance { get; set; } = 50;
        public float FocusRange{ get; set; } = 5;

        
        /// <summary>
        /// Prepare effect.
        /// </summary>
        protected override void PrepareEffect(Camera camera, GameTime time, IScene scene)
        {
            Effect.Parameters["DepthTexture"].SetValue(DepthTexture);
            Effect.Parameters["DoFParams"].SetValue(new Vector4(FocusDistance, FocusRange, camera.NearPlane, camera.FarPlane / (camera.FarPlane - camera.NearPlane)));
        }
    }
}
