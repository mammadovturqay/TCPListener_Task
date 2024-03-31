using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Server
{
    private static Dictionary<int, string> users = new Dictionary<int, string>();
    private static TcpListener listener;
    private const int PORT = 8080;
    private static List<NetworkStream> clientStreams = new List<NetworkStream>();

    static void Main(string[] args)
    {
        listener = new TcpListener(IPAddress.Any, PORT);
        listener.Start();
        Console.WriteLine("Server running...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        string userName = "";

        try
        {
            // Prompt user for their name
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            userName = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            users[Thread.CurrentThread.ManagedThreadId] = userName;
            Console.WriteLine(userName + " connected.");
            clientStreams.Add(stream); // Add new client's stream for broadcasting

            while (true)
            {
                // Read message from client
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine(userName + ": " + message);

                // Broadcast message to other clients
                byte[] messageBytes = Encoding.ASCII.GetBytes(userName + ": " + message);
                foreach (var clientStream in clientStreams)
                {
                    clientStream.Write(messageBytes, 0, messageBytes.Length);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(userName + " disconnected.");
            users.Remove(Thread.CurrentThread.ManagedThreadId);
            clientStreams.Remove(stream); // Remove stream when user leaves
        }
        finally
        {
            stream.Close();
            client.Close();
        }
    }
}
