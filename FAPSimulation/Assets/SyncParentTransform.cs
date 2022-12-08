using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncParentTransform : MonoBehaviour
{

    public Transform hmdObject;
    public Transform eyePosition;
    private void Start()
    {
        StartCoroutine(SyncUpdate());
    }
    IEnumerator SyncUpdate()
    {
        while (true)
        {
            Vector3 syncPos = 
                new Vector3(hmdObject.position.x-eyePosition.localPosition.x, hmdObject.position.y - eyePosition.localPosition.y, hmdObject.position.z -eyePosition.localPosition.z);
            Vector3 rotation = hmdObject.rotation.eulerAngles;
            // hmdObject.transform.localPosition = new Vector3(0, hmdObject.transform.localPosition.y, 0);
            transform.position = syncPos;
            transform.rotation = Quaternion.Euler(0, rotation.y, 0);
            // transform.parent.position = transform.position;
           // transform.localPosition = Vector3.zero;
            yield return null;
        }
      
    }
}
