using System;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

public class TCPServer
{
    
    // Default port. No real significance
    public const int Port = 27783;
    public const int MaxConnections = 4;
    
    private TcpListener _serverSocket;
    private Client[] _connectedClients = new Client[MaxConnections];

    /// <summary>
    /// Initializes the network and begins listening for connections
    /// </summary>
    /// <param name="port">Port to open the server with</param>
    public void InitNetwork(int port = Port)
    {
        for (int i = 0; i < _connectedClients.Length; i++)
        {
            _connectedClients[i] = new Client();
        }
        
        // Listen to connections from any IP address
        _serverSocket = new TcpListener(IPAddress.Any, Port);
        
        // Begin listening for connections
        _serverSocket.Start();
        _serverSocket.BeginAcceptTcpClient(ClientConnected, null);
    }

    /// <summary>
    /// Callback for accepting client connections
    /// </summary>
    /// <param name="result">Async result</param>
    private void ClientConnected(IAsyncResult result)
    {
        // Create temporary client and accept connection
        TcpClient client = _serverSocket.EndAcceptTcpClient(result);

        for (int i = 0; i < _connectedClients.Length; i++)
        {
            // Check for open spot on server
            if (_connectedClients[i].IsSocketEmpty()) continue;
            
            // Spot is open (no socket assigned) so assign new client to spot
            string clientIP = client.Client.RemoteEndPoint.ToString();
            _connectedClients[i].Start(client, i, clientIP);
                
            Debug.Log("Connection received from " + _connectedClients[i].IPAddress +
                              " with ID of " + _connectedClients[i].ConnectionID);

            
            SendInformation(_connectedClients[i].ConnectionID);
            
            break;
        }
        
        // Allow for further connections
        _serverSocket.BeginAcceptTcpClient(ClientConnected, null);
    }

    public void SendDataTo(int connectionID, byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        
        buffer.WriteInt((int) ServerPackets.P_TEST);
        buffer.WriteLong((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
        buffer.WriteBytes(data);
        
        // Send data to client
        _connectedClients[connectionID].Stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);

    }

    // Send methods to clients
    public void SendInformation(int connectionID)
    {
        // Create and add data to packet
        ByteBuffer buffer = new ByteBuffer(); 
        
        // Add data to the packet
        buffer.WriteInt((int) ServerPackets.P_TEST_METHOD);
        buffer.WriteString("Hello World");
        
        SendDataTo(connectionID, buffer.ToArray());
    }
}
