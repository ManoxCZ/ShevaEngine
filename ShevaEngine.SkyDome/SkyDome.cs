using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShevaEngine.Core;
using System;
using System.IO;

namespace ShevaEngine.SkyDomes;

public class SkyDome
{
    private const float _skyboxSize = 1000.0f;

    private readonly GraphicsDevice _graphicsDevice;
    public SkyDomeParameters Parameters { get; set; }
    public Vector3 SunPosition { get; set; }
    private Vector4 LightDirection => new(
            (float)(Math.Sin((double)Theta) * Math.Cos(Phi)),
            (float)Math.Cos((double)Theta),
            (float)(Math.Sin((double)Theta) * Math.Sin(Phi)),
            1.0f);    
    public bool SaveDebugTextures { get; set; } = false;

    public Model SkyDomeModel { get; private set; }
    private SkyDomeMaterial _skyDomeMaterial;
    private RenderTarget2D _mieRenderTarget;
    private RenderTarget2D _rayleighRenderTarget;
    //Texture moonTex, glowTex, starsTex;
    //Texture permTex, gradTex;

    private QuadRenderComponent _quadRenderer;
    private Effect _updateEffect;
    //private Effect texturedEffect;
    //private Effect noiseEffect;

    public float Theta { get; set; }
    public float Phi { get; set; }
    private float _previousTheta, _previousPhi;
    public bool RealTime { get; set; }

    public Color SunColor { get; private set; }
    public float InverseCloudVelocity { get; set; }
    public float CloudCover { get; set; }
    public float CloudSharpness { get; set; }
    public float NumTiles { get; set; }


    public SkyDome()
        : base()
    {
        _graphicsDevice = ShevaGame.Instance.GraphicsDevice;

        RealTime = false;
        Parameters = new();

        Theta = 0.0f;
        Phi = 0.0f;

        _quadRenderer = new();

        SunPosition = new Vector3(
            LightDirection.X * _skyboxSize,
            LightDirection.Y * _skyboxSize,
            LightDirection.Z * _skyboxSize);

        //GeneratePermTex(_graphicsDevice);

        _mieRenderTarget = new(_graphicsDevice,
            128,
            64,
            true,
            SurfaceFormat.HalfVector4,
            DepthFormat.Depth24Stencil8);

        _rayleighRenderTarget = new(_graphicsDevice,
            128,
            64,
            true,
            SurfaceFormat.HalfVector4,
            DepthFormat.Depth24Stencil8);

        // Clouds constantes
        InverseCloudVelocity = 16.0f;
        CloudCover = -0.1f;
        CloudSharpness = 0.5f;
        NumTiles = 16.0f;

        // scatterEffect = game.Content.Load<Effect>("Content/Effects/scatter");
        // texturedEffect = game.Content.Load<Effect>("Content/Effects/Textured");
        // noiseEffect = game.Content.Load<Effect>("Content/Effects/SNoise");

        //moonTex = ShevaGame.Instance.Content.Load<Texture2D>("moon");
        //glowTex = ShevaGame.Instance.Content.Load<Texture2D>("moonglow");
        //starsTex = ShevaGame.Instance.Content.Load<Texture2D>("starfield");

        //GenerateMoon();

        _skyDomeMaterial = new()
        {
            MieTexture = _mieRenderTarget,
            RayleighTexture = _rayleighRenderTarget,
        };

        SkyDomeModel = GenerateDomeModel(_skyDomeMaterial);

        _updateEffect = ShevaGame.Instance.Content.Load<Effect>("Content\\SkyDome\\Effects\\ScatterUpdate");

        UpdateMieRayleighTextures();
    }

    public void Update(GameTime _)
    {
        if (_previousTheta != Theta || _previousPhi != Phi)
        {
            UpdateMieRayleighTextures();

            Vector4 lightDirection = LightDirection;
            lightDirection.Normalize();

            SunPosition = new(lightDirection.X * _skyboxSize, lightDirection.Y * _skyboxSize, lightDirection.Z * _skyboxSize);
        }
    }

    ///// <summary>
    ///// Metoda vrati viditelne objekty.
    ///// </summary>
    ///// <param name="rState"></param>
    ///// <param name="visibleObjects"></param>
    //public override void AddInRenderingPipeline(RenderingPipeline pipeline, Camera camera)
    //{
    //    pipeline.AddOpaqueGraphicsObject(DrawCallKey, DrawCall, Matrix.CreateScale(5000));

    //    //this.SunColor = this.GetSunColor(-this.Theta, 2);            

    //    //this.Material.Parameters["v3SunDir"].SetValue(new Vector3(-this.Parameters.LightDirection.X,
    //    //    -this.Parameters.LightDirection.Y, -this.Parameters.LightDirection.Z));
    //    //this.Material.Parameters["NumSamples"].SetValue(this.Parameters.NumSamples);
    //    //this.Material.Parameters["fExposure"].SetValue(this.Parameters.Exposure);            

    //    //if (this.Theta < Math.PI / 2.0f || this.Theta > 3.0f * Math.PI / 2.0f)
    //    //    this.Material.Parameters["starIntensity"].SetValue((float)Math.Abs(
    //    //        Math.Sin(Theta + (float)Math.PI / 2.0f)));
    //    //else
    //    //    this.Material.Parameters["starIntensity"].SetValue(0.0f);            
    //}

    //private void DrawMoon()
    //{
    //    //BlendState temp = game.GraphicsDevice.BlendState;

    //    //game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

    //    //texturedEffect.CurrentTechnique = texturedEffect.Techniques["Textured"];

    //    //texturedEffect.Parameters["World"].SetValue(
    //    //    Matrix.CreateRotationX(this.Theta + (float)Math.PI / 2.0f) *
    //    //    Matrix.CreateRotationY(-this.Phi + (float)Math.PI / 2.0f) *
    //    //    Matrix.CreateTranslation(parameters.LightDirection.X * 15,
    //    //    parameters.LightDirection.Y * 15,
    //    //    parameters.LightDirection.Z * 15) *
    //    //    Matrix.CreateTranslation(camera.Position.X,
    //    //    camera.Position.Y,
    //    //    camera.Position.Z));
    //    //texturedEffect.Parameters["View"].SetValue(camera.View);
    //    //texturedEffect.Parameters["Projection"].SetValue(camera.Projection);
    //    //texturedEffect.Parameters["Texture"].SetValue(this.moonTex);
    //    //if (fTheta < Math.PI / 2.0f || fTheta > 3.0f * Math.PI / 2.0f)
    //    //    texturedEffect.Parameters["alpha"].SetValue((float)Math.Abs(
    //    //        Math.Sin(Theta + (float)Math.PI / 2.0f)));
    //    //else
    //    //    texturedEffect.Parameters["alpha"].SetValue(0.0f);
    //    //foreach (EffectPass pass in texturedEffect.CurrentTechnique.Passes)
    //    //{
    //    //    pass.Apply();

    //    //    game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
    //    //        (PrimitiveType.TriangleList, quadVerts, 0, 4, quadIb, 0, 2);
    //    //}

    //    //game.GraphicsDevice.BlendState = temp;

    //}



    //private void DrawGlow(Core.Modules.Graphics.RendererState rState)
    //{
    //    //BlendState temp = ShevaEngine.Core.ShevaEngine.Instance.GraphicsDevice.BlendState;

    //    //ShevaEngine.Core.ShevaEngine.Instance.GraphicsDevice.BlendState = BlendState.AlphaBlend;

    //    //texturedEffect.CurrentTechnique = texturedEffect.Techniques["Textured"];

    //    //texturedEffect.Parameters["World"].SetValue(
    //    //    Matrix.CreateRotationX(this.Theta + (float)Math.PI / 2.0f) *
    //    //    Matrix.CreateRotationY(-this.Phi + (float)Math.PI / 2.0f) *
    //    //    Matrix.CreateTranslation(this.Parameters.LightDirection.X * 5,
    //    //    this.Parameters.LightDirection.Y * 5,
    //    //    this.Parameters.LightDirection.Z * 5) *
    //    //    Matrix.CreateTranslation(rState.Camera.ViewMatrix.Translation));
    //    //texturedEffect.Parameters["View"].SetValue(rState.Camera.ViewMatrix);
    //    //texturedEffect.Parameters["Projection"].SetValue(rState.Camera.ProjectionMatrix);
    //    //texturedEffect.Parameters["Texture"].SetValue(this.glowTex);
    //    //if (this.Theta < Math.PI / 2.0f || this.Theta > 3.0f * Math.PI / 2.0f)
    //    //    texturedEffect.Parameters["alpha"].SetValue((float)Math.Abs(
    //    //        Math.Sin(Theta + (float)Math.PI / 2.0f)));
    //    //else
    //    //    texturedEffect.Parameters["alpha"].SetValue(0.0f);
    //    //foreach (EffectPass pass in texturedEffect.CurrentTechnique.Passes)
    //    //{
    //    //    pass.Apply();

    //    //    ShevaEngine.Core.ShevaEngine.Instance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
    //    //        (PrimitiveType.TriangleList, quadVerts, 0, 4, quadIb, 0, 2);
    //    //}

    //    //ShevaEngine.Core.ShevaEngine.Instance.GraphicsDevice.BlendState = temp;
    //}



    //private void DrawClouds(GameTime gameTime)
    //{
    //    //BlendState temp = game.GraphicsDevice.BlendState;

    //    //game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

    //    //noiseEffect.CurrentTechnique = noiseEffect.Techniques["Noise"];

    //    //noiseEffect.Parameters["World"].SetValue(Matrix.CreateScale(10000.0f) *
    //    //    Matrix.CreateTranslation(new Vector3(0, 0, -900)) *
    //    //    Matrix.CreateRotationX((float)Math.PI / 2.0f) *
    //    //    Matrix.CreateTranslation(camera.Position.X,
    //    //    camera.Position.Y,
    //    //    camera.Position.Z)
    //    //    );
    //    //noiseEffect.Parameters["View"].SetValue(camera.View);
    //    //noiseEffect.Parameters["Projection"].SetValue(camera.Projection);
    //    //noiseEffect.Parameters["permTexture"].SetValue(this.permTex);
    //    //noiseEffect.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds / inverseCloudVelocity);
    //    //noiseEffect.Parameters["SunColor"].SetValue(this.sunColor);
    //    //noiseEffect.Parameters["numTiles"].SetValue(numTiles);
    //    //noiseEffect.Parameters["CloudCover"].SetValue(cloudCover);
    //    //noiseEffect.Parameters["CloudSharpness"].SetValue(cloudSharpness);

    //    //foreach (EffectPass pass in noiseEffect.CurrentTechnique.Passes)
    //    //{
    //    //    pass.Apply();

    //    //    game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
    //    //        (PrimitiveType.TriangleList, quadVerts, 0, 4, quadIb, 0, 2);
    //    //}

    //    //game.GraphicsDevice.BlendState = temp;

    //}

    

    private void UpdateMieRayleighTextures()
    {
        _graphicsDevice.SetRenderTargets([new RenderTargetBinding(_rayleighRenderTarget), new RenderTargetBinding(_mieRenderTarget)]);
        _graphicsDevice.Clear(Color.CornflowerBlue);
        _graphicsDevice.BlendState = BlendState.Opaque;

        _updateEffect.Parameters["InvWavelength"].SetValue(Parameters.InvWaveLengths);
        _updateEffect.Parameters["WavelengthMie"].SetValue(Parameters.WaveLengthsMie);
        _updateEffect.Parameters["v3SunDir"].SetValue(-LightDirection);

        _updateEffect.CurrentTechnique.Passes[0].Apply();

        _quadRenderer.Render(_graphicsDevice, Vector2.One * -1, Vector2.One);

        _graphicsDevice.SetRenderTarget(null);

        if (SaveDebugTextures)
        {
            using (StreamWriter sw = new(@"Mie.jpg"))
            {
                _mieRenderTarget.SaveAsJpeg(sw.BaseStream, _mieRenderTarget.Width, _mieRenderTarget.Height);
            }

            using (StreamWriter sw = new(@"Rayleigh.jpg"))
            {
                _rayleighRenderTarget.SaveAsJpeg(sw.BaseStream, _rayleighRenderTarget.Width, _rayleighRenderTarget.Height);
            }
        }
    }

    private static Model GenerateDomeModel(Material material)
    {
        int domeN = 32;
        int latitude = domeN / 2;
        int longitude = domeN;
        int dVSize = longitude * latitude;
        int dISize = (longitude - 1) * (latitude - 1) * 2;
        dVSize *= 2;
        dISize *= 2;

        VertexPositionTexture[] vertices = new VertexPositionTexture[dVSize];

        int DomeIndex = 0;
        for (int i = 0; i < longitude; i++)
        {
            double moveXZ = -100.0f * (i / (longitude - 1.0f)) * MathHelper.Pi / 180.0;

            for (int j = 0; j < latitude; j++)
            {
                double MoveY = -MathHelper.Pi * j / (latitude - 1);

                vertices[DomeIndex] = new VertexPositionTexture();
                vertices[DomeIndex].Position.X = (float)(Math.Sin(moveXZ) * Math.Cos(MoveY));
                vertices[DomeIndex].Position.Y = (float)Math.Cos(moveXZ);
                vertices[DomeIndex].Position.Z = (float)(Math.Sin(moveXZ) * Math.Sin(MoveY));
                vertices[DomeIndex].TextureCoordinate.X = 0.5f / longitude + i / (float)longitude;
                vertices[DomeIndex].TextureCoordinate.Y = 0.5f / latitude + j / (float)latitude;

                DomeIndex++;
            }
        }
        for (int i = 0; i < longitude; i++)
        {
            double moveXZ = -100.0 * (i / (float)(longitude - 1)) * MathHelper.Pi / 180.0;

            for (int j = 0; j < latitude; j++)
            {
                double MoveY = -(MathHelper.Pi * 2.0) + MathHelper.Pi * j / (latitude - 1);

                vertices[DomeIndex] = new VertexPositionTexture();
                vertices[DomeIndex].Position.X = (float)(Math.Sin(moveXZ) * Math.Cos(MoveY));
                vertices[DomeIndex].Position.Y = (float)Math.Cos(moveXZ);
                vertices[DomeIndex].Position.Z = (float)(Math.Sin(moveXZ) * Math.Sin(MoveY));
                vertices[DomeIndex].TextureCoordinate.X = 0.5f / longitude + i / (float)longitude;
                vertices[DomeIndex].TextureCoordinate.Y = 0.5f / latitude + j / (float)latitude;

                DomeIndex++;
            }
        }

        short[] indices = new short[dISize * 3];
        int index = 0;
        for (short i = 0; i < longitude - 1; i++)
        {
            for (short j = 0; j < latitude - 1; j++)
            {
                indices[index++] = (short)(i * latitude + j);
                indices[index++] = (short)((i + 1) * latitude + j);
                indices[index++] = (short)((i + 1) * latitude + j + 1);

                indices[index++] = (short)((i + 1) * latitude + j + 1);
                indices[index++] = (short)(i * latitude + j + 1);
                indices[index++] = (short)(i * latitude + j);
            }
        }
        short Offset = (short)(latitude * longitude);
        for (short i = 0; i < longitude - 1; i++)
        {
            for (short j = 0; j < latitude - 1; j++)
            {
                indices[index++] = (short)(Offset + i * latitude + j);
                indices[index++] = (short)(Offset + (i + 1) * latitude + j + 1);
                indices[index++] = (short)(Offset + (i + 1) * latitude + j);

                indices[index++] = (short)(Offset + i * latitude + j + 1);
                indices[index++] = (short)(Offset + (i + 1) * latitude + j + 1);
                indices[index++] = (short)(Offset + i * latitude + j);
            }
        }

        return ModelExtensions.CreateModel(vertices, indices, material);
    }

    //private void GenerateMoon()
    //{
    //    quadVerts =
    //       [
    //           new VertexPositionTexture(new Vector3(1,-1,0), new Vector2(1,1)),
    //           new VertexPositionTexture(new Vector3(-1,-1,0), new Vector2(0,1)),
    //           new VertexPositionTexture(new Vector3(-1,1,0), new Vector2(0,0)),
    //           new VertexPositionTexture(new Vector3(1,1,0), new Vector2(1,0))
    //       ];

    //    quadIb = [0, 1, 2, 2, 3, 0];
    //}

    private Color GetSunColor(float fTheta, int nTurbidity)
    {
        float fBeta = 0.04608365822050f * nTurbidity - 0.04586025928522f;
        float fTauR, fTauA;
        float[] fTau = new float[3];

        float coseno = (float)Math.Cos((double)fTheta + Math.PI);
        double factor = (double)fTheta / Math.PI * 180.0;
        double jarl = Math.Pow(93.885 - factor, -1.253);
        float potencia = (float)jarl;
        float m = 1.0f / (coseno + 0.15f * potencia);

        int i;
        float[] fLambda = new float[3];
        fLambda[0] = Parameters.WaveLengths.X;
        fLambda[1] = Parameters.WaveLengths.Y;
        fLambda[2] = Parameters.WaveLengths.Z;

        for (i = 0; i < 3; i++)
        {
            potencia = (float)Math.Pow(fLambda[i], 4.0);
            fTauR = (float)Math.Exp((double)(-m * 0.008735f * potencia));

            const float fAlpha = 1.3f;
            potencia = (float)Math.Pow(fLambda[i], (double)-fAlpha);
            if (m < 0.0f)
                fTau[i] = 0.0f;
            else
            {
                fTauA = (float)Math.Exp((double)(-m * fBeta * potencia));
                fTau[i] = fTauR * fTauA;
            }

        }

        return new Color(fTau[0], fTau[1], fTau[2], 1.0f);
    }

    //private void GeneratePermTex(GraphicsDevice graphicsDevice)
    //{
    //    int[] perm = { 151,160,137,91,90,15,
    //    131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
    //    190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
    //    88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
    //    77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
    //    102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
    //    135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
    //    5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
    //    223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
    //    129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
    //    251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
    //    49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
    //    138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
    //    };

    //    int[] gradValues = { 1,1,0,
    //        -1,1,0, 1,-1,0,
    //        -1,-1,0, 1,0,1,
    //        -1,0,1, 1,0,-1,
    //        -1,0,-1, 0,1,1,
    //        0,-1,1, 0,1,-1,
    //        0,-1,-1, 1,1,0,
    //        0,-1,1, -1,1,0,
    //        0,-1,-1
    //    };

    //    permTex = new Texture2D(graphicsDevice, 256, 256, true, SurfaceFormat.Color);

    //    byte[] pixels;
    //    pixels = new byte[256 * 256 * 4];
    //    for (int i = 0; i < 256; i++)
    //    {
    //        for (int j = 0; j < 256; j++)
    //        {
    //            int offset = (i * 256 + j) * 4;
    //            byte value = (byte)perm[j + perm[i] & 0xFF];
    //            pixels[offset + 1] = (byte)(gradValues[value & 0x0F] * 64 + 64);
    //            pixels[offset + 2] = (byte)(gradValues[value & 0x0F + 1] * 64 + 64);
    //            pixels[offset + 3] = (byte)(gradValues[value & 0x0F + 2] * 64 + 64);
    //            pixels[offset] = value;
    //        }
    //    }

    //    (permTex as Texture2D)?.SetData(pixels);
    //}

    public void ApplyChanges()
    {
        UpdateMieRayleighTextures();
    }
}
