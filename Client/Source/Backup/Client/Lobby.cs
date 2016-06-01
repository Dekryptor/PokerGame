using System;
using System.Windows.Forms;
using System.Threading;

namespace Client
{
    public partial class Lobby : Form
    {
        public Lobby()
        {
            InitializeComponent();
            label1.Text = I.Name;
            I.Write("Money$");
            I.Money = int.Parse(I.Read());
            label2.Text = "" + I.Money;
            listView1.Columns.Add("Id", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("Name", 164, HorizontalAlignment.Left);
            listView1.Columns.Add("Blinds", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Seats", 100, HorizontalAlignment.Left);
            I.Write("List$");
            ProcessTables();
        }
        private void ProcessTables()
        {
            string rec = "";
            while (rec.IndexOf('@') == -1)
                rec = I.Read();
            string[] data = new string[4];
            string line;
            ListViewItem tmp;
            while (rec.IndexOf('@') != -1)
            {
                line = rec.Substring(0, rec.IndexOf('@'));
                for (int i = 0; line.IndexOf('$') != -1; i++)
                {
                    data[i] = line.Substring(0, line.IndexOf('$'));
                    line = line.Remove(0, line.IndexOf('$') + 1);
                }
                tmp = new ListViewItem(data);
                listView1.Items.Add(tmp);
                rec = rec.Remove(0, rec.IndexOf('@') + 1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            Thread.Sleep(3000);
            I.Write("List$");
            ProcessTables();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                I.Write("Spectate$" + listView1.SelectedItems[0].SubItems[0].Text + "$");
                Table t = new Table(this);
                t.Text = listView1.SelectedItems[0].SubItems[1].Text + "   " + listView1.SelectedItems[0].SubItems[2].Text;
                t.Show();
                this.Hide();
                listView1.Items.Clear();
            }
        }

        private void LobbyClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            I.Write("Exit$");
            Environment.Exit(1);
        }

        private void Lobby_VisibleChanged(object sender, EventArgs e)
        {
            label2.Text = I.Money + "";
        }
    }
}
