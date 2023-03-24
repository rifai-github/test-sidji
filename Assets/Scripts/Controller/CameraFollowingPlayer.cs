using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sidji.TestProject.Controller
{
    public class CameraFollowingPlayer : MonoBehaviour
    {
        [SerializeField] GameObject player;

        public void SetPlayer(GameObject player)
        {
            this.player = player;
        }


        private void LateUpdate() {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * 5);
        }
    }
}