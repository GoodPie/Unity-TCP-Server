
using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    
    public const string LocalIP = "127.0.0.1";
    public const int DefaultPort = TCPServer.Port;
    public const int BufferSize = 4096;

    private TcpClient _client;
    private NetworkStream _stream;

    private byte[] _receivedBytes;
    private byte[] _asyncBuffer;
    
    public bool IsConnected;
    public bool HandlingData = false;

    private string _IPAddress;
    private int _port;

    private void Update()
    {
        if (HandlingData)
        {
            ClientHandlePackets.HandleData(_receivedBytes);
            HandlingData = false;
        }
    }
    
    /// <summary>
    /// Initialize the connection to the server
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void Connect(string ip = LocalIP, int port = DefaultPort)
    {
        _client = new TcpClient();
        _IPAddress = ip;
        _port = port;
        
        Debug.Log("Attempting to connect to " + _IPAddress + ":" + _port);

        // Initialize the buffer
        _client = new TcpClient();
        _client.ReceiveBufferSize = BufferSize;
                                                   _client.SendBufferSize = BufferSize;
        _asyncBuffer =  new byte[BufferSize * 2];
        
        // Begin listening for connections
        _client.BeginConnect(_IPAddress, _port, new AsyncCallback(OnConnect), _client);

    }

    /// <summary>
    /// Callback for connection attempt
    /// </summary>
    /// <param name="result">async result</param>
    private void OnConnect(IAsyncResult result)
    {
        try
        {
            _client.EndConnect(result);

            if (_client.Connected == false)
            {
                // Invalid connection
                IsConnected = false;
                return;
            }
            else
            {
                // Connected successfully so initialize the reading of data
                _stream = _client.GetStream();
                _stream.BeginRead(_asyncBuffer, 0, BufferSize * 2, OnReceiveData, null);
                IsConnected = true;
                
                Debug.Log("Connected to the server successfully");
            }
            
        }
        catch (Exception e)
        {
            // Connection has failed for whatever reason
            IsConnected = false;
            Debug.Log("Failed to connect to the server");
            Console.WriteLine(e);
        }
    }


    /// <summary>
    /// Callback for data receiving. Handles how data is handled :)
    /// </summary>
    /// <param name="result">async result</param>
    private void OnReceiveData(IAsyncResult result)
    {
        try
        {
            // Get length of packet and stop reading packet
            int packetSize = _stream.EndRead(result);
            
            // Resize the byte array to receive the rest of the data
            _receivedBytes = new byte[packetSize];
            
            Buffer.BlockCopy(_asyncBuffer, 0, _receivedBytes, 0, packetSize);

            if (packetSize == 0)
            {
                // No information received from the server
                Debug.Log("Disconnected from the server");
                Application.Quit();
                return;
            }

            HandlingData = true;
            ClientHandlePackets.HandleData(_receivedBytes);
            _stream.BeginRead(_asyncBuffer, 0, BufferSize * 2, OnReceiveData, null);

        }
        catch (Exception e)
        {
            // No information received from the server
            Debug.Log(e);
            Debug.Log("Disconnected from the server");
            Application.Quit();
            Console.WriteLine(e);
            return;
        }
    }

    /// <summary>
    /// Helper function to close the client without client.client.client.client
    /// </summary>
    public void CloseConnection()
    {
        _client.Close();
    }

}
