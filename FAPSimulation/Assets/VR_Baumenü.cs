using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VR_Baumenü : MonoBehaviour
{

    public List<GameObject> Items;
    public GameObject UIItem;
    public Transform ContentHolder;

    public bool isFloatingInFrontOfHeadset;
    public Transform VRHeadset;
    public Vector3 FloatingTargetOffset;

    public TMP_Text debugText;
    public Transform RightController;
    // Erstellt für jedes Item einen Button
    public void Start()
    {
        SetUpMenu();
    }
    public void SetUpMenu()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            GameObject go = Instantiate(UIItem, ContentHolder);
            if (Items[i].GetComponent<PlaceableItem>() != null && Items[i].GetComponent<PlaceableItem>().UI_Representative != null)
            {
                RawImage[] images = go.GetComponentsInChildren<RawImage>();
                for (int j = 0; j < images.Length; j++)
                {
                    if (images[j].gameObject != go)
                    {
                        images[j].texture = Items[i].GetComponent<PlaceableItem>().UI_Representative;
                        break;
                    }
                   
                }             
            }
            go.GetComponentInChildren<TMP_Text>().text = Items[i].name;
            int idx = i;
            go.GetComponent<Button>().onClick.AddListener(() => PlaceItem(idx));
        }
    }


    private void Update()
    {
        if (isFloatingInFrontOfHeadset)
        {
            Vector3 demandedPos = VRHeadset.position + VRHeadset.forward * FloatingTargetOffset.z + VRHeadset.right * FloatingTargetOffset.x + VRHeadset.up * FloatingTargetOffset.y;
            transform.position = Vector3.Lerp(transform.position, demandedPos, 5 * Time.deltaTime);
            transform.LookAt(VRHeadset.position);
            transform.Rotate(-90, 0, 180);
            
            
        }
    }

    public void PlaceItem (int idx)
    {
        GameObject item = Items[idx];
        StartCoroutine(PlacingItemRoutine(item));

    }



    public IEnumerator PlacingItemRoutine (GameObject item)
    {
        
        if (true|| OVRInput.Get(OVRInput.Button.One))
        {
            GameObject spawnItem = Instantiate(item);
            Vector3 demandPosition = VRHeadset.position + VRHeadset.forward * 1.5f;
            //while (OVRInput.Get(OVRInput.Button.One))
            //{
            //    if (Physics.Raycast())


            //    yield return null;
            //}
            spawnItem.transform.position = demandPosition;
            debugText.text = "Completed Spawn";
            Debug.Log("Placed " + spawnItem.name + "Successfully");
            if (spawnItem.GetComponent<SyncObject>() != null)
            {
                spawnItem.GetComponent<SyncObject>().Init();
            }
                
        }
       
        yield return null;
    }


}
