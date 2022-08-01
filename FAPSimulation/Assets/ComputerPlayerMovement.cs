using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<SyncObject>().Init();
            GetComponent<SyncObject>().enabled = true;
        }
    }
}
