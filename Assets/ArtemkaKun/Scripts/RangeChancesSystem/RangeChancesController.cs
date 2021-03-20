using System;
using System.Collections.Generic;
using System.Linq;
using ArtemkaKun.Scripts.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ArtemkaKun.Scripts.RangeChancesSystem
{
    /// <summary>
    /// Class, that contains data and methods of RangeChances controller.
    /// </summary>
    /// <typeparam name="T">Type of the object, for which we will store chances.</typeparam>
    public class RangeChancesController<T> : MonoBehaviour
    {
        private const int RangeRightBound = 1;
        private const int PartToCutForNewUnit = 2;

        [SerializeField] private List<RangeChancesUnit<T>> rangeChancesUnits = new List<RangeChancesUnit<T>>();

        [SerializeField, EditorReadOnly]
        private List<RangeChancesUnit<T>> previousListData = new List<RangeChancesUnit<T>>();

        public Action onChancesChanged;

        public List<RangeChancesUnit<T>> RangeChancesUnits => rangeChancesUnits;

        private void OnValidate()
        {
            CheckOnChancesChangedDelegate();

            if (rangeChancesUnits.Count == previousListData.Count)
            {
                ProceedChangedData();
            }
            else
            {
                ProceedChangedUnits();
            }

            UpdatePreviousData();
        }

        private void ProceedChangedData()
        {
            for (int i = 0; i < rangeChancesUnits.Count; i++)
            {
                if (rangeChancesUnits[i] == previousListData[i])
                {
                    continue;
                }

                CalculateChanges(i);

                break;
            }
        }

        private void CalculateChanges(int i)
        {
            var currentItemData = rangeChancesUnits[i];

            var differenceInValues = Mathf.Clamp(currentItemData.ChanceRawForm, 0, 1) - previousListData[i].ChanceRawForm;

            currentItemData.SetUnitRange(currentItemData.UnitRange + new Vector2(0, differenceInValues));

            if (i + 1 < rangeChancesUnits.Count)
            {
                UpdateItem(i + 1, new Vector2(differenceInValues, 0));
            }
            else if (i - 1 > -1)
            {
                currentItemData.SetUnitRange(currentItemData.UnitRange - new Vector2(differenceInValues, 0));

                UpdateItem(i - 1, new Vector2(0, -differenceInValues));
            }

            rangeChancesUnits[i] = currentItemData;
        }

        private void ProceedChangedUnits()
        {
            if (rangeChancesUnits.Count > previousListData.Count)
            {
                AddUnit();
            }
            else
            {
                RemoveUnit();
            }
        }

        private void CheckOnChancesChangedDelegate()
        {
            onChancesChanged ??= UpdatePreviousData;
        }

        private void AddUnit()
        {
            // 15.03.2021. Artem Yurchenko.
            // Because of OnValidate() function triggered after the new element was added to the list, I
            // decided to remove a new empty element and add new with data.
            rangeChancesUnits.Remove(rangeChancesUnits.Last());

            Vector2 rangeOfNewUnit;

            if (rangeChancesUnits.Count == 0)
            {
                rangeOfNewUnit = new Vector2(0, RangeRightBound);
            }
            else
            {
                var lastUnit = rangeChancesUnits.Last();

                var halfLengthOfLastRange = lastUnit.ChanceRawForm / PartToCutForNewUnit;

                var halfOfTheLastUnitRange = lastUnit.UnitRange - new Vector2(0, halfLengthOfLastRange);

                lastUnit.SetUnitRange(halfOfTheLastUnitRange);

                rangeChancesUnits[rangeChancesUnits.Count - 1] = lastUnit;

                rangeOfNewUnit = new Vector2(halfOfTheLastUnitRange.y, RangeRightBound);
            }

            rangeChancesUnits.Add(CreateNewRangeChancesUnit(rangeOfNewUnit));
        }

        private RangeChancesUnit<T> CreateNewRangeChancesUnit(Vector2 newRange)
        {
            var newRangeChancesUnit = new RangeChancesUnit<T>();

            newRangeChancesUnit.SetUnitRange(newRange);

            return newRangeChancesUnit;
        }

        private void RemoveUnit()
        {
            var removedItemId = GetRemovedItemId();

            if (removedItemId == 0)
            {
                if (removedItemId >= rangeChancesUnits.Count)
                {
                    return;
                }

                UpdateItem(0, new Vector2(-previousListData[removedItemId].ChanceRawForm, 0));
            }
            else
            {
                UpdateItem(removedItemId - 1, new Vector2(0, previousListData[removedItemId].ChanceRawForm));
            }
        }

        private int GetRemovedItemId()
        {
            var removedUnit = previousListData.Except(rangeChancesUnits);

            return previousListData.FindIndex(unit => unit == removedUnit.First());
        }

        private void UpdateItem(int anotherItemId, Vector2 rangeShift)
        {
            var anotherItem = rangeChancesUnits[anotherItemId];

            var newRangeForPreviousItem = anotherItem.UnitRange + rangeShift;

            anotherItem.SetUnitRange(newRangeForPreviousItem);

            rangeChancesUnits[anotherItemId] = anotherItem;
        }

        private void UpdatePreviousData()
        {
            previousListData = new List<RangeChancesUnit<T>>(rangeChancesUnits);
        }

        /// <summary>
        /// Get an object from random unit.
        /// </summary>
        /// <returns>Object of type T</returns>
        protected T GetRandomUnitObject()
        {
            var randomValue = Random.value;

            foreach (var rangeUnit in rangeChancesUnits)
            {
                if (rangeUnit.UnitRangeObject.CheckIfValueInRange(randomValue))
                {
                    return rangeUnit.ChanceObject;
                }
            }

            return rangeChancesUnits.FirstOrDefault().ChanceObject;
        }
    }
}