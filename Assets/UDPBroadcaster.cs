using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPBroadcaster : MonoBehaviour
{
    private UdpClient udpClient;
    private int port = 1234;
    private string broadcastMessage = "Hello from Unity!";
    private IPEndPoint endPoint;

    void Start()
    {
        // Initialize the UdpClient
        udpClient = new UdpClient();

        // Setup the end point for broadcast to all (255.255.255.255)
        endPoint = new IPEndPoint(IPAddress.Broadcast, port);

        // Enable broadcast on the client
        udpClient.EnableBroadcast = true;

        // Start broadcasting
        InvokeRepeating("BroadcastMessage", 1.0f, 1.0f); // Send every 1 second
    }

    void BroadcastMessage()
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(broadcastMessage);
            udpClient.Send(data, data.Length, endPoint);
            Debug.Log("Broadcasting message: " + broadcastMessage);
        }
        catch (SocketException e)
        {
            Debug.LogError("Error broadcasting message: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        udpClient.Close(); // Close the UDP client when the application quits
    }
}
