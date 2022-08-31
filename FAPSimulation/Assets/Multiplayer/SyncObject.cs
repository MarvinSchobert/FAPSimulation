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
    public int SyncEveryXFrame;

    
    public bool isChild;
    public bool hasPhysics;

    private Vector3 velocity;

    public void Start()
    {
        if (SyncEveryXFrame == 0) SyncEveryXFrame = 1;
    }
    public void Init(bool send = true)
    {
        if (GameManager == null)
        {
            GameManager = GameObject.FindWithTag("GameManagement").GetComponent<GameMangagement>();
        }
        if (ID == "")
        {
            int highestID = 0;
            for (int j = 0; j < GameManager.RemoteObjects.Count; j++)
            {
                if (GameManager.RemoteObjects[j].ID != "" && int.Parse(GameManager.RemoteObjects[j].ID) >= highestID)
                {
                    highestID = int.Parse(GameManager.RemoteObjects[j].ID) + 1;
                    
                }
            }
            if (!GameManager.RemoteObjects.Contains(this))
            {
                GameManager.RemoteObjects.Add(this);
            }   
            ID = highestID.ToString();
            Debug.Log("Successful: " + ID);
        }
        if (send)
        {
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
            // Die Children bekommen einfach id ID+1:
            foreach (SyncObject sync in GetComponentsInChildren<SyncObject>())
            {
                if (sync.gameObject != gameObject)
                {
                    Debug.Log("Syncing Child");
                    sync.Init();
                }
            }
        }
        else
        {
            // Die Children bekommen einfach id ID+1:
            foreach (SyncObject sync in GetComponentsInChildren<SyncObject>())
            {
                if (sync.gameObject != gameObject)
                {
                    Debug.Log("Syncing Child");
                    sync.Init(false);
                }
            }
        }
       
        StartCoroutine(UpdateTransformRoutine());
    }

    public void ToggleUpdateTransform()
    {
        UpdateTransform = !UpdateTransform;
    }

   
    IEnumerator UpdateTransformRoutine()
    {
        LastPos = transform.position;
        LastRot = transform.rotation;
        while (true)
        {           
            if (velocity != Vector3.zero)
            {
                transform.Translate(velocity* Time.deltaTime, Space.World);
                velocity *= 0.9f;

                if (velocity.magnitude < 0.01f) velocity = Vector3.zero;
            }

            if (( LastPos != transform.position || LastRot != transform.rotation))
            {
                // Physik beachten bei Änderungen
                if (hasPhysics)
                {
                    velocity += ((transform.position - LastPos) - velocity) *0.9f;
                }

                // Transform syncen
                if (UpdateTransform)
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

                
            }
            if (SyncEveryXFrame == 0) SyncEveryXFrame = 1;
            for (int i = 0; i < SyncEveryXFrame; i++)
            {
                yield return null;
            }
        }
    }
}
