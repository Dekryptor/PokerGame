using System;
using System.IO;
using System.Net.Sockets;

namespace Client
{
    static class I
    {
        static TcpClient server = new TcpClient();
        static StreamWriter sw;
        static StreamReader sr;
        static string name;
        static int money;

        public static void Initialize()
        {
            server.Connect(System.Net.IPAddress.Parse("127.0.0.1"), 55555);
            sw = new StreamWriter(server.GetStream());
            sw.AutoFlush = true;
            sr = new StreamReader(server.GetStream());
        }

        public static string Name
        {
            get { return name; }
            set { name = value; }
        }

        public static int Money
        {
            get { return money; }
            set { money = value; }
        }

        public static void Write(string a)
        {
            sw.WriteLine(a);
        }

        public static string Read()
        {
            return (sr.ReadLine());
        }
    }
}
