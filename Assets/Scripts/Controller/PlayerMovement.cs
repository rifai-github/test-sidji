using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


namespace Sidji.TestProject.Controller
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] Animator playerAnimator;
        [SerializeField] Transform mainCamera;
        [SerializeField] InputActionReference inputMovement;
        [SerializeField] InputActionReference attack;

        [SerializeField] float movementSpeed;
        
        Vector3 movementPosition;
        float smoothTransition;

        private void Update()
        {
            var inputMovemnet = inputMovement.action.ReadValue<Vector2>();
            movementPosition = new Vector3(inputMovemnet.x, 0, inputMovemnet.y);
            smoothTransition = Mathf.Lerp(smoothTransition, movementPosition.magnitude, Time.deltaTime * movementSpeed);
            playerAnimator.SetFloat("movement", smoothTransition);
            transform.position = Vector3.Lerp(transform.position, transform.position + movementPosition, Time.deltaTime * movementSpeed);
            transform.forward = Vector3.Slerp(transform.forward, movementPosition, Time.deltaTime * movementSpeed);

            
            if (attack.action.triggered)
            {
                playerAnimator.SetTrigger("attack");
            }
        }
    }
}