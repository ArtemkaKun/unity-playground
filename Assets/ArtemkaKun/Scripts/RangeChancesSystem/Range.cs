using System;
using ArtemkaKun.Scripts.Tools;
using UnityEngine;

namespace ArtemkaKun.Scripts.RangeChancesSystem
{
    /// <summary>
    /// Structure, that stores data about range.
    /// </summary>
    [Serializable]
    public struct Range
    {
        [SerializeField, EditorReadOnly] private Vector2 range;
        [SerializeField, EditorReadOnly] private bool isXInclusive;
        [SerializeField, EditorReadOnly] private bool isYInclusive;

        public Vector2 RangeValue => range;

        /// <summary>
        /// Set new range and calculate inclusive bounds.
        /// </summary>
        /// <param name="newRange">New range value</param>
        public void SetData(Vector2 newRange)
        {
            range = ClampNewRangeValues(newRange);

            isXInclusive = range.x == 0 || FloatHelper.CheckIfProvidedFloatsAreEqual(1f, range.x);

            isYInclusive = true;
        }

        private Vector2 ClampNewRangeValues(Vector2 newRange)
        {
            if (newRange.x < 0)
            {
                newRange.x = 0;
            }

            if (newRange.y > 1)
            {
                newRange.y = 1;
            }

            newRange.x = (float) Math.Round(newRange.x, 2);
            newRange.y = (float) Math.Round(newRange.y, 2);

            return newRange;
        }

        /// <summary>
        /// Get length between range's y and x values.
        /// </summary>
        /// <returns></returns>
        public float GetRangeLength()
        {
            return range.y - range.x;
        }

        /// <summary>
        /// Checks is value positioned in current range.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns></returns>
        public bool CheckIfValueInRange(float value)
        {
            if (isXInclusive && FloatHelper.CheckIfProvidedFloatsAreEqual(value, range.x))
            {
                return true;
            }

            if (isYInclusive && FloatHelper.CheckIfProvidedFloatsAreEqual(value, range.y))
            {
                return true;
            }

            return value > range.x && value < range.y;
        }
    }
}