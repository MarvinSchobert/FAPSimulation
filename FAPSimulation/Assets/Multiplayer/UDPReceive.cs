
/*
 
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 127.0.0.1 : 8051
   
    // send
    // nc -u 127.0.0.1 8051
 
*/
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
    public GameMangagement GameManager;
    //Creates a UdpClient for reading incoming data.
    UdpClient receivingUdpClient;

    //Creates an IPEndPoint to record the IP Address and port number of the sender.
    // The IPEndPoint will allow you to read datagrams sent from any source.
    IPEndPoint RemoteIpEndPoint;
    Thread receiveThread;

    List <JObject> data;
    bool dataReady;

    public void Start()
    {
        data = new List<JObject>();

       
        receiveThread = new Thread(
           new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }
    public void Update()
    {
        if (data.Count > 0)
        {
            processData(0);
        }
        
       
    }

    void processData(int idx)
    {
        if (data[idx]["type"].ToString() == "SpawnInfo")
        {
            GameManager.SpawnObjectCallback(data[idx]);
        }

        if (data[idx]["type"].ToString() == "ChangeInfo")
        {
            GameManager.ChangeObjectCallback(data[idx]);
        }

        if (data[idx]["type"].ToString() == "InformClientLeadership")
        {
            GameManager.isClientLeader = true;
        }

        if (data[idx]["type"].ToString() == "RemoveInfo")
        {
            GameManager.RemoveObjectCallback(data[idx]);
        }
        if (data[idx]["type"].ToString() == "SyncVarInfo")
        {
            GameManager.SyncVarCallback(data[idx]);
        }
        data.RemoveAt(idx);
    }

    private void ReceiveData()
    {        
        receivingUdpClient = new UdpClient(5555);
        RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            try
            {
                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                string returnData = Encoding.ASCII.GetString(receiveBytes);

                JObject obj = JObject.Parse(returnData);
                if (data.Count > 10 && obj["type"].ToString() != "ChangeInfo")
                {
                    // Wenn zu viele Nachrichten eingehen, die ChangeInfos ignorieren. Alle anderen werden weiterhin berücksichtigt.
                } else
                {
                    data.Add(obj);
                }
               
               
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        receivingUdpClient.Close();
    }

    public void OnApplicationQuit()
    {
        receivingUdpClient.Close();
        receiveThread.Abort();
    }

}
