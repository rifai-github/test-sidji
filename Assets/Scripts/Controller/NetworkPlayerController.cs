using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Fusion;

namespace Sidji.TestProject.Controller.Player
{
    using Mounting;
    using InputPlayer;
    using Weapon;

    public class NetworkPlayerController : NetworkBehaviour, IPlayerLeft
    {
        [Header("Character")]
        [SerializeField] Slider health;
        [SerializeField] Transform characterObject;
        [SerializeField] Animator playerAnimator;

        [Header("Movement")]
        [SerializeField] float movementSpeed;

        [Header("Weapon")]
        [SerializeField] WeaponVariable weapon;

        Vector2 inputMovement;

        public bool inAttack => playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack02_SwordAndShiled");
        public bool isDie => playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Die01_Stay_SwordAndShield");
        

        int maxHealth = 10;

        Transform cameraMain;
        CinemachineFreeLook freeLook;
        
        [Networked(OnChanged = "RefreshHealth")] int currentHealth { get; set; }
        [Networked] NetworkObject mountingObject { get; set; }
        
        [Networked] Vector3 MovementPosition { get; set; }
        
        [Networked] float smoothTransition { get; set; }

        public static NetworkPlayerController Local;

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;
                Debug.Log("Local Player Spawned");
                InputCharacterController.Instance.InitialPlayerControl(this);
                OnSpawn();
            }
            else
            {
                Debug.Log("Client Player Spawned");
            }

            health.maxValue = maxHealth;
            currentHealth = 10;
            var colliderPlayerTrigger = GetComponent<ColliderTrigger>();
            colliderPlayerTrigger.OnTriggerEnterToObject += OnTriggerObjectEnter;
            colliderPlayerTrigger.OnTriggerExitFromObject += OnTriggerObjectExit;
        }

        private void OnTriggerObjectEnter(GameObject other)
        {
            if (other.TryGetComponent<MountingVariable>(out var mountVar))
            {
                if (other.transform.parent != null) return;
                if (other.TryGetComponent<NetworkObject>(out var networkObj))
                    RpcMountObject(networkObj);
            }
        }

        private void OnTriggerObjectExit(GameObject other)
        {
            if (other.TryGetComponent<WeaponVariable>(out var weaponVar))
            {
                if (weaponVar.inAttack)
                {
                    currentHealth--;
                }
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (player == Object.InputAuthority)
            {
                Runner.Despawn(Object);
            }
        }

        private void OnSpawn()
        {
            cameraMain = Camera.main.transform;
            
            freeLook = FindObjectOfType<CinemachineFreeLook>();

            freeLook.Follow = characterObject;
            freeLook.LookAt = characterObject;
        }


        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RpcExecuteAttack()
        {
            if (mountingObject == null)
            {
                playerAnimator.SetTrigger("attack");
            }
            else
            {
                RpcMountObject(null);
            }
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RpcSetMovePlayer(Vector3 movementPosition)
        {
            MovementPosition = movementPosition;
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RpcRefreshInAttack()
        {
            weapon.inAttack = inAttack;
        }
        
        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RpcMountObject(NetworkObject mounting)
        {
            if (mounting == null)
            {
                if (mountingObject == null) return;

                Instantiate(mountingObject);
                Destroy(mountingObject);

                transform.position = transform.position + (transform.right * 2);
                GetComponent<Rigidbody>().useGravity = true;
                
                if (Object.HasInputAuthority)
                {
                    freeLook.LookAt = characterObject;
                }
            }
            else if (mountingObject == null)
            {
                mountingObject = mounting;

                playerAnimator.SetFloat("movement", 0);
                transform.rotation = mountingObject.transform.rotation;

                transform.localPosition = new Vector3(mountingObject.transform.localPosition.x, mountingObject.transform.localPosition.y + 1.5f, mountingObject.transform.localPosition.z);
                GetComponent<Rigidbody>().useGravity = false;
                transform.localRotation = mountingObject.transform.localRotation;

                mountingObject.transform.SetParent(transform);
                
                if (Object.HasInputAuthority)
                {
                    freeLook.LookAt = mountingObject.transform;
                }
            }
        }

        public static void RefreshHealth(Changed<NetworkPlayerController> changed)
        {
            var player = changed.Behaviour;


            player.health.value = player.currentHealth;
            if (player.currentHealth == 0)
            {
                player.playerAnimator.SetTrigger("die");
            }
        }

        public override void FixedUpdateNetwork()
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + MovementPosition, Time.deltaTime * movementSpeed * (mountingObject == null ? 1 : 1.5f));
            transform.forward = Vector3.Slerp(transform.forward, MovementPosition, Time.deltaTime * movementSpeed * (mountingObject == null ? 1 : 1.5f));
                
            smoothTransition = Mathf.Lerp(smoothTransition, MovementPosition.magnitude, Time.deltaTime * movementSpeed * (mountingObject == null ? 1 : 1.5f));
            if (mountingObject == null) playerAnimator.SetFloat("movement", smoothTransition);
            else mountingObject.GetComponent<Animator>().SetFloat("movement", smoothTransition);
        }

        public void InputReceived(Vector2 input)
        {
            if (!isDie)
                inputMovement = input;
        }

        private void Update()
        {
            if (cameraMain != null)
            {
                var movementPosition = (cameraMain.forward * inputMovement.y + cameraMain.right * inputMovement.x);
                movementPosition.y = 0f;
                
                RpcSetMovePlayer(movementPosition);
                RpcRefreshInAttack();
            }
        }
    }
}