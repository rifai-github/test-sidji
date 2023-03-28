using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sidji.TestProject.Controller
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public event Action<Vector2> OnJoystickInput;

        [SerializeField] RectTransform joystickContainer;
        [SerializeField] RectTransform joystickHandle;

        [Header("Joystick Settings")]
        [SerializeField] float joystickRange = 50f;

        private void Start()
        {
            SetJoystickInput(Vector2.zero);
            joystickHandle.anchoredPosition = Vector2.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetJoystickInput(Vector2.zero);
            joystickHandle.anchoredPosition = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickContainer, eventData.position, eventData.pressEventCamera, out Vector2 touchPosition);
            touchPosition = touchPosition / ((joystickContainer.sizeDelta) / 2);

            var clampedPosition = Vector2.ClampMagnitude(touchPosition, 1);
            joystickHandle.anchoredPosition = clampedPosition * joystickRange;
            
            SetJoystickInput(clampedPosition);
        }

        private void SetJoystickInput(Vector2 input)
        {
            OnJoystickInput?.Invoke(input);
        }
    }
}