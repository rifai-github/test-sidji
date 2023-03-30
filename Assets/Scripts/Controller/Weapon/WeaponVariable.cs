using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Sidji.TestProject.Controller.Weapon
{
    public class WeaponVariable : NetworkBehaviour
    {
        [SerializeField] Weapon weapon;
        [Networked] public NetworkBool inAttack { get; set; }

        public Weapon GetWeapon => weapon;
    }

    public enum Weapon
    {
        Sword,
        Stick,
        Hit,

    }
}