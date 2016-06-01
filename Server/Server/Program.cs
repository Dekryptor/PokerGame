using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(55555);
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine(" >> " + "Server Started");
            ServerLobby.Initialize();
            ServerLobby.reader.Load(@"C:\Users\Богдан\Desktop\Poker\Server\Server\Players.xml");

            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                Client client = new Client(clientSocket);
            }
        }
    }
}
