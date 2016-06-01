using System;
using System.Collections.Generic;
using System.Threading;

namespace Server
{
    class Table
    {
        string name;
        int size;
        CircularLinkedList<Client> players;
        List<Client> spectators;
        List<Client> waitingplayers;
        int blind;
        Deck deck;
        int pot = 0;
        int bet = 0;
        string com;
        string history = "";
        int count;
        Node<Client> button;
        Node<Client> last;
        Node<Client> playerturn;
        Card[] community = new Card[5];
        Thread game;

        public Table(string Name, int Size, int SmallBlind)
        {
            name = Name;
            size = Size;
            players = new CircularLinkedList<Client>();
            spectators = new List<Client>();
            waitingplayers = new List<Client>();
            blind = SmallBlind;
        }

        public int FindNextSeat()
        {
            for (int i = 0; i < size; i++)
            {
                bool check = true;
                foreach (Client p in players)
                    if (p.Position == i)
                        check = false;
                foreach (Client p in waitingplayers)
                    if (p.Position == i)
                        check = false;
                if (check)
                    return i;
            }
            return 0;
        }

        public void Add(Client p)
        {
            p.Position = FindNextSeat();
            waitingplayers.Add(p);
            Inform("Joined$" + p.Position + "$" + p.Name + "$" + p.Money + "$");
            if (waitingplayers.Count + players.Count > 1)
                if (game == null)
                {
                    game = new Thread(Game);
                    game.Start();
                }
                else if (!game.IsAlive)
                {
                    game = new Thread(Game);
                    game.Start();
                }
            try
            {
                string rec;
                while ((rec = p.Reader.ReadLine()) != "Stand$")
                    if (playerturn.Value == p)
                        com = rec;
                spectators.Add(p);
            }
            catch
            {
                if (playerturn.Value == p)
                {
                    com = "Fold$";
                    Thread.Sleep(500);
                }
                p.Disconnect();
            }
            finally
            {
                if (p.Pocket[0].Value != 0)
                    deck.Add(p.Pocket);
                Remove(p);
            }
        }

        public void Remove(Client p)
        {
            if (waitingplayers.Contains(p))
                waitingplayers.Remove(p);
            else
                players.Remove(p);
            Inform("Left$" + p.Position + "$");
            if (game != null)
                if (game.IsAlive && players.Count + waitingplayers.Count < 2)
                    game.Abort();
        }

        public void Spectate(Client s)
        {
            string tmp = null;
            foreach (Client p in players)
                tmp += "Sitting$" + p.Position + "$" + p.Name + "$" + (p.Money + p.InRoundMoney) + "$@";
            foreach (Client p in waitingplayers)
                tmp += "Joined$" + p.Position + "$" + p.Name + "$" + p.Money + "$@";
            s.Writer.WriteLine(tmp);
            s.Writer.WriteLine(history);
            spectators.Add(s);
            try
            {
                string rec;
                while ((rec = s.Reader.ReadLine()) != "Leave$")
                    if (rec == "Sit$")
                        if (players.Count + waitingplayers.Count < size && s.Money > 10 * blind)
                        {
                            spectators.Remove(s);
                            Add(s);
                        }
                s.Writer.WriteLine("Removed$");
            }
            catch
            {
                s.Disconnect();
            }
            finally
            {
                spectators.Remove(s);
            }
        }

        public void Game()
        {
         
        }

        public void AddToGame()
        {
            foreach (Client p in waitingplayers)
                players.AddLast(p);
            waitingplayers.Clear();
        }

   

   

        public void Deal()
        {
            foreach (Client p in players)
            {
                p.Pocket[0] = deck.Draw();
                p.Writer.WriteLine("Pocket$0$" + p.Pocket[0]);
                p.Pocket[1] = deck.Draw();
                p.Writer.WriteLine("Pocket$1$" + p.Pocket[1]);
            }
        }

        public void Flop()
        {
          
        }

        public void Turn()
        {
           
        }

        public void River()
        {
           
        }

        public void Showdown()
        {
         
        }

        public void EarlyWin()
        {
           
        }

        public void PlayerMove(Client p)
        {
            
        }

        public void Collect()
        {
      
        }

        public void Inform(string a)
        {
            if (a.Substring(0, 4) != "Join" && a.Substring(0, 4) != "Left") // not if Joined or Left
                history += a + "@";
            foreach (Client i in players)
                i.Writer.WriteLine(a);
            foreach (Client i in waitingplayers)
            {
                try { i.Writer.WriteLine(a); }
                catch { continue; }
            }
            foreach (Client i in spectators)
                i.Writer.WriteLine(a);
        }

        public override string ToString()
        {
            return (name + "$" + blind + "/" + 2 * blind + "$" + (players.Count + waitingplayers.Count) + "/" + size + "$");
        }
    }
}