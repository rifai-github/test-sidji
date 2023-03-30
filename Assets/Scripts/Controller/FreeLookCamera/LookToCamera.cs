using UnityEngine;

public class LookToCamera : MonoBehaviour
{
    private void Update()
    {
        transform.eulerAngles = Camera.main.transform.eulerAngles;
    }
}