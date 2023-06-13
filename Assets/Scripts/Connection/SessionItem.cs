using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SessionItem : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text text;
    
    public void Init(string sessionName)
    {
        text.text = sessionName;
        button.onClick.AddListener(()=>
        {
            MainNetworkRunner.Instance.StartSessionFusion(sessionName);
        });
    }
}
