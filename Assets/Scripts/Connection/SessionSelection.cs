using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SessionSelection : MonoBehaviour
{
    [SerializeField] GameObject item;

    public void RefreshSession(List<SessionInfo> sessionList)
    {
        Debug.Log("RefreshSession");
        gameObject.SetActive(true);
        foreach (var session in sessionList)
        {
            var newItem = Instantiate(item, item.transform.parent);
            newItem.SetActive(true);
            newItem.GetComponent<SessionItem>().Init(session.Name);
        }
    }
}
