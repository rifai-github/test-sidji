using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sidji.TestProject.Controller.InputPlayer
{
    using Joystick;
    using Player;

    public class InputCharacterController : MonoBehaviour
    {
        public static InputCharacterController Instance;

        [Header("Movement")]
        [SerializeField] VirtualJoystick movementJoystick;

        [Header("Attack")]
        [SerializeField] Button buttonAttack;

        private void Awake()
        {
            Instance = this;
        }

        public void InitialPlayerControl(NetworkPlayerController player)
        {
            if (player == null)
                return;
            
            movementJoystick.OnJoystickInput += player.InputReceived;
            buttonAttack.onClick.AddListener(player.RpcExecuteAttack);
        }
    }
}