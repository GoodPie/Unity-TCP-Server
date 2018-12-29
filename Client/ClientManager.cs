using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ClientManager : MonoBehaviour
{

    private TCPClient _client;

    public string ip = "127.0.0.1";
    public int port = TCPServer.Port;
    
    void Start()
    {
        DontDestroyOnLoad(this);
        _client = new TCPClient(ip, port);
        _client.Connect();
    }

    private void OnApplicationQuit()
    {
        _client.CloseConnection();
    }
}
