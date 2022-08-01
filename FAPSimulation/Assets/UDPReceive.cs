
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

    JObject data;
    bool dataReady;

    public void Start()
    {
        receivingUdpClient = new UdpClient(5555);

        RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        receiveThread = new Thread(
           new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }
    public void Update()
    {
        if (dataReady)
        {
            if (data["type"].ToString() == "SpawnInfo")
            {
                GameManager.SpawnObjectCallback(data);
            }

            if (data["type"].ToString() == "ChangeInfo")
            {
                GameManager.ChangeObjectCallback(data);
            }
            dataReady = false;
        }
    }

    private void ReceiveData()
    {
        while (true)
        {
            try
            {

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                string returnData = Encoding.ASCII.GetString(receiveBytes);
                Debug.Log("This is the message you received " + returnData);
                
                
                JObject obj = JObject.Parse(returnData);
                data = obj;
                
                while (dataReady) { }
                dataReady = true;
               
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
}
