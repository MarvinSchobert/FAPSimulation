using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class RessourceManager : MonoBehaviour
{
    public string RessourceID;
    public float processTime = 5.0f;
    public Transform input;
    public Transform output;
    public SimulationsManager simManager;
    public JObject currentTask;


    List<Product> batch = new List<Product>();
    /// <summary>
    /// Wie viele Teile auf einmal maximal bearbeitet werden können
    /// </summary>
    public int maxLoadAmount;

    public enum RessourceState
    {
        Idle, Processing, Broken, Preparing
    }
    public RessourceState state = RessourceState.Idle;

    private void Start()
    {
        if (processTime == 0)
        {
            processTime = 5.0f;
        }
        StartCoroutine(Routine());
    }

    public void HandOverProducts(List<Product> products)
    {
        foreach (Product p in products)
        {
            batch.Add(p);
        }
    }


    public IEnumerator Routine()
    {
        

        while (true)
        {
            if (simManager == null || simManager.GameManager == null || !simManager.GameManager.isClientLeader)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            switch (state)
            {
                case RessourceState.Idle:
                    break;
                case RessourceState.Processing:

                    
                    while (state == RessourceState.Processing)
                    {
                        // Zuerst entsprechende Waren entnehmen, falls da.
                        Debug.Log("Working on Task:\n" + currentTask.ToString());
                        List <Product> p = new List<Product>();
                        for (int i = 0; i < ((JArray)currentTask["inputProducts"]).Count; i++)
                        {
                            
                            // Konkrete ProduktID eingeplant
                            if (((JArray)currentTask["inputProducts"])[i]["produktId"].ToString() != "")
                            {
                                Debug.Log("Need to process Item " + ((JArray)currentTask["inputProducts"])[i]["produktId"].ToString());
                                // zuerst schauen, ob das Objekt schon nahe am Input liegt:
                                Collider[] cols = Physics.OverlapSphere(input.position, 2.0f);
                                bool success = false;
                                foreach (Collider col in cols)
                                {
                                    if (col.GetComponent<Product>() != null && col.GetComponent<Product>().productID == ((JArray)currentTask["inputProducts"])[i]["produktId"].ToString())
                                    {
                                        Debug.Log("Input item is close to input");
                                        p.Add (col.GetComponent<Product>());
                                        success = true;
                                        break;
                                    }
                                }
                                if (!success)
                                {
                                    for (int j = 0; j < simManager.products.Count; j++)
                                    {

                                        if (simManager.products[j].productID == ((JArray)currentTask["inputProducts"])[i]["produktId"].ToString())
                                        {
                                            Debug.Log("Material ist weiter weg und muss angefragt werden.");
                                            p.Add(simManager.products[j]);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (((JArray)currentTask["inputProducts"])[i]["materialstamm"].ToString() != "")
                            {
                                Debug.Log("Need to process Item " + ((JArray)currentTask["inputProducts"])[i]["materialstamm"].ToString());
                                // zuerst schauen, ob das Objekt schon nahe am Input liegt:
                                Collider[] cols = Physics.OverlapSphere(input.position, 2.0f);
                                bool success = false;
                                foreach (Collider col in cols)
                                {
                                    if (col.GetComponent<Product>() != null && col.GetComponent<Product>().artikelStammNummer == ((JArray)currentTask["inputProducts"])[i]["materialstamm"].ToString())
                                    {
                                        Debug.Log("Input item is close to input");
                                        p.Add(col.GetComponent<Product>());
                                        success = true;
                                        break;
                                    }
                                }
                                // ist nicht in der Nähe, sondern muss von weiter weg bezogen werden (--> ne Art Kanban?)
                                if (!success)
                                {                                   
                                    for (int j = 0; j < simManager.products.Count; j++)
                                    {
                                        if (simManager.products[j].productID == "" && simManager.products[j].artikelStammNummer == ((JArray)currentTask["inputProducts"])[i]["materialstamm"].ToString())
                                        {
                                            Debug.Log("Material ist weiter weg und muss angefragt werden.");
                                            p.Add(simManager.products[j]);                                            
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    
                                }
                            }
                        }
                        
                        if (p.Count != ((JArray)currentTask["inputProducts"]).Count)
                        {
                            Debug.Log("Input Products not available!");
                            yield return new WaitForSeconds(1.0f);
                        }
                        else
                        {
                            yield return simManager.UpdateTaskStatus(currentTask, "processed");
                            while (p.Count != 0)
                            {
                                for (int i = 0; i < p.Count; i++)
                                {
                                    if (Vector3.SqrMagnitude(p[i].transform.position - input.position) > 0.005f)
                                    {
                                        p[i].transform.position = Vector3.Lerp(p[i].transform.position, input.position, Time.deltaTime);                                        
                                    }
                                    else
                                    {                                     
                                        simManager.products.Remove(p[i]);
                                        Destroy(p[i].gameObject);
                                        p.Remove(p[i]);
                                        break;
                                    }                                    
                                }
                                yield return null;
                            }
                            p = new List<Product>();
                            // Jetzt Waren hinzufügen
                            for (int i = 0; i < ((JArray)currentTask["outputProducts"]).Count; i++)
                            {
                                Debug.Log("Will produce Item " + ((JArray)currentTask["outputProducts"])[i].ToString());
                                // Instanz von diesem Produkt hinzufügen
                                GameObject ng = simManager.CreateNewProduct((JObject)((JArray)currentTask["outputProducts"])[i]);
                                // Debug.Log("FYI: " + name + ", " + ng.name);
                                ng.transform.position = input.position; // TODO: Hier gibts manchmal Exception, dass Objekt schon gelöscht wurde
                                p.Add (ng.GetComponent<Product>());
                            }
                            while (p.Count != 0)
                            {
                                for (int i = 0; i < p.Count; i++)
                                {
                                    if (Vector3.SqrMagnitude(p[i].transform.position - output.position) > 0.005f)
                                    {
                                        p[i].transform.position = Vector3.Lerp(p[i].transform.position, output.position, Time.deltaTime);
                                    }
                                    else
                                    {
                                        p.Remove(p[i]);
                                        break;
                                    }
                                }
                                yield return null;
                            }
                            state = RessourceState.Idle;
                        }
                        yield return null;
                    }
                    yield return simManager.UpdateTaskStatus(currentTask, "complete");
                    simManager.GameManager.sender.OnFinishedProductionTask(currentTask["taskID"].ToString());
                    break;
                case RessourceState.Broken: 
                    break;
                default:
                    break;
            }
            yield return null;
        }
    }
}
