using System;
using System.Windows.Forms;
using System.Threading;

namespace Client
{
    public partial class Login : Form
    {

        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            I.Write(textBox1.Text + "$" + textBox2.Text);
            if (I.Read() == "1")
            {
                I.Name = textBox1.Text;

                this.Visible = false;
                Lobby lobby = new Lobby();
                lobby.Show();
            }
            else
            {
                label1.Visible = true;
                Thread.Sleep(5000);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
