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

        bool initialize = false;


        bool inAttack;
        bool isDie => playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Die01_Stay_SwordAndShield");
        

        int maxHealth = 10;

        Transform cameraMain;
        CinemachineFreeLook freeLook;

        [Networked] Vector3 unmountPosition { get; set; }
        [Networked] NetworkBool unmounting { get; set; }
        
        [Networked] int currentHealth { get; set; }
        [Networked] NetworkObject mountingObject { get; set; }
        
        [Networked] Vector3 MovementPosition { get; set; }
        
        [Networked] float smoothTransition { get; set; }

        public static NetworkPlayerController Local;


        private void Init()
        {
            if (initialize) return;

            health.maxValue = maxHealth;
            health.value = maxHealth;
            if (currentHealth > 0)
                health.value = currentHealth;
            else
                currentHealth = maxHealth;

            var colliderPlayerTrigger = GetComponent<ColliderTrigger>();
            colliderPlayerTrigger.OnTriggerEnterToObject += OnTriggerObjectEnter;
            colliderPlayerTrigger.OnTriggerExitFromObject += OnTriggerObjectExit;
        }

        public override void Spawned()
        {
            Init();
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
        }

        private void OnTriggerObjectEnter(GameObject other)
        {
            if (other.TryGetComponent<WeaponVariable>(out var weaponVar))
            {
                if (weaponVar.inAttack)
                {
                    RpcSetHealth(currentHealth - 1);
                }
            }
            
            if (other.TryGetComponent<MountingVariable>(out var mountVar))
            {
                if (other.TryGetComponent<NetworkObject>(out var networkObj))
                {
                    if (mountVar.PassangerInfo != null)
                    {
                        
                        Debug.Log("mountVar.PassangerInfo :: " + mountVar.PassangerInfo.gameObject);
                        return;
                    }

                    Debug.Log("unmounting : " + unmounting);
                    if (unmounting)
                        return;

                    RpcMountObject(networkObj);
                }
            }
        }

        private void OnTriggerObjectExit(GameObject other)
        {
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
                if (!isDie)
                    playerAnimator.SetTrigger("attack");
            }
            else
            {
                RpcMountObject(null);
            }
        }
        
        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RpcSetHealth(int hp)
        {
            currentHealth = hp;
            health.value = currentHealth;

            if (hp == 0)
                playerAnimator.SetTrigger("die");
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RpcSetMovePlayer(Vector3 movementPosition)
        {
            MovementPosition = movementPosition;
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RpcRefreshInAttack()
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack02_SwordAndShiled"))
            {
                if (!inAttack)
                {
                    inAttack = true;
                    weapon.inAttack = true;
                }
                else
                {
                    weapon.inAttack = false;
                }
            }
            else
            {
                inAttack = false;
                weapon.inAttack = false;
            }
        }
        
        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RpcMountObject(NetworkObject mounting)
        {
            if (mounting == null)
            {
                unmounting = true;
                unmountPosition = transform.position + (transform.right * 2);
                
                if (mountingObject != null)
                {
                    if (mountingObject.TryGetComponent<MountingVariable>(out var mountingVariable))
                    {
                        Debug.Log("PassangerInfo Before - " + mountingVariable.PassangerInfo.gameObject.name);
                        mountingVariable.PassangerInfo = null;
                    }
                }
            }
            else
            {
                unmounting = false;
                if (mounting.TryGetComponent<MountingVariable>(out var mountingVariable))
                {
                    mountingVariable.PassangerInfo = this;
                }
                mountingObject = mounting;
            }
            

            playerAnimator.SetFloat("movement", 0);
        }

        public override void FixedUpdateNetwork()
        {
            if (mountingObject != null)
            {
                if (unmounting)
                {
                    Debug.Log("JALAN : " + unmountPosition);
                    transform.position = Vector3.MoveTowards(transform.position, unmountPosition, Time.fixedDeltaTime * movementSpeed);
                    if(Vector3.Distance(transform.position, unmountPosition) < 0.1f)
                    {
                        mountingObject = null;
                        unmounting = false;
                    }
                    return;
                }
            }
            else
            {
                smoothTransition = Mathf.Lerp(smoothTransition, MovementPosition.magnitude, Time.fixedDeltaTime * movementSpeed * (mountingObject == null ? 1 : 2f));
                if (mountingObject == null)
                {
                    playerAnimator.SetFloat("movement", smoothTransition);
                }
            }

            transform.position = Vector3.Lerp(transform.position, transform.position + MovementPosition, Time.fixedDeltaTime * movementSpeed * (mountingObject == null ? 1 : 2f));
            transform.forward = Vector3.Slerp(transform.forward, MovementPosition, Time.fixedDeltaTime * movementSpeed * (mountingObject == null ? 1 : 2f));
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