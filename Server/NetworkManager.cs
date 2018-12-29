using UnityEngine;

public class NetworkManager : MonoBehaviour
{

    public TCPServer Server;
    
    void Start()
    {
        // Make sure the object persists throughout scenes
        DontDestroyOnLoad(this);
        
        // Initialize the network
        Server = new TCPServer();
        Server.InitNetwork();
    }

}

