using System;
using System.Net.Sockets;

public class Client
{
    // Default size of the buffer
    public const int BufferSize = 4096;
    
    public int ConnectionID;
    public string IPAddress;
    public TcpClient Socket;
    public NetworkStream Stream;

    private int _maxNoReadFailures = 10;
    private int _currentFailures = 0;

    private byte[] _readBuffer;

    /// <summary>
    /// Begin listening on the client
    /// </summary>
    public void Start()
    {
        // Initialize the buffer
        _readBuffer = new byte[BufferSize];
        Socket.SendBufferSize = BufferSize;
        Socket.ReceiveBufferSize = BufferSize;

        // Setup stream
        Stream = Socket.GetStream();
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
                _currentFailures += 1;
                if (_currentFailures >= _maxNoReadFailures)
                {
                    CloseConnection();
                }
                // Didn't actually get any information from the client
                return;
            }

            _currentFailures = 0;
            
            // Read the data
            byte[] bytes = new byte[readSize];
            Buffer.BlockCopy(_readBuffer, 0, bytes, 0, readSize);

            // Continue listening for packages
            Stream.BeginRead(_readBuffer, 0, BufferSize, ReceiveDataCallback, null);
            
        }
        catch (Exception e)
        {
            CloseConnection();
            throw;
        }
    }

    /// <summary>
    /// Close connection to server
    /// </summary>
    private void CloseConnection()
    {
        Console.WriteLine("Connection from {0} has been closed", IPAddress);
        Socket.Close();
    }
    
    
}
