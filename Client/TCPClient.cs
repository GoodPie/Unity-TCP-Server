
using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class TCPClient
{
    
    public const string LocalIP = "127.0.0.1";
    public const int DefaultPort = TCPServer.Port;
    public const int BufferSize = 4096;

    private TcpClient _client;
    private NetworkStream _stream;
    private byte[] _asyncBuffer;
    
    public bool IsConnected;

    private string _IPAddress;
    private int _port;

    /// <summary>
    /// Default constructor to define the IP address and Port to connect to
    /// </summary>
    /// <param name="IP">IP Address of server</param>
    /// <param name="port">Port of server</param>
    public TCPClient(string IP, int port)
    {
        _client = new TcpClient();
        _IPAddress = IP;
        _port = port;
    }
    
    /// <summary>
    /// Initialize the connection to the server
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void Connect(string ip = LocalIP, int port = DefaultPort)
    {
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
                return;
            }
            else
            {
                // Connected successfully so initialize the reading of data
                _stream = _client.GetStream();
                _stream.BeginRead(_asyncBuffer, 0, BufferSize * 2, OnReceiveData, null);
                IsConnected = false;
                
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
            // TODO: Implement data reading
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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
