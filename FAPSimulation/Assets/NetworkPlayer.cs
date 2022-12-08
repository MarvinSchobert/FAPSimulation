using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class NetworkPlayer : SyncObject
{

    public JObject stats;
    public int health;
    public int score;

    public Transform rightHand;
    public Transform leftHand;

    public void Start()
    {
        stats = new JObject();
        stats["health"] = health;
        stats["score"] = score;
    }

    public bool canBeDestroyed;
    public void AddDamage(int value)
    {
        Debug.Log("Damage");
        health -= value;
        stats["health"] = health;
        if (health <= 0 && canBeDestroyed)
        {
            // Löschen
            JObject obj = new JObject();
            obj["type"] = "RemoveRqt";
            obj["ID"] = ID;
            GameManager.sender.SpawnObjectRequest(obj);
        }
        else
        {
            JObject obj = new JObject();
            obj["type"] = "SyncVarRqt";
            obj["ID"] = ID;
            obj["stats"] = stats;
            GameManager.sender.SpawnObjectRequest(obj);
        }
    }

    public void changeHandTransformPositions(Vector3 posR, Vector3 posL) {
        rightHand.transform.position = posR;
        leftHand.transform.position = posL;
    }

    public void AddScore(int value)
    {
        score += value;
        stats["score"] = score;
        JObject obj = new JObject();
        obj["type"] = "SyncVarRqt";
        obj["ID"] = ID;
        obj["stats"] = stats;
        GameManager.sender.SpawnObjectRequest(obj);
    }
}
