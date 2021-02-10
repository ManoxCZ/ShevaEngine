using System;

namespace ShevaEngine.UI
{
	/// <summary>
	/// Button class.
	/// </summary>	
	public class SliderFloat : Slider<float>
	{
        /// <summary>
        /// Get ratio.
        /// </summary>
        protected override float GetRatio()
        {
            return Value.Value / (Max.Value - Min.Value);
        }

        /// <summary>
        /// Set ratio.
        /// </summary>
        protected override void SetRatio(float ratio)
        {
            Value.OnNext(
                Math.Min(Max.Value,
                Math.Max(Min.Value, 
                ((Max.Value - Min.Value) * ratio + Min.Value))));
        }
	}
}
