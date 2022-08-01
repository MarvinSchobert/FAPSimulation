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

    List<SyncObject> RemoteObjects;


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
        Invoke("InitRemoteObjects", 1.0f);

    }

       public void InitRemoteObjects()
    {
        for (int i = 0; i < RemoteObjects.Count; i++)
        {
            RemoteObjects[i].GameManager = this;
            RemoteObjects[i].Init();
        }
    }

    public void SpawnObjectCallback(JObject obj)
    {      
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
            }
        }        
    }

    public void ChangeObjectCallback(JObject obj)
    {
        Debug.Log("Updating Transform " + obj.ToString()) ;
        for (int i = 0; i < RemoteObjects.Count; i++)
        {
            if (RemoteObjects[i].ID == obj["ID"].ToString())
            {
                RemoteObjects[i].transform.position = new Vector3(float.Parse(obj["posX"].ToString()), float.Parse(obj["posY"].ToString()), float.Parse(obj["posZ"].ToString()));
                RemoteObjects[i].transform.rotation = new Quaternion(float.Parse(obj["rotX"].ToString()), float.Parse(obj["rotY"].ToString()), float.Parse(obj["rotZ"].ToString()), float.Parse(obj["rotW"].ToString()));
                RemoteObjects[i].LastPos = RemoteObjects[i].transform.position;
                RemoteObjects[i].LastRot = RemoteObjects[i].transform.rotation;
            }
        }
    }
}
