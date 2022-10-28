using System;
using UnityEngine;

public class ForceZPosition : MonoBehaviour
{
    [SerializeField] private float position;
 
    private void LateUpdate()
    {
        var pos = transform.position;
        pos.z = position;

        transform.position = pos;
    }
}