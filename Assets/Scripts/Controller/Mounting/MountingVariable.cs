using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sidji.TestProject.Controller.Mounting
{
    public class MountingVariable : MonoBehaviour
    {
        [SerializeField] MountingType type;
        [SerializeField] Animator mountingAnimator;
        [SerializeField] GameObject mountingObject;

        public Animator MountingAnimator => mountingAnimator;
        public GameObject MountingObject => mountingObject;
    }

    public enum MountingType
    {
        Bear,
        Horse,
        Car,
    }
}