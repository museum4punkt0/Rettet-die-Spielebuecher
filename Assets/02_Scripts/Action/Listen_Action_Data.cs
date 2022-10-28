using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Listen_Action_Data : MonoBehaviour
{
    void Awake()
    {
       Messenger<string>.AddListener("NEW_ACTION_DATA",NewActionData);
    }

    void Destroy()
    {
       Messenger<string>.RemoveListener("NEW_ACTION_DATA",NewActionData);
    }

    public void NewActionData(string data) {
      Debug.Log("NewActionData NewActionData NewActionData NewActionData" + data);
    }
}
