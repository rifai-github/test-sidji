// Copyright 2019 Google LLC
// All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Agones
{
    [RequireComponent(typeof(AgonesSdk))]
    public class AgonesServerBehaviour : MonoBehaviour
    {
        public static AgonesServerBehaviour Instance;

        private int Port { get; set; } = 7777;
        private UdpClient client = null;
        public AgonesSdk AgonesSdk {get; set;}


        private void Awake()
        {
            #if !UNITY_STANDALONE_LINUX || UNITY_EDITOR
            Debug.Log("#if !UNITY_STANDALONE_LINUX || UNITY_EDITOR");
            gameObject.SetActive(false);
            #else
            Debug.Log("#if UNITY_STANDALONE_LINUX");
            DontDestroyOnLoad(gameObject);
            Instance = this;
            #endif           
        }

        async void Start()
        {
            client = new UdpClient(Port);

            AgonesSdk = GetComponent<AgonesSdk>();
            bool ok = await AgonesSdk.Connect();
            if (ok)
            {
                Debug.Log(("Server - Connected"));
            }
            else
            {
                Debug.Log(("Server - Failed to connect, exiting"));
                Application.Quit(1);
            }

            ok = await AgonesSdk.Ready();

            if (ok)
            {
                Debug.Log($"Server - Ready");
                var gameserver = await AgonesSdk.GameServer();
                string serverName = gameserver.ObjectMeta.Name;
                Debug.Log("Server Name : " + serverName);
                //Create Session Fusion if Agones initialized
                MainNetworkRunner.Instance.StartSessionFusion(serverName);
            }
            else
            {
                Debug.Log($"Server - Ready failed");
                Application.Quit();
            }
        }

        void OnDestroy()
        {
            client.Close();
            Debug.Log("Server - Close");
        }
    }
}