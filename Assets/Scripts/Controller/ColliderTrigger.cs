using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Sidji.TestProject.Controller
{
    public class ColliderTrigger : MonoBehaviour
    {
        public event Action<Collision> OnColosionEnterToObject;
        public event Action<Collision> OnColosionExitFromObject;

        private void OnCollisionEnter(Collision other)
        {    
            OnColosionEnterToObject?.Invoke(other);
        }

        private void OnCollisionExit(Collision other)
        {
            OnColosionExitFromObject?.Invoke(other);
        }
    }
}