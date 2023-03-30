using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Sidji.TestProject.Controller
{
    public class ColliderTrigger : MonoBehaviour
    {
        public event Action<GameObject> OnTriggerEnterToObject;
        public event Action<GameObject> OnTriggerExitFromObject;

        private void OnTriggerEnter(Collider other)
        {    
            OnTriggerEnterToObject?.Invoke(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitFromObject?.Invoke(other.gameObject);
        }
    }
}