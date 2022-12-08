using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRMultiplayerController : MonoBehaviour
{
    public TMPro.TMP_Text UiText; 
    public void EnableMultiplayerCallback()
    {
        GetComponent<SyncObject>().enabled = true;
        GetComponent<SyncObject>().Init();       
        GetComponent<SyncObject>().UpdateTransform = !GetComponent<SyncObject>().UpdateTransform;
        if (GetComponent<SyncObject>().UpdateTransform)
        {
            UiText.text = "Sync Network";
        }else
        {
            UiText.text = "Don't Sync";
        }
            
    }
}
