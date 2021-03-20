using System;
using System.Collections.Generic;
using ArtemkaKun.Scripts.Tools;
using UnityEngine;

namespace ArtemkaKun.Scripts.RangeChancesSystem
{
    /// <summary>
    /// Struct, that contains data about one chances unit.
    /// </summary>
    /// <typeparam name="T">Type of the object, for which we will store chances.</typeparam>
    [Serializable]
    public struct RangeChancesUnit<T>
    {
        [SerializeField] private T chanceObject;

        [SerializeField, EditorReadOnly] private string chanceHumanForm;

        [SerializeField] private float chanceRawForm;

        [SerializeField] private Range unitRange;

        public T ChanceObject => chanceObject;

        public string ChanceHumanForm => chanceHumanForm;

        public float ChanceRawForm => chanceRawForm;

        public Vector2 UnitRange => unitRange.RangeValue;

        public Range UnitRangeObject => unitRange;

        /// <summary>
        /// Sets new chance range of this unit.
        /// </summary>
        /// <param name="newUnitChanceRange">Value of chance range.ss</param>
        public void SetUnitRange(Vector2 newUnitChanceRange)
        {
            unitRange.SetData(newUnitChanceRange);

            SetChanceValue(unitRange.GetRangeLength());
        }

        private void SetChanceValue(float newRawChance)
        {
            chanceRawForm = newRawChance;
            chanceHumanForm = $"{(newRawChance * 100).ToString()}%";
        }

        #region Operators

        public static bool operator ==(RangeChancesUnit<T> item1, RangeChancesUnit<T> item2)
        {
            return item1.UnitRange == item2.UnitRange && FloatHelper.CheckIfProvidedFloatsAreEqual(item1.ChanceRawForm, item2.ChanceRawForm);
        }

        public static bool operator !=(RangeChancesUnit<T> item1, RangeChancesUnit<T> item2)
        {
            return !(item1.UnitRange == item2.UnitRange || FloatHelper.CheckIfProvidedFloatsAreEqual(item1.ChanceRawForm, item2.ChanceRawForm));
        }

        public bool Equals(RangeChancesUnit<T> other)
        {
            return EqualityComparer<T>.Default.Equals(chanceObject, other.chanceObject) &&
                   chanceHumanForm == other.chanceHumanForm && chanceRawForm.Equals(other.chanceRawForm) &&
                   unitRange.Equals(other.unitRange);
        }

        public override bool Equals(object obj)
        {
            return obj is RangeChancesUnit<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EqualityComparer<T>.Default.GetHashCode(chanceObject);
                hashCode = (hashCode * 397) ^ (chanceHumanForm != null ? chanceHumanForm.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ chanceRawForm.GetHashCode();
                hashCode = (hashCode * 397) ^ unitRange.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}