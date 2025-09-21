using System;
using Microsoft.Xna.Framework;

namespace ShevaEngine.SkyDomes;

public class SkyDomeParameters
{    
    public Vector4 LightColor { get; set; }
    public Vector4 LightColorAmbient { get; set; }
    public float FogDensity { get; set; }
    public Vector3 InvWaveLengths { get; private set; }
    public Vector3 WaveLengthsMie { get; private set; }
    private Vector3 _WaveLengths;
    public Vector3 WaveLengths
    {
        get { return _WaveLengths; }
        set
        {
            _WaveLengths = value;

            SetLengths();
        }
    }
    public int NumSamples { get; set; }
    public float Exposure { get; set; }

    public SkyDomeParameters()
    {
        LightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        LightColorAmbient = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
        FogDensity = 0.0003f;
        WaveLengths = new Vector3(0.65f, 0.57f, 0.475f);
        NumSamples = 10;
        Exposure = -2.0f;

        SetLengths();
    }

    private void SetLengths()
    {
        InvWaveLengths = new Vector3(
            1.0f / (float)Math.Pow(WaveLengths.X, 4.0),
            1.0f / (float)Math.Pow(WaveLengths.Y, 4.0),
            1.0f / (float)Math.Pow(WaveLengths.Z, 4.0));

        WaveLengthsMie = new Vector3(
            (float)Math.Pow(WaveLengths.X, -0.84),
            (float)Math.Pow(WaveLengths.Y, -0.84),
            (float)Math.Pow(WaveLengths.Z, -0.84));
    }
}
