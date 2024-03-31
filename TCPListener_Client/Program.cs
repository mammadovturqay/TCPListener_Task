using System;
using System.Net.Sockets;
using System.Text;

public class Client
{
    private const string SERVER_IP = "127.0.0.1";
    private const int PORT = 8080;

    static void Main(string[] args)
    {
        Console.Write("Adi daxil edin: ");
        string userName = Console.ReadLine();

        TcpClient client = new TcpClient(SERVER_IP, PORT);
        NetworkStream stream = client.GetStream();

        byte[] userNameBytes = Encoding.ASCII.GetBytes(userName);
        stream.Write(userNameBytes, 0, userNameBytes.Length);

        Console.WriteLine("Servera  isledi ");

        while (true)
        {
            Console.Write("Kime mesaj yazmaq isteyirsen ID  (0 dan basla ): ");
            int recipientID = int.Parse(Console.ReadLine());

            Console.Write("mesaj yaz ... : ");
            string message = Console.ReadLine();

            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            stream.Write(messageBytes, 0, messageBytes.Length);
        }
    }
}
