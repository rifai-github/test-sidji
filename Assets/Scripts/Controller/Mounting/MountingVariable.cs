using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Sidji.TestProject.Controller.Mounting
{
    public class MountingVariable : NetworkBehaviour
    {
        [SerializeField] MountingType type;
    }

    public enum MountingType
    {
        Bear,
        Horse,
        Car,
    }
}