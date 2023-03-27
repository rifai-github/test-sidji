using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Sidji.TestProject.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] GameObject characterObject;
        [SerializeField] Animator playerAnimator;

        [Header("Movement")]
        [SerializeField] VirtualJoystick joystick;
        [SerializeField] float movementSpeed;
        

        [Header("Attack")]
        [SerializeField] Button buttonAttack;

        Transform cameraMain;
        
        Vector2 inputMovement;
        Vector3 movementPosition;
        float smoothTransition;

        private void Start()
        {
            cameraMain = Camera.main.transform;
            buttonAttack.onClick.AddListener(ExecuteAttack);
        }

        private void OnEnable()
        {
            joystick.OnJoystickInput += InputReceived;
        }
        private void OnDisable()
        {
            joystick.OnJoystickInput -= InputReceived;
        }

        private void ExecuteAttack()
        {
            playerAnimator.SetTrigger("attack");
        }

        public void InputReceived(Vector2 input)
        {
            inputMovement = input;
        }

        private void Update()
        {
            movementPosition = (cameraMain.forward * inputMovement.y + cameraMain.right * inputMovement.x);
            movementPosition.y = 0f;
            smoothTransition = Mathf.Lerp(smoothTransition, movementPosition.magnitude, Time.deltaTime * movementSpeed);
            playerAnimator.SetFloat("movement", smoothTransition);
            transform.position = Vector3.Lerp(transform.position, transform.position + movementPosition, Time.deltaTime * movementSpeed);
            transform.forward = Vector3.Slerp(transform.forward, movementPosition, Time.deltaTime * movementSpeed);
        }
    }
}