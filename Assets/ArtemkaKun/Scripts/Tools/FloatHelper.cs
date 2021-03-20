using System;

namespace ArtemkaKun.Scripts.Tools
{
    /// <summary>
    /// Static class, that provides helper methods for floats (like float comparison).
    /// </summary>
    public static class FloatHelper
    {
        private const float FloatComparisonEpsilon = 0.00001f;
        
        /// <summary>
        /// Compares two floats if they are equal with the "epsilon method".
        /// </summary>
        /// <param name="number1">First number to compare.</param>
        /// <param name="number2">Second number to compare.</param>
        /// <returns>True - numbers are equal, false - numbers are different.</returns>
        public static bool CheckIfProvidedFloatsAreEqual(float number1, float number2)
        {
            return Math.Abs(number1 - number2) <= FloatComparisonEpsilon;
        }
    }
}