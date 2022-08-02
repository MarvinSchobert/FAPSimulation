using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class SyncObject : MonoBehaviour
{
    public GameMangagement GameManager; 
    public string ID;
    public string prefabName;
    public Vector3 LastPos;
    public Quaternion LastRot;
    public bool UpdateTransform = true;
    public string SyncInfoString;

    public void Start()
    {
        
    }
    public void Init(bool send = true)
    {
        if (send)
        {
            Debug.Log("Initializing Gameobject");
            JObject obj = new JObject();
            obj["type"] = "SpawnRqt";
            obj["prefabName"] = prefabName;
            obj["name"] = name;
            obj["ID"] = ID;
            obj["posX"] = transform.position.x;
            obj["posY"] = transform.position.y;
            obj["posZ"] = transform.position.z;
            obj["rotX"] = transform.rotation.x;
            obj["rotY"] = transform.rotation.y;
            obj["rotZ"] = transform.rotation.z;
            obj["rotW"] = transform.rotation.w;
            obj["syncInfoString"] = SyncInfoString;

            GameManager.sender.SpawnObjectRequest(obj);
            Debug.Log(obj.ToString());
        }
        StartCoroutine(UpdateNetwork());
    }

    public void ToggleUpdateTransform()
    {
        UpdateTransform = !UpdateTransform;
    }
    IEnumerator UpdateNetwork()
    {
        LastPos = transform.position;
        LastRot = transform.rotation;
        while (true)
        {
            if (UpdateTransform && ( LastPos != transform.position || LastRot != transform.rotation))
            {
                LastPos = transform.position;
                LastRot = transform.rotation;
                JObject obj = new JObject();
                obj["type"] = "ChangeRqt";
                obj["name"] = name;
                obj["prefabName"] = prefabName;
                obj["ID"] = ID;
                obj["posX"] = transform.position.x;
                obj["posY"] = transform.position.y;
                obj["posZ"] = transform.position.z;
                obj["rotX"] = transform.rotation.x;
                obj["rotY"] = transform.rotation.y;
                obj["rotZ"] = transform.rotation.z;
                obj["rotW"] = transform.rotation.w;
                obj["syncInfoString"] = SyncInfoString;
                GameManager.sender.SpawnObjectRequest(obj);
                
            }
            yield return null;
        }
    }
}
