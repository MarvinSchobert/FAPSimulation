using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class WeaponScript : MonoBehaviour
{
    public bool isMounted;
    public GameObject bullet;
    public GameMangagement GameManager;
    // Start is called before the first frame update
    public void ObjectGrabbed (bool mode)
    {
        isMounted = mode;
    }

    public void Start()
    {
        GameManager = GameObject.FindWithTag("GameManagement").GetComponent<GameMangagement>();
    }

    private bool isShooting;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isMounted = true;
        }
        if (isMounted)
        {
            if (!isShooting && (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.5f || Input.GetKeyDown(KeyCode.P)))
            {
                isShooting = true;
                StartCoroutine(Shoot());
            }
            else if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) < 0.5f || Input.GetKeyUp(KeyCode.P)) {
                isShooting = false;
            }
        }
    }

    IEnumerator Shoot()
    {
        GameObject _bullet = Instantiate(bullet, transform.position + transform.forward*0.1f, transform.rotation);
        _bullet.GetComponent<SyncObject>().Init();

        // Bullet fliegt x Sekunden weit
        float timer = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - timer < 6)
        {
            if (_bullet == null) break;
            _bullet.transform.Translate(Vector3.forward * Time.deltaTime * 15);
            Collider[] cols = Physics.OverlapSphere(_bullet.transform.position, 0.02f);
            bool destroy = false;
            foreach (Collider col in cols)
            {
               
                if (col.gameObject != gameObject) {
                    Debug.Log(col.name);
                    col.SendMessage("AddDamage", 5);
                    destroy = true; 
                }


            }
            if (destroy)
            {
                // Objekt wieder löschen
                JObject obj = new JObject();
                obj["type"] = "RemoveRqt";
                obj["name"] = bullet.GetComponent<SyncObject>().name;
                obj["prefabName"] = bullet.GetComponent<SyncObject>().prefabName;
                obj["ID"] = _bullet.GetComponent<SyncObject>().ID;
                obj["syncInfoString"] = bullet.GetComponent<SyncObject>().SyncInfoString;
                GameManager.sender.SpawnObjectRequest(obj);
            }
           
            yield return null;
        }

        if (_bullet != null)
        {
            // Objekt wieder löschen
            JObject obj = new JObject();
            obj["type"] = "RemoveRqt";
            obj["name"] = bullet.GetComponent<SyncObject>().name;
            obj["prefabName"] = bullet.GetComponent<SyncObject>().prefabName;
            obj["ID"] = _bullet.GetComponent<SyncObject>().ID;
            obj["syncInfoString"] = bullet.GetComponent<SyncObject>().SyncInfoString;
            GameManager.sender.SpawnObjectRequest(obj);
        }
    }
}
