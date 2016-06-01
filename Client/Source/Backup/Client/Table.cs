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
            Log.Clear();
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
                Write(players[pos].Name + " has joined the table with " + players[pos].Money + "$");
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
                Write(players[pos].Name + " has left the table");
                players[pos].Name = "Open";
                players[pos].MoneyAsControl.Text = "Seat";
                players[pos].Pocket0.Hide();
                players[pos].Pocket1.Hide();
                players[pos].Action.Hide();
            }
            else if (command[0] == "Button") // Button$position$
            {
                int pos = int.Parse(command[1]);
                if(button == 0)
                    Write(players[pos].Name + " has been randomly chosen to be the dealer");
                button = pos;
                players[button].Button.Show();
            }
            else if (command[0] == "Dealer") // Dealer$position$
            {
                button = int.Parse(command[1]);
                players[button].Button.Show();
                foreach (Player p in players)
                    if (p.Name != "Open")
                    {
                        p.Pocket0.Show();
                        p.Pocket1.Show();
                    }
            }
            else if (command[0] == "SmallBlind") // SmallBlind$position$amount$
            {
                int pos = int.Parse(command[1]);
                int amount = int.Parse(command[2]);
                players[pos].Money -= amount;
                if (players[pos].Name == I.Name)
                    I.Money -= amount;
                Pot.Text = (int.Parse(Pot.Text) + amount) + "";
                Write(players[pos].Name + " has submitted a small blind of " + amount);
            }
            else if (command[0] == "BigBlind") // BigBlind$position$amount$
            {
                int pos = int.Parse(command[1]);
                int amount = int.Parse(command[2]);
                bb = amount;
                players[pos].Money -= amount;
                if (players[pos].Name == I.Name)
                    I.Money -= amount;
                Pot.Text = (int.Parse(Pot.Text) + amount) + "";
                Write(players[pos].Name + " has submitted a big blind of " + amount);
            }
            else if (command[0] == "Pocket") // Pocket$id$number(2-14)$shape(1-4)$
            {
                Image Card = Image.FromFile("Data\\" + int.Parse(command[2]) + "_" + int.Parse(command[3]) + ".gif");
                if (command[1] == "0")
                {
                    Pocket0.BackgroundImage = Card;
                    Pocket0.Show();
                    Write("Pocket Card 1: " + Number(command[2]) + Shape(command[3]));
                }
                else
                {
                    Pocket1.BackgroundImage = Card;
                    Pocket1.Show();
                    Write("Pocket Card 2: " + Number(command[2]) + Shape(command[3]));
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
                        Write("Community Card 1: " + Number(command[2]) + Shape(command[3]));
                        break;
                    case "1":
                        Community1.BackgroundImage = Card;
                        Community1.Show();
                        Write("Community Card 2: " + Number(command[2]) + Shape(command[3]));
                        break;
                    case "2":
                        Community2.BackgroundImage = Card;
                        Community2.Show();
                        Write("Community Card 3: " + Number(command[2]) + Shape(command[3]));
                        break;
                    case "3":
                        Community3.BackgroundImage = Card;
                        Community3.Show();
                        Write("Community Card 4: " + Number(command[2]) + Shape(command[3]));
                        break;
                    default:
                        Community4.BackgroundImage = Card;
                        Community4.Show();
                        Write("Community Card 5: " + Number(command[2]) + Shape(command[3]));
                        break;
                }
            }
            else if (command[0] == "Hand") // Hand$position$number(2-14)$shape(1-4)$number(2-14)$shape(1-4)$
            {
                int pos = int.Parse(command[1]);
                players[pos].Pocket0.BackgroundImage = Image.FromFile("Data\\" + int.Parse(command[2]) + "_" + int.Parse(command[3]) + ".gif");
                players[pos].Pocket1.BackgroundImage = Image.FromFile("Data\\" + int.Parse(command[4]) + "_" + int.Parse(command[5]) + ".gif");
                Write(players[pos].Name + " shows " + Number(command[2]) + Shape(command[3]) + " " + Number(command[4]) + Shape(command[5]));
            }
            else if (command[0] == "Win") // Win$position$pot$
            {
                int pos = int.Parse(command[1]);
                int amount = int.Parse(command[2]);
                players[pos].Money += amount;
                if (players[pos].Name == I.Name)
                    I.Money += amount;
                Write(players[pos].Name + " wins a pot of " + amount + "$");
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
                HideButtons();
                Community0.Hide();
                Community1.Hide();
                Community2.Hide();
                Community3.Hide();
                Community4.Hide();
                Pocket0.Hide();
                Pocket1.Hide();
            }
            else if (command[0] == "SidePot") // SidePot$position$amount
            {
                int pos = int.Parse(command[1]);
                int amount = int.Parse(command[2]);
                players[pos].Money += amount;
                if (players[pos].Name == I.Name)
                    I.Money += amount;
                Write(players[pos].Name + " wins a side pot of " + amount + "$");
                Pot.Text = (int.Parse(Pot.Text) - amount) + "";
            }
            else if (command[0] == "Playing") // Playing$position
                players[int.Parse(command[1])].Action.Show();
            else if (command[0] == "Waiting") // Waiting$bet$inroundmoney$
            {
                int bet = int.Parse(command[1]);
                int inround = int.Parse(command[2]);
                int tmp;
                if (bet - inround != 0)
                {
                    Call.Text = "Call " + (bet - inround);
                    Raise.Text = "Raise";
                    tmp = 2 * bet - inround;
                }
                else
                {
                    Call.Text = "Check";
                    Raise.Text = "Bet";
                    tmp = bb - inround;
                }
                RaiseAmount.Text = tmp + "";
                RaiseBar.Minimum = tmp;
                RaiseBar.Maximum = I.Money;
                RaiseBar.SmallChange = bb;
                RaiseBar.LargeChange = 5 * bb;
                ShowButtons();
                if (bet - inround > I.Money)
                {
                    Call.Text = "All In";
                    RaiseBar.Hide();
                    RaiseAmount.Hide();
                    Raise.Hide();
                }
                else if (tmp > I.Money)
                {
                    Raise.Text = "All In";
                    RaiseBar.Hide();
                    RaiseAmount.Hide();
                }
            }
            else if (command[0] == "Call") // Call$position$amount
            {
                int pos = int.Parse(command[1]);
                int amount = int.Parse(command[2]);
                players[pos].Money -= amount;
                Pot.Text = (int.Parse(Pot.Text) + amount) + "";
                Write(players[pos].Name + " calls and adds " + amount + "$ to the pot");
                players[pos].Action.Hide();
            }
            else if (command[0] == "Raise") // Raise$position$amount$total
            {
                int pos = int.Parse(command[1]);
                int amount = int.Parse(command[2]);
                players[pos].Money -= amount;
                Pot.Text = (int.Parse(Pot.Text) + amount) + "";
                Write(players[pos].Name + " raises to " + int.Parse(command[3]));
                players[pos].Action.Hide();
            }
            else if (command[0] == "Fold") // Fold$position$
            {
                int pos = int.Parse(command[1]);
                players[pos].Pocket0.Hide();
                players[pos].Pocket1.Hide();
                Write(players[pos].Name + " folds");
                players[pos].Action.Hide();
            }
            else if (command[0] == "Check") // Check$position$
            {
                Write(players[int.Parse(command[1])].Name + " checks");
                players[int.Parse(command[1])].Action.Hide();
            }
            else if (command[0] == "AllIn") // Allin$position$
            {
                int pos = int.Parse(command[1]);
                Write(players[pos].Name + " moves All-In");
                players[pos].Action.Hide();
                Pot.Text = (int.Parse(Pot.Text) + players[pos].Money) + "";
                players[int.Parse(command[1])].Money = 0;
            }

            else if (command[0] == "Kick") // Kick$
            {
                I.Write("Stand$");
                Stand.Hide();
                Sit.Show();
            }
        }

        public void Write(string a)
        {
            Log.Text += a + Environment.NewLine;
            Log.SelectionStart = Log.Text.Length;
            Log.ScrollToCaret();
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

        private void TableClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Fold.Visible)
            {
                I.Write("Fold$");
                Thread.Sleep(500);
            }
            I.Write("Stand$");
            I.Write("Leave$");
            lobby.Show();
        }

        private void Sit_Click(object sender, EventArgs e)
        {
            I.Write("Sit$");
            Sit.Hide();
            Stand.Show();
        }

        private void Call_Click(object sender, EventArgs e)
        {
            if (Call.Text == "All In")
            {
                I.Write("AllIn$");
                I.Money = 0;
            }
            else
            {
                I.Write("Call$");
                if (Call.Text != "Check")
                    I.Money -= int.Parse(Call.Text.Substring(Call.Text.IndexOf(" ") + 1));
            }
            HideButtons();
        }

        private void Raise_Click(object sender, EventArgs e)
        {
            if (int.Parse(RaiseAmount.Text) == I.Money || Raise.Text == "All In")
            {
                I.Write("AllIn$");
                I.Money = 0;
            }
            else
            {
                I.Write("Raise$" + RaiseAmount.Text + "$");
                I.Money -= int.Parse(RaiseAmount.Text);
            }
            HideButtons();
        }

        private void Fold_Click(object sender, EventArgs e)
        {
            I.Write("Fold$");
            HideButtons();
        }

        private void HideButtons()
        {
            RaiseBar.Hide();
            Call.Hide();
            Raise.Hide();
            Fold.Hide();
            RaiseAmount.Hide();
        }

        private void ShowButtons()
        {
            RaiseBar.Show();
            Call.Show();
            Raise.Show();
            Fold.Show();
            RaiseAmount.Show();
        }

        private void Stand_Click(object sender, EventArgs e)
        {
            I.Write("Fold$");
            Thread.Sleep(500);
            HideButtons();
            I.Write("Stand$");
            Stand.Hide();
            Sit.Show();
        }

        private void RaiseBar_Scroll(object sender, System.EventArgs e)
        {
            RaiseAmount.Text = RaiseBar.Value + "";
        }
    }
}