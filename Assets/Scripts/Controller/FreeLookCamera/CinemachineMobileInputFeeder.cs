using Cinemachine;
using UnityEngine;

namespace Sidji.TestProject.Controller.Freelook
{
    public class CinemachineMobileInputFeeder : MonoBehaviour
    {
        //[SerializeField] private UITouchPanel touchPanelInput;
        [SerializeField] private TouchPanel touchInput;

        private Vector2 lookInput;

        [SerializeField] private float touchSpeedSensitivityX = 3f;
        [SerializeField] private float touchSpeedSensitivityY = 3f;

        private string touchXMapTo = "Mouse X";
        private string touchYMapTo = "Mouse Y";

        void Start()
        {
            CinemachineCore.GetInputAxis = GetInputAxis;
        }

        private float GetInputAxis(string axisName)
        {
            lookInput = touchInput.PlayerJoystickOutputVector();

            if (axisName == touchXMapTo)
                return lookInput.x / touchSpeedSensitivityX;

            if (axisName == touchYMapTo)
                return lookInput.y / touchSpeedSensitivityY;

            return Input.GetAxis(axisName);
        }
    }
}

