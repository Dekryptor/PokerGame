using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Client
{
    public partial class Table : Form
    {
        delegate void VoidDelegate(string a);
        VoidDelegate ProcessDelegate;
        Player[] players = new Player[8];
        Thread Listener;
        Lobby lobby;
        int button;
        int bb;
        public Table(Lobby lobby)
        {
            this.lobby = lobby;
            InitializeComponent();
            players[0] = new Player(249, 60);
            players[1] = new Player(87, 126);
            players[2] = new Player(87, 309);
            players[3] = new Player(249, 373);
            players[4] = new Player(483, 373);
            players[5] = new Player(667, 309);
            players[6] = new Player(667, 126);
            players[7] = new Player(483, 60);
            for (int i = 0; i < players.Length; i++)
            {
                this.Controls.Add(players[i].NameAsControl);
                this.Controls.Add(players[i].MoneyAsControl);
                this.Controls.Add(players[i].Pocket0);
                this.Controls.Add(players[i].Pocket1);
                this.Controls.Add(players[i].Button);
                this.Controls.Add(players[i].Action);
            }
            string p = I.Read();
            for (int i = 0; p.IndexOf('@') != -1; i++)
            {
                Process(p.Substring(0, p.IndexOf('@')));
                p = p.Remove(0, p.IndexOf('@') + 1);
            }
            string h = I.Read();
            for (int i = 0; h.IndexOf('@') != -1; i++)
            {
                Process(h.Substring(0, h.IndexOf('@')));
                h = h.Remove(0, h.IndexOf('@') + 1);
            }
            ProcessDelegate = new VoidDelegate(Process);
            Listener = new Thread(Listen);
            Listener.Start();
        }

        public void Listen()
        {
            string rec;
            while ((rec = I.Read()) != "Removed$")
                try
                {
                    Invoke(ProcessDelegate, rec);
                }
                catch
                {
                }
        }

        public void Process(string a)
        {
            string[] command = new string[6];
            for (int i = 0; a.IndexOf('$') != -1; i++)
            {
                command[i] = a.Substring(0, a.IndexOf('$'));
                a = a.Remove(0, a.IndexOf('$') + 1);
            }
            if (command[0] == "Joined") // Joined$position$name$money$
            {
                int pos = int.Parse(command[1]);
                players[pos].Name = command[2];
                players[pos].Money = int.Parse(command[3]);
            }
            else if (command[0] == "Sitting") // Sitting$position$name$money$
            {
                int pos = int.Parse(command[1]);
                players[pos].Name = command[2];
                players[pos].Money = int.Parse(command[3]);
                players[pos].Pocket0.Show();
                players[pos].Pocket1.Show();
            }
            else if (command[0] == "Left") // Left$position$
            {
                int pos = int.Parse(command[1]);
                players[pos].Name = "Open";
                players[pos].MoneyAsControl.Text = "Seat";
                players[pos].Pocket0.Hide();
                players[pos].Pocket1.Hide();
                players[pos].Action.Hide();
            }
         
            else if (command[0] == "Pocket") // Pocket$id$number(2-14)$shape(1-4)$
            {
                Image Card = Image.FromFile("Data\\" + int.Parse(command[2]) + "_" + int.Parse(command[3]) + ".gif");
                if (command[1] == "0")
                {
                    Pocket0.BackgroundImage = Card;
                    Pocket0.Show();
                }
                else
                {
                    Pocket1.BackgroundImage = Card;
                    Pocket1.Show();
                }
            }
            else if (command[0] == "Community") // Community$id$number(2-14)$shape(1-4)$
            {
                Image Card = Image.FromFile("Data\\" + int.Parse(command[2]) + "_" + int.Parse(command[3]) + ".gif");
                switch (command[1])
                {
                    case "0":
                        Community0.BackgroundImage = Card;
                        Community0.Show();
                        break;
                    case "1":
                        Community1.BackgroundImage = Card;
                        Community1.Show();
                        break;
                    case "2":
                        Community2.BackgroundImage = Card;
                        Community2.Show();
                        break;
                    case "3":
                        Community3.BackgroundImage = Card;
                        Community3.Show();
                        break;
                    default:
                        Community4.BackgroundImage = Card;
                        Community4.Show();
                        break;
                }
            }
            else if (command[0] == "Hand") // Hand$position$number(2-14)$shape(1-4)$number(2-14)$shape(1-4)$
            {
                int pos = int.Parse(command[1]);
                players[pos].Pocket0.BackgroundImage = Image.FromFile("Data\\" + int.Parse(command[2]) + "_" + int.Parse(command[3]) + ".gif");
                players[pos].Pocket1.BackgroundImage = Image.FromFile("Data\\" + int.Parse(command[4]) + "_" + int.Parse(command[5]) + ".gif");
            }
            else if (command[0] == "Win") // Win$position$pot$
            {
                int pos = int.Parse(command[1]);
                int amount = int.Parse(command[2]);
                players[pos].Money += amount;
                if (players[pos].Name == I.Name)
                    I.Money += amount;
                Pot.Text = "0";
                foreach (Player p in players)
                {
                    p.Pocket0.BackgroundImage = Image.FromFile("Data\\back.jpg");
                    players[pos].Pocket0.Show();
                    p.Pocket1.BackgroundImage = Image.FromFile("Data\\back.jpg");
                    players[pos].Pocket1.Show();
                    players[pos].Action.Hide();
                }
                players[button].Button.Hide();
                Community0.Hide();
                Community1.Hide();
                Community2.Hide();
                Community3.Hide();
                Community4.Hide();
                Pocket0.Hide();
                Pocket1.Hide();
            }
         
  
            else if (command[0] == "Kick") // Kick$
            {
                I.Write("Stand$");
                Stand.Hide();
                Sit.Show();
            }
        }

      

        public char Shape(string a)
        {
            switch (int.Parse(a))
            {
                case 1:
                    return Convert.ToChar(9824);
                case 2:
                    return Convert.ToChar(9829);
                case 3:
                    return Convert.ToChar(9830);
                default:
                    return Convert.ToChar(9827);
            }
        }

        public string Number(string a)
        {
            switch (int.Parse(a))
            {
                case 14:
                    return "A";
                case 13:
                    return "K";
                case 12:
                    return "Q";
                case 11:
                    return "J";
                case 10:
                    return "T";
                default:
                    return a;
            }
        }

      

        private void Sit_Click(object sender, EventArgs e)
        {
            I.Write("Sit$");
            Sit.Hide();
            Stand.Show();
        }

     


        private void Stand_Click(object sender, EventArgs e)
        {
            I.Write("Fold$");
            Thread.Sleep(500);
            I.Write("Stand$");
            Stand.Hide();
            Sit.Show();
        }

  
    }
}