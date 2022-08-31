using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class GameMangagement : MonoBehaviour
{
    // Start is called before the first frame update

    public enum Platform
    {
        Computer, Android, VirtualReality
    }
    public Platform activePlatform;


    public GameObject AndroidObject;
    public GameObject ComputerObject;
    public GameObject VRObject;

    [HideInInspector] public UDPSend sender;

    
    public List <SyncObject>  RemoteObjectsDatabase;

    public List<SyncObject> RemoteObjects;


    public void Start()
    {
        switch (activePlatform)
        {
            case Platform.Android:
                AndroidObject.SetActive(true);
                ComputerObject.SetActive(false);
                VRObject.SetActive(false);
                sender = AndroidObject.GetComponentInChildren<UDPSend>();
                break;
            case Platform.Computer:
                AndroidObject.SetActive(false);
                ComputerObject.SetActive(true);
                VRObject.SetActive(false);
                sender = ComputerObject.GetComponentInChildren<UDPSend>();
                break;
            case Platform.VirtualReality:
                AndroidObject.SetActive(false);
                ComputerObject.SetActive(false);
                VRObject.SetActive(true);
                sender = VRObject.GetComponentInChildren<UDPSend>();
                break;
        }

        RemoteObjects = new List<SyncObject>(FindObjectsOfType<SyncObject>());
        Invoke("InitRemoteObjects", 1.5f);

    }

    public void InitRemoteObjects()
    {
        for (int i = 0; i < RemoteObjects.Count; i++)
        {
            
            if (RemoteObjects[i].ID == "")
            {
                int highestID = 0;
                for (int j = 0; j < RemoteObjects.Count; j++)
                {
                    if (RemoteObjects[j].ID != "" && int.Parse(RemoteObjects[j].ID) >= highestID)
                    {
                        highestID = int.Parse(RemoteObjects[j].ID) + 1;
                    }
                }
                RemoteObjects[i].ID = highestID.ToString();
            }
            RemoteObjects[i].GameManager = this;
            
            RemoteObjects[i].Init();            
        }
    }

    public void SpawnLocalObject(GameObject obj, Vector3 pos, Quaternion rot)
    {
        // Höchste ID finden
        int highestID = 0;
        for (int i = 0; i < RemoteObjects.Count; i++)
        {
            if (int.Parse(RemoteObjects[i].ID) >= highestID)
            {
                highestID = int.Parse(RemoteObjects[i].ID) + 1;
            }
        }

        if (obj == null)
        {
            obj = RemoteObjectsDatabase[1].gameObject;
        }
        GameObject go = Instantiate(obj, pos, rot);
        go.GetComponent<SyncObject>().GameManager = this;
        go.GetComponent<SyncObject>().ID = highestID.ToString();
        go.GetComponent<SyncObject>().Init();
        RemoteObjects.Add(go.GetComponent<SyncObject>());
    }

    public void SpawnObjectCallback(JObject obj)
    {
        Debug.Log("Spawn Object " + obj.ToString()); 
        for (int i = 0; i < RemoteObjects.Count; i++)
        {
            if (RemoteObjects[i].ID == obj["ID"].ToString())
            {
                return;
            }
        }
        for (int i = 0; i < RemoteObjectsDatabase.Count; i++)
        {
            if (RemoteObjectsDatabase[i].prefabName == obj["prefabName"].ToString())
            {                
                GameObject newObject = Instantiate(RemoteObjectsDatabase[i].gameObject, new Vector3(float.Parse(obj["posX"].ToString()), float.Parse(obj["posY"].ToString()), float.Parse(obj["posZ"].ToString())), new Quaternion(float.Parse(obj["rotX"].ToString()), float.Parse(obj["rotY"].ToString()), float.Parse(obj["rotZ"].ToString()), float.Parse(obj["rotW"].ToString())));
                newObject.name = obj["name"].ToString();                
                newObject.GetComponent<SyncObject>().ID = obj["ID"].ToString();
                newObject.GetComponent<SyncObject>().GameManager = this;
                RemoteObjects.Add(newObject.GetComponent<SyncObject>());
                newObject.GetComponent<SyncObject>().Init(false);
                break;
            }
        }        
    }

    public void RemoveObjectCallback (JObject obj)
    {
        Debug.Log("Removing Object " + obj.ToString());
        for (int i = 0; i < RemoteObjects.Count; i++)
        {
            if (RemoteObjects[i].ID == obj["ID"].ToString())
            {
                Debug.Log("Found right object: " + RemoteObjects[i].gameObject.name);
                Destroy(RemoteObjects[i].gameObject);
                RemoteObjects.Remove(RemoteObjects[i]);
                return;
            }
        }
    }

    public void SyncVarCallback(JObject obj)
    {
        Debug.Log("Syncing Var " + obj.ToString());
        for (int i = 0; i < RemoteObjects.Count; i++)
        {
            if (RemoteObjects[i].ID == obj["ID"].ToString())
            {
                Debug.Log("Found right object: " + RemoteObjects[i].gameObject.name);
                if (RemoteObjects[i].GetComponent<NetworkPlayer>() != null)
                {
                    RemoteObjects[i].GetComponent<NetworkPlayer>().stats = (JObject) obj["stats"];
                }


                return;
            }
        }
    }

    public void ChangeObjectCallback(JObject obj)
    {;
        for (int i = 0; i < RemoteObjects.Count; i++)
        {
            if (RemoteObjects[i].ID == obj["ID"].ToString())
            {
                RemoteObjects[i].transform.position = new Vector3(float.Parse(obj["posX"].ToString()), float.Parse(obj["posY"].ToString()), float.Parse(obj["posZ"].ToString()));
                RemoteObjects[i].transform.rotation = new Quaternion(float.Parse(obj["rotX"].ToString()), float.Parse(obj["rotY"].ToString()), float.Parse(obj["rotZ"].ToString()), float.Parse(obj["rotW"].ToString()));
                RemoteObjects[i].LastPos = RemoteObjects[i].transform.position;
                RemoteObjects[i].LastRot = RemoteObjects[i].transform.rotation;
                RemoteObjects[i].SyncInfoString = obj["syncInfoString"].ToString();
            }
        }
    }
}
