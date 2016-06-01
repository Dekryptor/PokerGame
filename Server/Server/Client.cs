using System;
using System.IO;
using System.Xml;
using System.Threading;
using System.Net.Sockets;

namespace Server
{
    class Client
    {
        XmlNode I;
        TcpClient socket;
        Thread thread;
        StreamReader sr;
        StreamWriter sw;
        string name;
        int money;
        int inroundmoney = 0;
        int position = 50;
        int hefresh = 0;
        Card[] pocket = new Card[2];

        public StreamWriter Writer
        {
            get { return sw; }
        }

        public StreamReader Reader
        {
            get { return sr; }
        }

        public Card[] Pocket
        {
            get { return pocket; }
            set { pocket = value; }
        }

        public int Money
        {
            get { return money; }
            set { this.money = value; }
        }

        public int InRoundMoney
        {
            get { return inroundmoney; }
            set { this.inroundmoney = value; }
        }

        public int Position
        {
            get { return position; }
            set { this.position = value; }
        }

        public int Hefresh
        {
            get { return hefresh; }
            set { this.hefresh = value; }
        }

        public string Name
        {
            get { return name; }
        }

        public Client(TcpClient Socket)
        {
            socket = Socket;
            sr = new StreamReader(socket.GetStream());
            sw = new StreamWriter(socket.GetStream());
            sw.AutoFlush = true;
            thread = new Thread(Login);
            thread.Start();
        }

        private void Login()
        {
            while (name == null)
            {
                try
                {
                    string data = sr.ReadLine();
                    string username = data.Substring(0, data.IndexOf('$'));
                    string password = data.Substring(data.IndexOf('$') + 1);
                    if ((I = ServerLobby.reader.SelectSingleNode("/Users/User[Username='" + username + "' and Password='" + password + "' and Online='0']")) != null)
                    {
                        sw.WriteLine("1");
                        this.name = username;
                        this.money = int.Parse(I.FirstChild.NextSibling.NextSibling.InnerText);
                        I.LastChild.InnerText = "1";
                        Lobby();
                    }
                    else
                    {
                        sw.WriteLine("0");
                        Thread.Sleep(5000);
                    }
                }
                catch
                {
                    Disconnect();
                }
            }
        }

        public void Disconnect()
        {
            if (name != null)
            {
                I.FirstChild.NextSibling.NextSibling.InnerText = money + "";
                I.LastChild.InnerText = "0";
            }
            sr.Close();
            sw.Close();
            socket.Close();
        }

        private void Lobby()
        {
            string rec;
            while ((rec = sr.ReadLine()) != "Exit$")
            {
                try
                {
                    LobbyRequest(rec);
                }
                catch
                {
                    Disconnect();
                }
            }
            Disconnect();
        }

        private void LobbyRequest(string a)
        {
            string[] command = new string[3];
            for (int i = 0; a.IndexOf('$') != -1; i++)
            {
                command[i] = a.Substring(0, a.IndexOf('$'));
                a = a.Remove(0, a.IndexOf('$') + 1);
            }
            if (command[0] == "List")
            {
                string tmp = null;
                int i = 0;
                foreach (Table n in ServerLobby.Tables)
                {
                    tmp += i + "$" + n.ToString() + "@";
                    i++;
                }
                sw.WriteLine(tmp);
            }
            else if (command[0] == "Spectate")
                ServerLobby.Tables[int.Parse(command[1])].Spectate(this);
            else if (command[0] == "Money")
                sw.WriteLine(money);
        }
    }
}