using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidPlayerMovement : MonoBehaviour
{
    public GameObject AndroidCamera;
    public GameMangagement GameManager;
    float Moving = 0;
   
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
        //Vector3 dir = Vector3.zero;

        //dir.x = -Input.acceleration.y + 1.0f;
        //dir.z = Input.acceleration.x;

        //if (dir.sqrMagnitude > 1)
        //    dir.Normalize();

        //dir *= Time.deltaTime;

        //transform.Translate(dir * 1f);
        //GetComponent<SyncObject>().SyncInfoString = dir.ToString();
        transform.Translate(Vector3.forward * Time.deltaTime * 3 * Moving);


    }
    public void EnableMultiplayerCallback()
    {
        GetComponent<SyncObject>().Init();
        GetComponent<SyncObject>().enabled = true;
        UiText.text= "Multiplayer Request done";
    }

    public void PlaceObject()
    {
        GameManager.SpawnLocalObject(null, transform.position + AndroidCamera.transform.forward*1, AndroidCamera.transform.rotation);
    }

    public void SetMovement(float nr)
    {
        Moving = nr;
    }
}
