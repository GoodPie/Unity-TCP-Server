using System;
using System.Net.Sockets;

public class Client
{
    // Default size of the buffer
    public const int BufferSize = 4096;
    public const int MaxReadFailures = 10;

    private int _connectionID;
    public int ConnectionID => _connectionID;
    
    private string _ipAddress;
    public string IPAddress => _ipAddress;
    private TcpClient _socket;
    public NetworkStream Stream;
    private byte[] _readBuffer;
    private int _currentFailures = 0;


    /// <summary>
    /// Begin listening on the client
    /// </summary>
    public void Start(TcpClient socket, int connectionID, string ip)
    {
        // Initialize client values 
        _socket = socket;
        _connectionID = connectionID;
        _ipAddress = ip;
        
        // Initialize the buffer
        _readBuffer = new byte[BufferSize];
        _socket.SendBufferSize = BufferSize;
        _socket.ReceiveBufferSize = BufferSize;

        // Setup stream
        Stream = _socket.GetStream();
        Stream.BeginRead(_readBuffer, 0, BufferSize, ReceiveDataCallback, null);
    }

    /// <summary>
    /// Callback to handle reading data
    /// </summary>
    /// <param name="result">Async result</param>
    private void ReceiveDataCallback(IAsyncResult result)
    {
        try
        {
            int readSize = Stream.EndRead(result);
            
            if (readSize <= 0)
            {
                // Didn't actually get any information from the client meaning we have disconnected
                _currentFailures += 1;
                if (_currentFailures >= MaxReadFailures)
                {
                    // We haven't received any messages for a while so close the connection
                    CloseConnection();
                }
                
                return;
            }
            else
            {
                _currentFailures = 0;
            }

            // Read the data
            byte[] bytes = new byte[readSize];
            Buffer.BlockCopy(_readBuffer, 0, bytes, 0, readSize);

            // Continue listening for packages
            Stream.BeginRead(_readBuffer, 0, BufferSize, ReceiveDataCallback, null);
            
        }
        catch (Exception e)
        {
            // Exception occured so close the connection
            CloseConnection();
            throw;
        }
    }

    /// <summary>
    /// Close connection to server
    /// </summary>
    public void CloseConnection()
    {
        Console.WriteLine("Connection from {0} has been closed", _ipAddress);
        _socket.Close();
    }

    /// <summary>
    /// Helper function to check if the socket is empty
    /// </summary>
    /// <returns></returns>
    public bool IsSocketEmpty()
    {
        return _socket == null;
    }
    
}
