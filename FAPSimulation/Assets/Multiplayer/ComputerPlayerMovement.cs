using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayerMovement : MonoBehaviour
{
    public GameMangagement GameManager;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<SyncObject>().Init();
            GetComponent<SyncObject>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            PlaceObject();
        }
    }

    public void PlaceObject()
    {
        GameManager.SpawnLocalObject(null, transform.position + transform.forward * 1, transform.rotation);
    }
}
