using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngineInternal;

namespace Common.Utils
{
    public class MathUtil
    {
        public static Random Random = new Random();

        public static int RoundToInt(float f)
        {
            return (int)Math.Round((double)f);
        }

        public static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }
            if (value > 1f)
            {
                return 1f;
            }
            return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        /// <summary>
        ///   <para>A tiny floating point value (Read Only).</para>
        /// </summary>
        public static readonly float Epsilon = (MathfInternal.IsFlushToZeroEnabled ? MathfInternal.FloatMinNormal : MathfInternal.FloatMinDenormal);


    }


}
