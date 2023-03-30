using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Sidji.TestProject.Controller.Freelook
{
    public class TouchPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        Vector2 playerTouchVectorOutput;
        bool isPlayerTouchingPanel;
        Touch myTouch;
        int touchID;

        private void FixedUpdate()
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    myTouch = Input.GetTouch(i);
                    if (isPlayerTouchingPanel)
                    {
                        if (myTouch.fingerId == touchID)
                        {
                            if (myTouch.phase != TouchPhase.Moved)
                                OutputVectorValue(Vector2.zero);
                        }
                    }
                }
            }
        }

        private void OutputVectorValue(Vector2 outputValue)
        {
            playerTouchVectorOutput = outputValue;
        }

        public Vector2 PlayerJoystickOutputVector()
        {
            return playerTouchVectorOutput;
        }

        public void OnPointerUp(PointerEventData _onPointerUpData)
        {
            OutputVectorValue(Vector2.zero);
            isPlayerTouchingPanel = false;
        }

        public void OnPointerDown(PointerEventData _onPointerDownData)
        {
            OnDrag(_onPointerDownData);
            touchID = myTouch.fingerId;
            isPlayerTouchingPanel = true;
        }

        public void OnDrag(PointerEventData _onDragData)
        {
            OutputVectorValue(new Vector2(_onDragData.delta.normalized.x, _onDragData.delta.normalized.y));
        }
    }
}

