using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandlePackets : MonoBehaviour
{

    private delegate void _packet(byte[] data);
    private static Dictionary<int, _packet> _packets;

    public static ByteBuffer PlayerBuffer;
    private static int _packetSize;

    private void Awake()
    {
        InitPackets();
    }

    private static void InitPackets()
    {
        _packets = new Dictionary<int, _packet>();
        _packets.Add((int) ServerPackets.P_TEST, Handle_P_TEST);
        _packets.Add((int) ServerPackets.P_TEST_METHOD, Handle_P_TEST_METHOD);
    }
    
    public static void HandleData(byte[] data)
    {
        byte[] buffer;
        buffer = (byte[]) data.Clone();

        if (PlayerBuffer == null)
        {
            PlayerBuffer = new ByteBuffer();
        }
        
        PlayerBuffer.WriteBytes(buffer);

        if (PlayerBuffer.GetCount() == 0)
        {
            // Package is empty so don't allow code to proceed any further
            PlayerBuffer.Clear();
            return;
        }

        if (PlayerBuffer.GetLength() >= sizeof(int))
        {
            // Read the server command
            _packetSize = PlayerBuffer.ReadInteger(false);
            if (_packetSize <= (int) ServerPackets.P_MIN || _packetSize >= (int) ServerPackets.P_MAX)
            {
                // Invalid command or packet isn't complete
                PlayerBuffer.Clear();
                return;
            }
                
        }
        
        // Package has made it past validation so loop through and get the information
        while (_packetSize > 0 && _packetSize <= PlayerBuffer.GetLength() - sizeof(int))
        {

            if (_packetSize <= PlayerBuffer.GetLength() - sizeof(int))
            {
                // Get the packet identifier
                PlayerBuffer.ReadInteger();
            
                // Read in the data to bytes
                data = PlayerBuffer.ReadBytes((int) _packetSize);
            
                // Handle the packet data
                HandleDataPacket(data);
            }

            _packetSize = 0;

            if (PlayerBuffer.GetLength() >= sizeof(int))
            {
                _packetSize = PlayerBuffer.ReadInteger(false);
                if (_packetSize < 0)
                {
                    PlayerBuffer.Clear();
                    break;
                }
            }
        }


    }

    private static void HandleDataPacket(byte[] data)
    {
        int packetID;
        ByteBuffer buffer;
        _packet packet;

        // Write the data (which will contain the packet identifier) to the buffer and then read the packet ID
        buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        packetID = buffer.ReadInteger();
        buffer.Dispose();
        
        
        // Run the method from the packet identifier
        if (_packets.TryGetValue(packetID, out packet))
        {
            packet.Invoke(data);
        }
        
    }

    private static void Handle_P_TEST(byte[] data)
    {
        Debug.Log("Test has been invoked");
    }

    private static void Handle_P_TEST_METHOD(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger(); // Clear the packet ID
        
        // Receives one string 
        string msg = buffer.ReadString();
        Debug.Log(msg);
        
        buffer.Dispose();
        
    }
}
