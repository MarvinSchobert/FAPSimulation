/*
 
    -----------------------
    UDP-Send
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
    // > gesendetes unter
    // 127.0.0.1 : 8050 empfangen
   
    // nc -lu 127.0.0.1 8050
 
        // todo: shutdown thread at the end
*/
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

public class UDPSend : MonoBehaviour
{
    private static int localPort;

    private string IP;  // define in init
    public int port;  // define in init

    IPEndPoint remoteEndPoint;
    UdpClient client;
    public TMPro.TMP_Text UiText;
    public Text UiText_Normal;


    public void Start()
    {
        init();
    }
   
    public void init()
    {
        IP = "192.168.137.1";
        //IP = "127.0.0.1";
        port = 33333;

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
        RegisterRequest();
    }
    public void RegisterRequest()
    {
        JObject obj = new JObject();
        obj["type"] = "RegisterRqt";
        obj["name"] = "Marvin Schobert";
        obj["port"] = "5555";
        if (UiText != null) UiText.text = "Send Message to Server";
        else if (UiText_Normal != null) UiText_Normal.text = "Send Message to Server";
        _sendObject(obj);
    }
    public void OnFinishedProductionTask (string taskId)
    {
        JObject obj = new JObject();
        obj["taskId"] = taskId;
        obj["type"] = "FinishedProdRqt";
        _sendObject(obj);
    }
    public void SpawnObjectRequest(JObject obj)
    {
        _sendObject(obj);
    }


    public void ChangeObjectRequest() {
        JObject obj = new JObject();
        obj["type"] = "ChangeRqt";        
        _sendObject(obj);
    }

    public void UnRegisterOwnClient()
    {
        JObject obj = new JObject();
        obj["type"] = "DeleteClientRqt";
        _sendObject(obj);
    }


    // sendData
    private void _sendObject(JObject obj)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(obj.ToString());
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }
       

}

