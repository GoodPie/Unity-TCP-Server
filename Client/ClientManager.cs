using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(TCPClient))]
public class ClientManager : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public int port = TCPServer.Port;
    
    void Start()
    {
        DontDestroyOnLoad(this);
        GetComponent<TCPClient>().Connect(ip, port);
    }

    private void OnApplicationQuit()
    {
        GetComponent<TCPClient>().CloseConnection();
    }
}
