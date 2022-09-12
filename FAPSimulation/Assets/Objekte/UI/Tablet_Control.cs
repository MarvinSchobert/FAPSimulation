using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEditor;

public class Tablet_Control : MonoBehaviour
{

    public List <GameObject> Items;
    public GameObject PlaceableItemButton;
    public Transform AllContentHolder;
    public Transform TopContentHolder;
    public Transform CategoryHolderInstance;
    public Transform CategoryButton;

    public bool isFloatingInFrontOfHeadset;
    public Transform VRHeadset;
    public Vector3 FloatingTargetOffset;

    public TMP_Text debugText;
    public Transform RightController;

    public List<GameObject> Apps;


    // Manipulator:
    public TMP_Text objectName;
    public Slider scaleSlider;
    public SyncObject chosenObject;
    public GameObject manipulator;
    public bool manipulationActive;

    public Slider posSliderX;
    public Slider posSliderY;
    public Slider posSliderZ;
    public Slider rotSliderX;
    public Slider rotSliderY;
    public Slider rotSliderZ;




    // Erstellt für jedes Item einen Button
    public void Start()
    {
        SetUpMenu();
    }

    

    public void SetUpMenu()
    {
        List<GameObject> _Holders = new List<GameObject>();
        // Erst die Items hinzufügen: Items = ...
        // Die RemoteObjectsDatabase füllen:
        GameObject[] allResources = Resources.LoadAll("", typeof(GameObject)).Cast<GameObject>().ToArray();
        foreach (GameObject go in allResources)
        {
            if ((go.GetComponent<SyncObject>() != null && go.GetComponent<SyncObject>().AddTablet) || go.GetComponent<PlaceableItem>() != null)
            {
                Items.Add(go);
            }
        }

        // Jetzt auf die Menüs verteilen:
        for (int i = 0; i < Items.Count; i++)
        {
            GameObject _holder = null;
            // Zuerst entsprechenden categoryHolder erzeugen falls noch nicht existent
            string[] categories = Items[i].GetComponent<SyncObject>().categories; // bspw neue category Fabrik,Anlagen,Handhabung und es existiert bisher Fabrik,Anlagen und Fabrik,Anlagen,Sonstiges in categories
            int w = 0;
            string n = "";
            for (w = 0; w < categories.Length; w++)
            {
                n += categories[w];
                bool yetEqual = false;
                for (int y = 0; y < _Holders.Count; y++)
                {
                    if (n == _Holders[y].name)
                    {
                        _holder = _Holders[y];
                        yetEqual = true;
                        break;
                    }
                }
                // Es wird bis w gesucht, bis kein Holder mehr übereinstimmt.
                if (!yetEqual) break;
            }
            
            // muss komplett erzeugt werden
            if (_holder == null)
            {
                string n_tot = "";
                GameObject categoryHolderParent = null;
                for (int j = 0; j < categories.Length; j++)
                {
                    string s = categories[j];
                    n_tot += s;
                    if (j == 0)
                    {
                        GameObject buttonObj = Instantiate(CategoryButton.gameObject, TopContentHolder); 
                        buttonObj.GetComponentInChildren<TMP_Text>().text = categories[j];
                        buttonObj.name = n_tot;

                        GameObject categoryHolder = Instantiate(CategoryHolderInstance.gameObject, AllContentHolder);
                        categoryHolder.GetComponentInChildren<TMP_Text>().text = categories[j];
                        categoryHolder.name = n_tot;
                        categoryHolder.SetActive(false);
                        _Holders.Add(categoryHolder);

                        //// Noch die Buttons instanzieren...
                        //// Zurück
                        categoryHolder.GetComponentInChildren<TMP_Text>().text = "<- zurück zur Auswahl";
                        GameObject h2 = categoryHolder;
                        categoryHolder.GetComponentInChildren<Button>().onClick.AddListener(() => BaumenüNavigate(TopContentHolder.gameObject, h2));

                        //// hin:
                        buttonObj.GetComponentInChildren<Button>().onClick.AddListener(() => BaumenüNavigate(h2, TopContentHolder.gameObject));

                        categoryHolderParent = categoryHolder;
                        _holder = categoryHolderParent;
                    }
                    else
                    {
                        GameObject buttonObj = Instantiate(CategoryButton.gameObject, categoryHolderParent.transform);
                        buttonObj.GetComponentInChildren<TMP_Text>().text = categories[j];
                        buttonObj.name = n_tot;


                        GameObject categoryHolder = Instantiate(CategoryHolderInstance.gameObject, AllContentHolder);
                        categoryHolder.GetComponentInChildren<TMP_Text>().text = categories[j];
                        categoryHolder.name = n_tot;                        
                        categoryHolder.SetActive(false);
                        _Holders.Add(categoryHolder);

                        //// Noch die Buttons instanzieren...
                        //// Zurück
                        categoryHolder.GetComponentInChildren<TMP_Text>().text = "<- zurück zu " + categories[j-1];
                        GameObject h1 = categoryHolderParent.gameObject;
                        GameObject h2 = categoryHolder;
                        categoryHolder.GetComponentInChildren<Button>().onClick.AddListener(() => BaumenüNavigate(h1, h2));

                        //// hin:
                        buttonObj.GetComponentInChildren<Button>().onClick.AddListener(() => BaumenüNavigate(h2, h1));

                        categoryHolderParent = categoryHolder;
                        _holder = categoryHolderParent;
                    }
                    
                }
            }
            // _holder muss noch erweitert werden
            else if (_holder.name != n)
            {
                
                string n_tot = "";
                for (int j = 0; j < w; j++)
                {
                    n_tot += categories[j];
                }
                GameObject categoryHolderParent = _holder;
                for (int j = w; j < categories.Length; j++)
                {
                    string s = categories[j];
                    n_tot += s;

                    GameObject buttonObj = Instantiate(CategoryButton.gameObject, categoryHolderParent.transform);
                    buttonObj.GetComponentInChildren<TMP_Text>().text = categories[j];
                    buttonObj.name = n_tot;


                    GameObject categoryHolder = Instantiate(CategoryHolderInstance.gameObject, AllContentHolder);
                    categoryHolder.GetComponentInChildren<TMP_Text>().text = categories[j];
                    categoryHolder.name = n_tot;
                    categoryHolder.SetActive(false);
                    _Holders.Add(categoryHolder);
                    categoryHolder.transform.SetAsFirstSibling();

                    //// Noch die Buttons instanzieren...
                    //// Zurück
                    categoryHolder.GetComponentInChildren<TMP_Text>().text = "<- zurück zu " + categories[j - 1];
                    GameObject h1 = categoryHolderParent.gameObject;
                    GameObject h2 = categoryHolder;
                    categoryHolder.GetComponentInChildren<Button>().onClick.AddListener(() => BaumenüNavigate(h1, h2));

                    //// hin:
                    buttonObj.GetComponentInChildren<Button>().onClick.AddListener(() => BaumenüNavigate(h2, h1));

                    categoryHolderParent = categoryHolder;
                    _holder = categoryHolderParent;
                }
            }
            if (_holder != null)
            {
                // Holder Object gefunden
                GameObject go = Instantiate(PlaceableItemButton, _holder.transform);
                if (Items[i].GetComponent<PlaceableItem>() != null && Items[i].GetComponent<PlaceableItem>().UI_Representative != null)
                {
                    RawImage[] images = go.GetComponentsInChildren<RawImage>();
                    for (int x = 0; x < images.Length; x++)
                    {
                        if (images[x].gameObject != go)
                        {
                            images[x].texture = Items[i].GetComponent<PlaceableItem>().UI_Representative;
                            break;
                        }
                    }
                }
                //RawImage[] _images = go.GetComponentsInChildren<RawImage>();
                //for (int x = 0; x < _images.Length; x++)
                //{
                //    if (_images[x].gameObject != go)
                //    {
                //        _images[x].texture = AssetPreview.GetAssetPreview(Items[i]);
                //        break;
                //    }
                //}
                

                go.GetComponentInChildren<TMP_Text>().text = Items[i].name;
                int idx = i;
                go.GetComponent<Button>().onClick.AddListener(() => PlaceItem(idx));
                go.transform.SetAsLastSibling();
            }
            else
            {
                GameObject go = Instantiate(PlaceableItemButton, TopContentHolder.transform);
                if (Items[i].GetComponent<PlaceableItem>() != null && Items[i].GetComponent<PlaceableItem>().UI_Representative != null)
                {
                    RawImage[] images = go.GetComponentsInChildren<RawImage>();
                    for (int x = 0; x < images.Length; x++)
                    {
                        if (images[x].gameObject != go)
                        {
                            images[x].texture = Items[i].GetComponent<PlaceableItem>().UI_Representative;
                            break;
                        }
                    }
                }

                go.GetComponentInChildren<TMP_Text>().text = Items[i].name;
                int idx = i;
                go.GetComponent<Button>().onClick.AddListener(() => PlaceItem(idx));
                go.transform.SetAsLastSibling();
            }
        }
    }

    void BaumenüNavigate (GameObject enable, GameObject disable)
    {
        enable.SetActive(true);
        disable.SetActive(false);
    }

    public void Update()
    {
        if (isFloatingInFrontOfHeadset)
        {
            Vector3 demandedPos = VRHeadset.position + VRHeadset.forward * FloatingTargetOffset.z + VRHeadset.right * FloatingTargetOffset.x + VRHeadset.up * FloatingTargetOffset.y;
            transform.position = Vector3.Lerp(transform.position, demandedPos, 5 * Time.deltaTime);
            transform.LookAt(VRHeadset.position);
            transform.Rotate(-90, 0, 180);
        }

        // Manipulator:
        if (manipulationActive)
        {
            if (chosenObject == null && (OVRInput.Get(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.P)))
            {
                RaycastHit[] hits = Physics.RaycastAll(RightController.transform.position, RightController.transform.forward, 10);

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.GetComponent<SyncObject>() != null || hit.collider.gameObject.GetComponentInParent<SyncObject>() != null)
                    {
                        Debug.Log(hit.collider.name);   
                        chosenObject = hit.collider.gameObject.GetComponent<SyncObject>();
                        if (chosenObject == null) chosenObject = hit.collider.gameObject.GetComponentInParent<SyncObject>();
                        if (chosenObject.tag == "Player") chosenObject = null;
                        else
                        {
                            manipulator.SetActive(true);
                            objectName.text = chosenObject.name;
                            break;
                        }
                    }
                }
            }
            if (chosenObject != null)
            {
                chosenObject.scaleFactor = scaleSlider.value;
                chosenObject.transform.Translate(new Vector3(posSliderX.value, posSliderY.value, posSliderZ.value)*Time.deltaTime);
                
                chosenObject.transform.Rotate(new Vector3(rotSliderX.value, rotSliderY.value, rotSliderZ.value)*1);
            }
        }
    }


    public void SliderReleased(Slider slid)
    {
        slid.value = 0;
    }
    public void ResetRotation()
    {
        if (chosenObject != null)
        {
            chosenObject.transform.rotation = chosenObject.initRotation;
        }
    }
    public void OpenApp (GameObject newApp)
    {
        foreach (GameObject app in Apps)
        {
            app.SetActive(false);
        }
        newApp.SetActive(true);
    }
    public void PlaceItem (int idx)
    {
        GameObject item = Items[idx];
        StartCoroutine(PlacingItemRoutine(item));

    }

    public void ToggleManipulationActive(bool mode)
    {
        manipulationActive = mode;
    }

    public void ExitButtonCallback()
    {
        chosenObject = null;
        manipulator.SetActive(false);
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
            debugText.text = "Added " + spawnItem.name;
            Debug.Log("Placed " + spawnItem.name + "Successfully");
            if (spawnItem.GetComponent<SyncObject>() != null)
            {
                spawnItem.GetComponent<SyncObject>().Init();
            }
                
        }
       
        yield return null;
    }


}
