using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Sidji.TestProject.Controller.Mounting
{
    using Player;
    public class MountingVariable : NetworkBehaviour
    {
        [SerializeField] MountingType type;
        [Networked] public NetworkPlayerController PassangerInfo { get; set; }


        public override void FixedUpdateNetwork()
        {
            if (PassangerInfo != null)
            {
                transform.position = Vector3.Lerp(transform.position, PassangerInfo.transform.position, Time.fixedDeltaTime * 50);
                transform.forward = Vector3.Slerp(transform.forward, PassangerInfo.transform.forward, Time.fixedDeltaTime * 50);
            }
        }
    }

    public enum MountingType
    {
        Bear,
        Horse,
        Car,
    }
}