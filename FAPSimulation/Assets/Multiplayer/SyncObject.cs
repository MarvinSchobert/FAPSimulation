using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using OculusSampleFramework;

public class SyncObject : MonoBehaviour
{
    public GameMangagement GameManager; 
    public string ID;
    public string prefabName;
    public Vector3 LastPos;
    public Quaternion LastRot;
    public float scaleFactor = 1;
    public bool UpdateTransform = true;
    public string SyncInfoString;
    public int SyncEveryXFrame;
    public string[] categories;

    public bool AddTablet;
    public bool isChild;
    public bool hasPhysics;

    public bool lockRotationX;
    public bool lockRotationY;
    public bool lockRotationZ;

    private Vector3 velocity;
    public Quaternion initRotation;
    public float lastScale = 1;
    DistanceGrabbable grab;


    public void Start()
    {
        if (SyncEveryXFrame == 0) SyncEveryXFrame = 3;
        grab = GetComponent<DistanceGrabbable>();

        

    }
    public void Init(bool send = true, int forceId = -1)
    {
        if (GameManager == null)
        {
            GameManager = GameObject.FindWithTag("GameManagement").GetComponent<GameMangagement>();
        }
        if (ID == "")
        {
            if (forceId == -1)
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
            else
            {
                ID = forceId.ToString();
            }
           
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
            obj["scaleX"] = transform.localScale.x;
            obj["scaleY"] = transform.localScale.y;
            obj["scaleZ"] = transform.localScale.z;
            obj["syncInfoString"] = SyncInfoString;

            GameManager.sender.SpawnObjectRequest(obj);
            // Die Children bekommen einfach id ID+1:
            SyncObject[] sync = GetComponentsInChildren<SyncObject>();
            for (int s = 0; s < sync.Length; s++ )
            {
                if (sync[s].gameObject != gameObject)
                {
                    Debug.Log("Syncing Child");
                    sync[s].Init(true, int.Parse(ID) + s + 1);
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
        initRotation = transform.rotation;
    }

    public void ToggleUpdateTransform()
    {
        UpdateTransform = !UpdateTransform;
    }

   
    IEnumerator UpdateTransformRoutine()
    {
        LastPos = transform.position;
        LastRot = transform.rotation;
        Vector3 initScale = transform.localScale;
       
        while (true)
        {           
            if (velocity != Vector3.zero && GameManager.isClientLeader)
            {
                transform.Translate(velocity* Time.deltaTime, Space.World);
                velocity *= 0.9f;

                if (velocity.magnitude < 0.01f) velocity = Vector3.zero;
            }


            // Physik beachten 
            if (hasPhysics && GameManager.isClientLeader)
            {
                // velocity += ((transform.position - LastPos) - velocity) *0.9f;
                GetComponent<Rigidbody>().isKinematic = false;
            }

            if (( LastPos != transform.position || LastRot != transform.rotation || lastScale != scaleFactor))
            {
                if (lastScale != scaleFactor)
                {
                    transform.localScale = initScale * scaleFactor;
                    lastScale = scaleFactor;
                }
                if (hasPhysics && GameManager.isClientLeader)
                {
                    
                    velocity = ((transform.position - LastPos)) *20f;
                    Debug.Log("Apply Physics: " + velocity);
                    GetComponent<Rigidbody>().velocity = velocity;
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
                    obj["scaleX"] = transform.localScale.x;
                    obj["scaleY"] = transform.localScale.y;
                    obj["scaleZ"] = transform.localScale.z;
                    obj["syncInfoString"] = SyncInfoString;
                    // ggf. noch mit die H�nde synchronisieren
                    if (GetComponent<NetworkPlayer>() != null)
                    {
                        if (GetComponent<NetworkPlayer>().leftHand != null && GetComponent<NetworkPlayer>().rightHand)
                        {
                            Vector3 lhp = GetComponent<NetworkPlayer>().leftHand.position;
                            Vector3 rhp = GetComponent<NetworkPlayer>().rightHand.position;
                            string ls = "";
                            string rs = "";
                            ls = lhp[0] + "_" + lhp[1] + "_" + lhp[2];
                            rs = rhp[0] + "_" + rhp[1] + "_" + rhp[2];
                            obj["leftHandRelPos"] = ls;
                            obj["rightHandRelPos"] = rs;
                        }
                    }
                    GameManager.sender.SpawnObjectRequest(obj);
                }

                
            }
            if (SyncEveryXFrame == 0) SyncEveryXFrame = 3;
            for (int i = 0; i < SyncEveryXFrame; i++)
            {
                yield return null;
            }
        }
    }
    //public bool breakLerp = false;
    //public IEnumerator lerpTransform (Vector3 pos, Quaternion rot)
    //{
    //    Debug.Log("Lerping");
    //    // Delay: 0,2 Sekunden
    //    for (float i = 0; i < 0.2f; i+= Time.deltaTime)
    //    {
    //        transform.position = Vector3.Lerp(transform.position, pos, i);
    //        transform.rotation = Quaternion.Lerp(transform.rotation, rot, i);
    //        LastPos = transform.position;
    //        LastRot = transform.rotation;
    //        if (breakLerp) break;
    //        yield return null;
    //    }
    //    breakLerp = false;
    //    yield return null;
    //}
    public void LateUpdate()
    {
        


        // Wenn das Objekt gegriffen wird nicht komplett bunt rotieren
        //Vector3 localRot = transform.rotation.eulerAngles;
        //if (lockRotationX)
        //{
        //    localRot.x = initRotation.eulerAngles.x;
        //}
        //if (lockRotationY)
        //{
        //    localRot.y = initRotation.eulerAngles.y;
        //}
        //if (lockRotationZ)
        //{
        //    localRot.z = initRotation.eulerAngles.z;
        //}

        //transform.rotation = Quaternion.Euler(localRot);
    }
}
