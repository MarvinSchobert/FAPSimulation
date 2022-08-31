using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncParentTransform : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.parent.position = transform.position;
        transform.localPosition = Vector3.zero;
    }
}
