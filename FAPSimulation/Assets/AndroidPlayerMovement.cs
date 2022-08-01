using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidPlayerMovement : MonoBehaviour
{
    public GameObject AndroidCamera;
    // Start is called before the first frame update
    public Text UiText;

    public void Start()
    {
        Input.gyro.enabled = true;
    }

    public void Update()
    {
        AndroidCamera.transform.Rotate(-Input.gyro.rotationRateUnbiased.x / 2, 0, 0);
        if (AndroidCamera.transform.localEulerAngles.y < -160) { AndroidCamera.transform.localEulerAngles = new Vector3(0,-160,0);}
        if (AndroidCamera.transform.localEulerAngles.y > 160) { AndroidCamera.transform.localEulerAngles = new Vector3(0,160,0);}
        transform.Rotate(0, -Input.gyro.rotationRateUnbiased.y / 2, 0);
       
    }

    public void EnableMultiplayerCallback()
    {
        GetComponent<SyncObject>().Init();
        GetComponent<SyncObject>().enabled = true;
        UiText.text= "Multiplayer Request done";
    }
}
