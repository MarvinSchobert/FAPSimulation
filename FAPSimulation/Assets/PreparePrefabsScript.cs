using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PreparePrefabsScript : MonoBehaviour
{
    public GameObject MultiplayerObject;
    public GameObject RootObject;
    public void Start()
    {
        PreparePrefabs();
    }

    public void PreparePrefabs()
    {
        foreach (Transform so in RootObject.GetComponentsInChildren<Transform>())
        {
            if (so.gameObject != RootObject && so.parent == RootObject.transform)
            {
                GameObject g = Instantiate(MultiplayerObject);
                so.transform.parent = g.transform;
                so.transform.localPosition = Vector3.zero;
                so.transform.localRotation = Quaternion.identity;

                g.GetComponent<SyncObject>().prefabName = so.name;
                g.name = g.GetComponent<SyncObject>().prefabName;
                g.GetComponent<BoxCollider>().size = so.transform.GetComponent<Collider>().bounds.size;
                if (so.GetComponent<MeshCollider>() != null) so.GetComponent<MeshCollider>().convex = true;
            }
        }
    }
}
