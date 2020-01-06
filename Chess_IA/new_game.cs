using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NewGame
{
    public partial class new_game : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        
        public bool versus = true;
        public bool blancas = true;
        public int dif = 1;

        public new_game(bool versus1, bool blancas1, int dif1)
        {
            versus = versus1;
            blancas = blancas1;
            dif = dif1;
            InitializeComponent();
            label10.MouseWheel += label10_MouseWheel;
            pictureBox9.Image = Image.FromFile("image\\radio_" + (versus ? "on" : "off") + "1.png");
            pictureBox10.Image = Image.FromFile("image\\radio_" + (!versus ? "on" : "off") + "1.png");
            label10.Text = dif.ToString();
            pictureBox12.Image = Image.FromFile("image\\radio_" + (versus ? "on" : "off") + ".png");
            pictureBox1.Image = Image.FromFile("image\\radio_" + (!versus ? "on" : "off") + ".png");
        }

        void label10_MouseWheel(object sender, MouseEventArgs e)
        {
            int d = e.Delta;
            label10.Text = (Convert.ToInt32(label10.Text) + ((label10.Text != "12" && d>0)?1:(label10.Text != "1" && d<0)?-1:0)).ToString();
            dif = Convert.ToInt32(label10.Text);
            label11.Text = Math.Round(0.01F * (Math.Pow(2.0F, dif - 1)), 2).ToString() + " sec";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Mover(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            pictureBox9.Image = Image.FromFile("image\\radio_on1.png");
            pictureBox10.Image = Image.FromFile("image\\radio_off1.png");
            versus = true;
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            pictureBox12.Image = Image.FromFile("image\\radio_off.png");
            pictureBox1.Image = Image.FromFile("image\\radio_off.png");
            pictureBox13.Image = Image.FromFile("image\\radio_on.png");
            blancas = ((int)DateTime.Now.Second % 2 == 1);
            pictureBox9_Click(sender, e);
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            pictureBox9.Image = Image.FromFile("image\\radio_off1.png");
            pictureBox10.Image = Image.FromFile("image\\radio_on1.png");
            versus = false;
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            pictureBox12.Image = Image.FromFile("image\\radio_on.png");
            pictureBox1.Image = Image.FromFile("image\\radio_off.png");
            pictureBox13.Image = Image.FromFile("image\\radio_off.png");
            blancas = false;
            pictureBox9_Click(sender, e);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox12.Image = Image.FromFile("image\\radio_off.png");
            pictureBox1.Image = Image.FromFile("image\\radio_on.png");
            pictureBox13.Image = Image.FromFile("image\\radio_off.png");
            blancas = true;
            pictureBox9_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pictureBox14_MouseEnter(object sender, EventArgs e)
        {
            pictureBox14.Image = Image.FromFile("image\\back_button_pressed.png");
        }

        private void pictureBox15_MouseEnter(object sender, EventArgs e)
        {
            pictureBox15.Image = Image.FromFile("image\\next_buton_press.png");
        }

        private void pictureBox14_MouseLeave(object sender, EventArgs e)
        {
            pictureBox14.Image = Image.FromFile("image\\back_button.png");
        }

        private void pictureBox15_MouseLeave(object sender, EventArgs e)
        {
            pictureBox15.Image = Image.FromFile("image\\next_buton.png");
        }

        private void pictureBox14_MouseDown(object sender, MouseEventArgs e)
        {
            if (label10.Text != "1")
            {
                label10.Text = (Convert.ToInt32(label10.Text) - 1).ToString();
                dif = Convert.ToInt32(label10.Text);
            }
            label11.Text = Math.Round(0.01F * (Math.Pow(2.0F, dif - 1)), 2).ToString() + " sec";
        }

        private void pictureBox15_MouseDown(object sender, MouseEventArgs e)
        {
            if (label10.Text != "12")
            {
                label10.Text = (Convert.ToInt32(label10.Text) + 1).ToString();
                dif = Convert.ToInt32(label10.Text);
            }
            label11.Text = Math.Round(0.01F * (Math.Pow(2.0F, dif - 1)), 2).ToString() + " sec";
        }

        private void label10_MouseEnter(object sender, EventArgs e)
        {
            label10.Select();
        }

        private void label10_MouseLeave(object sender, EventArgs e)
        {
            button1.Select();
        }
    }
}
