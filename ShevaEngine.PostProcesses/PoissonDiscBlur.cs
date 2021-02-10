﻿// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;

// namespace ShevaEngine.PostProcesses
// {
//     public class PoissonDiscBlur : BasePostProcess
//     {
//         private Vector2[] taps;

//         protected float _DiscRadius = 15;
//         public float DiscRadius
//         {
//             get { return _DiscRadius; }
//             set { _DiscRadius = value; }
//         }

//         public PoissonDiscBlur(Game game) : base(game)
//         {
//             taps = new Vector2[]{
//                     new Vector2(-0.326212f,-0.40581f),new Vector2(-0.840144f,-0.07358f),
//                     new Vector2(-0.695914f,0.457137f),new Vector2(-0.203345f,0.620716f),
//                     new Vector2(0.96234f,-0.194983f),new Vector2(0.473434f,-0.480026f),
//                     new Vector2(0.519456f,0.767022f),new Vector2(0.185461f,-0.893124f),
//                     new Vector2(0.507431f,0.064425f),new Vector2(0.89642f,0.412458f),
//                     new Vector2(-0.32194f,-0.932615f),new Vector2(-0.791559f,-0.59771f)};
//         }

//         public override void Draw(GameTime gameTime)
//         {
//             if (effect == null)
//                 effect = Game.Content.Load<Effect>("Shaders/PostProcessing/PoissonDiscBlur");

//             effect.Parameters["Taps"].SetValue(taps);
//             effect.Parameters["DiscRadius"].SetValue(_DiscRadius);
//             effect.Parameters["TexelSize"].SetValue(-new Vector2(.5f / (float)Game.GraphicsDevice.Viewport.Width, .5f / (float)Game.GraphicsDevice.Viewport.Height));

//             // Set Params.
//             base.Draw(gameTime);
//         }
//     }
// }
