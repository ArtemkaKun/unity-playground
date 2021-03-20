using ArtemkaKun.Scripts.RangeChancesSystem;
using UnityEditor;
using UnityEngine;

namespace ArtemkaKun.Scripts.Editor
{
    /// <summary>
    /// Class, that handles UI of RangeChancesController.
    /// </summary>
    /// <typeparam name="T">Type of the object, for which we will store chances.</typeparam>
    [CustomEditor(typeof(RangeChancesController<>))]
    public class RangeChancesControllerUi<T> : UnityEditor.Editor
    {
        private const float SliderStep = 0.01f;
        private const int SliderbarTopMargin = 2;
        private const int SliderbarHeight = 24;
        private const int SliderbarBottomMargin = 2;
        private const int PartitionHandleWidth = 2;
        private const int PartitionHandleExtraHitAreaWidth = 2;
        private const string RangeChancesSliderName = "Range chances controller";
        private const string SliderbarStyleName = "LODSliderRange";

        private readonly int _spawnChancesSliderId = "spawnChancesSliderId".GetHashCode();

        private readonly Color[] _spawnRangeColors =
        {
            new Color(0.5f, 0.5f, 0.6f, 1.0f),
            new Color(0.5f, 0.6f, 0.5f, 1.0f),
            new Color(0.6f, 0.6f, 0.5f, 1.0f),
            new Color(0.6f, 0.5f, 0.5f, 1.0f),
            new Color(0.760f, 0.694f, 0.658f),
            new Color(0.815f, 0.780f, 0.709f),
            new Color(0.701f, 0.760f, 0.541f),
            new Color(0.549f, 0.717f, 0.552f),
            new Color(0.490f, 0.588f, 0.690f),
            new Color(0.580f, 0.752f, 0.752f)
        };

        private int _sliderControlId;
        private int _hotPartitionHandleIndex = -1;
        private GUIStyle _textStyle;
        private DragCache _dragCache;

        private void OnEnable()
        {
            _sliderControlId = GUIUtility.GetControlID(_spawnChancesSliderId, FocusType.Passive);

            if (_textStyle != null || EditorStyles.whiteMiniLabel == null)
            {
                return;
            }

            _textStyle = new GUIStyle(EditorStyles.whiteMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var targetRangeChancesController = (RangeChancesController<T>) target;

            _hotPartitionHandleIndex = -1;

            var cascadeSliderWidth = DrawRanges(targetRangeChancesController);

            switch (Event.current.GetTypeForControl(_sliderControlId))
            {
                case EventType.MouseDown:
                    CreateCacheAndActivateControl();

                    break;

                case EventType.MouseUp:
                    DeactivateControlAndFlushCache();

                    break;

                case EventType.MouseDrag:
                    ProceedDragAction(cascadeSliderWidth, targetRangeChancesController);

                    break;
            }
        }

        private float DrawRanges(RangeChancesController<T> targetRangeChancesController)
        {
            var sliderRect = CreateSliderRectangle();

            float currentX = sliderRect.x;

            float cascadeBoxStartY = sliderRect.y + SliderbarTopMargin;

            var rangeChanceUnitsCount = targetRangeChancesController.RangeChancesUnits.Count;
            
            float cascadeSliderWidth = sliderRect.width - rangeChanceUnitsCount * PartitionHandleWidth;

            int colorIndex = -1;

            Color origTextColor = GUI.color;

            Color origBackgroundColor = GUI.backgroundColor;

            for (int i = 0; i < rangeChanceUnitsCount; ++i)
            {
                colorIndex = (colorIndex + 1) % _spawnRangeColors.Length;

                GUI.backgroundColor = _spawnRangeColors[colorIndex];

                float boxLength = cascadeSliderWidth * targetRangeChancesController.RangeChancesUnits[i].ChanceRawForm;

                Rect partitionRect = new Rect(currentX, cascadeBoxStartY, boxLength, SliderbarHeight);

                GUI.Box(partitionRect, GUIContent.none, SliderbarStyleName);

                currentX += boxLength;

                GUI.color = Color.white;

                Rect textRect = partitionRect;

                var cascadeText = $"Elem. {i} - {targetRangeChancesController.RangeChancesUnits[i].ChanceHumanForm}";

                GUI.Label(textRect, new GUIContent(cascadeText), _textStyle);

                if (i == rangeChanceUnitsCount - 1)
                {
                    break;
                }

                GUI.backgroundColor = Color.black;

                Rect handleRect = partitionRect;

                handleRect.x = currentX;

                handleRect.width = PartitionHandleWidth;

                GUI.Box(handleRect, GUIContent.none, SliderbarStyleName);

                Rect handleHitRect = handleRect;

                handleHitRect.xMin -= PartitionHandleExtraHitAreaWidth;

                handleHitRect.xMax += PartitionHandleExtraHitAreaWidth;

                if (handleHitRect.Contains(Event.current.mousePosition))
                {
                    _hotPartitionHandleIndex = i;
                }

                if (_dragCache == null)
                {
                    EditorGUIUtility.AddCursorRect(handleHitRect, MouseCursor.ResizeHorizontal, _sliderControlId);
                }

                currentX += PartitionHandleWidth;
            }

            GUI.color = origTextColor;

            GUI.backgroundColor = origBackgroundColor;

            return cascadeSliderWidth;
        }

        private Rect CreateSliderRectangle()
        {
            GUILayout.Label(RangeChancesSliderName);

            var sliderRect = GUILayoutUtility.GetRect(
                GUIContent.none,
                SliderbarStyleName,
                GUILayout.Height(SliderbarTopMargin + SliderbarHeight + SliderbarBottomMargin),
                GUILayout.ExpandWidth(true)
            );

            GUI.Box(sliderRect, GUIContent.none);

            return sliderRect;
        }

        private void CreateCacheAndActivateControl()
        {
            if (_hotPartitionHandleIndex < 0)
            {
                return;
            }

            _dragCache = new DragCache(_hotPartitionHandleIndex, Event.current.mousePosition);

            if (GUIUtility.hotControl == 0)
            {
                GUIUtility.hotControl = _sliderControlId;
            }

            Event.current.Use();
        }

        private void DeactivateControlAndFlushCache()
        {
            if (GUIUtility.hotControl == _sliderControlId)
            {
                GUIUtility.hotControl = 0;
                
                Event.current.Use();
            }

            _dragCache = null;
        }

        private void ProceedDragAction(float cascadeSliderWidth, RangeChancesController<T> targetRangeChancesController)
        {
            if (GUIUtility.hotControl != _sliderControlId)
            {
                return;
            }
            
            if (!CalculateDeltaAndCheckSliderStep(cascadeSliderWidth, out var delta))
            {
                return;
            }

            if (ValidateSliderMovement(targetRangeChancesController, delta))
            {
                UpdateItems(targetRangeChancesController, delta);
            }

            var currentEvent = Event.current;
            
            _dragCache.lastCachedMousePosition = currentEvent.mousePosition;
            
            currentEvent.Use();
        }

        private bool CalculateDeltaAndCheckSliderStep(float cascadeSliderWidth, out float delta)
        {
            delta = (Event.current.mousePosition - _dragCache.lastCachedMousePosition).x / cascadeSliderWidth;

            if (Mathf.Abs(delta) < SliderStep)
            {
                return false;
            }

            delta = Mathf.Sign(delta) * SliderStep;
            
            return true;
        }

        private bool ValidateSliderMovement(RangeChancesController<T> targetRangeChancesController, float delta)
        {
            bool isLeftPartitionHappy = targetRangeChancesController.RangeChancesUnits[_dragCache.activePartitionId].ChanceRawForm + delta > 0.0f;
            
            bool isRightPartitionHappy = targetRangeChancesController.RangeChancesUnits[_dragCache.activePartitionId + 1].ChanceRawForm - delta > 0.0f;

            return isLeftPartitionHappy && isRightPartitionHappy;
        }

        private void UpdateItems(RangeChancesController<T> targetRangeChancesController, float delta)
        {
            UpdateLeftItem(targetRangeChancesController, delta);

            if (_dragCache.activePartitionId < targetRangeChancesController.RangeChancesUnits.Count - 1)
            {
                UpdateRightItem(targetRangeChancesController, delta);
            }

            targetRangeChancesController.onChancesChanged?.Invoke();

            GUI.changed = true;
        }

        private void UpdateLeftItem(RangeChancesController<T> targetRangeChancesController, float delta)
        {
            var selectedValue = targetRangeChancesController.RangeChancesUnits[_dragCache.activePartitionId];

            selectedValue.SetUnitRange(selectedValue.UnitRange + new Vector2(0, delta));

            targetRangeChancesController.RangeChancesUnits[_dragCache.activePartitionId] = selectedValue;
        }

        private void UpdateRightItem(RangeChancesController<T> targetRangeChancesController, float delta)
        {
            var nextItemIndex = _dragCache.activePartitionId + 1;
            
            var nextItemValue = targetRangeChancesController.RangeChancesUnits[nextItemIndex];

            nextItemValue.SetUnitRange(nextItemValue.UnitRange + new Vector2(delta, 0f));

            targetRangeChancesController.RangeChancesUnits[nextItemIndex] = nextItemValue;
        }

        private class DragCache
        {
            public readonly int activePartitionId;
            public Vector2 lastCachedMousePosition;

            public DragCache(int newActivePartitionId, Vector2 currentMousePos)
            {
                activePartitionId = newActivePartitionId;
                lastCachedMousePosition = currentMousePos;
            }
        }
    }
}