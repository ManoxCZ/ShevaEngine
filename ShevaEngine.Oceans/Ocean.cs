using Microsoft.Xna.Framework;
using ShevaEngine.Core;
using System;
using System.Collections.Generic;

namespace ShevaEngine.Oceans
{
    /// <summary>
    /// Ocean.
    /// </summary>
    public class Ocean
    {
        public List<GerstnerWave> Waves { get; private set; } = new List<GerstnerWave>();


        /// <summary>
        /// Generate waves.
        /// </summary>
        public void GenerateWaves(float maxAmplitude, float size, float timeScale, int wavesCount)
        {
            Waves = new List<GerstnerWave>();

            int dec = 3;

            float a = maxAmplitude;
            Random random = new Random();

            for (int i = 0; i < wavesCount; ++i)
            {
                double rot = MathUtils.Lerp(-Math.PI, Math.PI, random.NextDouble());

                double amp = MathUtils.Lerp(a * 0.5, a, random.NextDouble());
                double wavelength = amp * MathUtils.Lerp(size, size * 2.0, random.NextDouble());
                double k = 2 * Math.PI / wavelength;

                Waves.Add(new GerstnerWave()
                {
                    Direction = new Vector2((float)-Math.Sin(rot), (float)Math.Cos(rot)),
                    Amplitude = (float)amp,
                    K = (float)k,
                    W = (float)Math.Sqrt(9.8 * k) * timeScale,
                    Phase = (float)random.NextDouble() * 1000,
                });

                if (((i + 1) % dec) == 0)
                    a /= dec;
            }
        }

        /// <summary>
        /// Get position normal.
        /// </summary>
        public (Vector3 Position, Vector3 Normal) GetPositionNormal(Vector3 position, float time)
        {
            double Pinch = 0;

            Vector3 positionCorrection = Vector3.Zero;
            Vector3 normal = Vector3.Up;

            foreach (GerstnerWave wave in Waves)
            {
                double wl = Math.PI / wave.K;

                double p = wave.K * Vector3.Dot(new Vector3(wave.Direction.X, 0, wave.Direction.Y), position) - time * wave.W + wave.Phase;
                double cosine = Math.Cos(p);
                Vector2 d = new Vector2(wave.Direction.X, wave.Direction.Y) * (float)Math.Sin(p) * (float)Pinch;

                positionCorrection += new Vector3(-d.X * wave.Amplitude, (float)(wave.Amplitude * cosine), -d.Y * wave.Amplitude);
                normal += Vector3.Normalize(new Vector3(d.X, d.Y, (float)cosine) * wave.K) * wave.Amplitude;
            }

            normal = Vector3.Normalize(normal);

            return (position + positionCorrection, normal);
        }
    }
}
