using System;
using UnityEngine;

public class ToggleIcon: MonoBehaviour
{
    [SerializeField] private GameObject toggleObject;
    [SerializeField] private KeyCode key;

    private bool toggleOn = true;
    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            toggleOn = !toggleOn;
            
            toggleObject.SetActive(toggleOn);
        }
    }
}