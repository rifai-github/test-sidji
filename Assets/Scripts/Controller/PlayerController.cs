using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

namespace Sidji.TestProject.Controller.Player
{
    using Mounting;

    public class PlayerController : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] GameObject characterObject;
        [SerializeField] Animator playerAnimator;

        [Header("Movement")]
        [SerializeField] VirtualJoystick joystick;
        [SerializeField] float movementSpeed;
        

        [Header("Attack")]
        [SerializeField] Button buttonAttack;

        Transform cameraMain;
        CinemachineFreeLook freeLook;
        
        GameObject mountingObject;
        Animator mountingAnimator;
        
        Vector2 inputMovement;
        Vector3 movementPosition;
        float smoothTransition;

        RigidbodyConstraints tempConstraintCharacter;

        private void OnEnable()
        {
            cameraMain = Camera.main.transform;
            freeLook = FindObjectOfType<CinemachineFreeLook>();

            joystick.OnJoystickInput += InputReceived;
            buttonAttack.onClick.AddListener(ExecuteAttack);

            tempConstraintCharacter = characterObject.GetComponent<Rigidbody>().constraints;

            if (characterObject != null)
            {
                var colliderPlayerTrigger = characterObject.GetComponent<ColliderTrigger>();
                colliderPlayerTrigger.OnColosionEnterToObject += OnCollisionEnterToPlayer;
            }
        }

        private void OnDisable()
        {
            joystick.OnJoystickInput -= InputReceived;
            buttonAttack.onClick.RemoveListener(ExecuteAttack);
            
            if (characterObject != null)
            {
                var colliderPlayerTrigger = characterObject.GetComponent<ColliderTrigger>();
                colliderPlayerTrigger.OnColosionEnterToObject -= OnCollisionEnterToPlayer;
            }
        }

        private void OnCollisionEnterToPlayer(Collision collision)
        {
            if (mountingObject != null) return;

            if (collision.gameObject.GetComponent<MountingVariable>())
            {
                var mounting = collision.gameObject.GetComponent<MountingVariable>();
                
                playerAnimator.SetFloat("movement", 0);

                mountingObject = mounting.MountingObject;
                mountingAnimator = mounting.MountingAnimator;

                freeLook.Follow = mountingObject.transform;
                freeLook.LookAt = mountingObject.transform;

                characterObject.transform.parent = mountingObject.transform;
                characterObject.transform.localPosition = new Vector3(0, 2, 0);
                characterObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        private void ExecuteAttack()
        {
            if (mountingObject == null)
            {
                playerAnimator.SetTrigger("attack");
            }
            else
            {
                characterObject.transform.localPosition = mountingObject.transform.right;

                characterObject.transform.parent = transform;
                freeLook.Follow = characterObject.transform;
                freeLook.LookAt = characterObject.transform;

                mountingAnimator.SetFloat("movement", 0);
                mountingObject = null;
                mountingAnimator = null;
            }

        }

        public void InputReceived(Vector2 input)
        {
            inputMovement = input;
        }

        private void FixedUpdate()
        {
            movementPosition = (cameraMain.forward * inputMovement.y + cameraMain.right * inputMovement.x);
            movementPosition.y = 0f;

            smoothTransition = Mathf.Lerp(smoothTransition, movementPosition.magnitude, Time.deltaTime * movementSpeed);

            if (mountingObject == null)
            {
                playerAnimator.SetFloat("movement", smoothTransition);
                characterObject.transform.position = Vector3.Lerp(characterObject.transform.position, characterObject.transform.position + movementPosition, Time.deltaTime * movementSpeed);
                characterObject.transform.forward = Vector3.Slerp(characterObject.transform.forward, movementPosition, Time.deltaTime * movementSpeed);
            }
            else
            {
                mountingAnimator.SetFloat("movement", smoothTransition);
                mountingObject.transform.position = Vector3.Lerp(mountingObject.transform.position, mountingObject.transform.position + movementPosition, Time.deltaTime * movementSpeed);
                mountingObject.transform.forward = Vector3.Slerp(mountingObject.transform.forward, movementPosition, Time.deltaTime * movementSpeed);
            }
        }
    }
}