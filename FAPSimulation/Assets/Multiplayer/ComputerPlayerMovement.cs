using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayerMovement : MonoBehaviour
{
    public GameMangagement GameManager;
    Vector3 mousePos;

    private void Start()
    {
        mousePos = Input.mousePosition;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<SyncObject>().Init();
            GetComponent<SyncObject>().enabled = true;
        }
        // Translation
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - mousePos; 
            transform.Translate(new Vector3 (-delta.x, 0, -delta.y)*Time.deltaTime);           
        }
        // Rotation
        else if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - mousePos;
            transform.RotateAround(transform.position -transform.up*5, Vector3.up, delta.x * 0.1f);
            transform.RotateAround(transform.position -transform.up*5,transform.right, -delta.y * 0.1f);
        }
        else if (Input.mouseScrollDelta.y != 0)
        {
            transform.Translate(Vector3.down * Input.mouseScrollDelta.y * Time.deltaTime * 45);
        }
        mousePos = Input.mousePosition;
    }
}
