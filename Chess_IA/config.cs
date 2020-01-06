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

namespace Config
{
    public partial class config : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        public int t = 0;
        public int p = 0;
        public bool sound = true;
        public bool mostrar = true;
        public bool coord = true;

        public config(string t1, string p1, bool sound1, bool mostrar1, bool coord1)
        {
            t = Convert.ToInt32(t1.Substring(t1.IndexOf("board") + 5, 1));
            p = Int32.Parse(p1.Substring(p1.IndexOf("\\e") + 2, 1));
            sound = sound1;
            mostrar = mostrar1;
            coord = coord1;
            InitializeComponent();
            pictureBox1.Image = Image.FromFile("image\\board" + t.ToString() + "\\button_b.png");
            pictureBox10.Image = Image.FromFile("image\\e" + p.ToString() + "\\button_p.png");

            pictureBox11.Image = Image.FromFile("image\\app_" + (mostrar ? "" : "un") + "selected.png");
            pictureBox13.Image = Image.FromFile("image\\app_" + (sound ? "" : "un") + "selected.png");
            pictureBox12.Image = Image.FromFile("image\\app_" + (coord ? "" : "un") + "selected.png");

            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            pictureBox10.MouseWheel += pictureBox1_MouseWheel;
        }

        void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            var obj = (PictureBox)sender;
            if (obj.Name == pictureBox1.Name)
            {
                t = (t + 1 + (e.Delta < 0 ? 7 : 0)) % 9;
                pictureBox1.Image = Image.FromFile("image\\board" + t.ToString() + "\\button_b.png");
            }
            else
            {
                p = (p + 1 + (e.Delta < 0 ? 6 : 0)) % 8;
                pictureBox10.Image = Image.FromFile("image\\e" + p.ToString() + "\\button_p.png");
            }
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

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pictureBox14_MouseDown(object sender, MouseEventArgs e)
        {
            t = (t + 8) % 9;
            pictureBox1.Image = Image.FromFile("image\\board" + t.ToString() + "\\button_b.png");
        }

        private void pictureBox14_MouseEnter(object sender, EventArgs e)
        {
            pictureBox14.Image = Image.FromFile("image\\back_button_pressed.png");
        }

        private void pictureBox14_MouseLeave(object sender, EventArgs e)
        {
            pictureBox14.Image = Image.FromFile("image\\back_button.png");
        }

        private void pictureBox15_MouseDown(object sender, MouseEventArgs e)
        {
            t = (t + 1) % 9;
            pictureBox1.Image = Image.FromFile("image\\board" + t.ToString() + "\\button_b.png");
        }

        private void pictureBox15_MouseEnter(object sender, EventArgs e)
        {
            pictureBox15.Image = Image.FromFile("image\\next_buton_press.png");
        }

        private void pictureBox15_MouseLeave(object sender, EventArgs e)
        {
            pictureBox15.Image = Image.FromFile("image\\next_buton.png");
        }

        private void pictureBox9_MouseDown(object sender, MouseEventArgs e)
        {
            p = (p + 7) % 8;
            pictureBox10.Image = Image.FromFile("image\\e" + p.ToString() + "\\button_p.png");
        }

        private void pictureBox9_MouseEnter(object sender, EventArgs e)
        {
            pictureBox9.Image = Image.FromFile("image\\back_button_pressed.png");
        }

        private void pictureBox9_MouseLeave(object sender, EventArgs e)
        {
            pictureBox9.Image = Image.FromFile("image\\back_button.png");
        }

        private void pictureBox8_MouseDown(object sender, MouseEventArgs e)
        {
            p = (p + 1) % 8;
            pictureBox10.Image = Image.FromFile("image\\e" + p.ToString() + "\\button_p.png");
        }

        private void pictureBox8_MouseEnter(object sender, EventArgs e)
        {
            pictureBox8.Image = Image.FromFile("image\\next_buton_press.png");
        }

        private void pictureBox8_MouseLeave(object sender, EventArgs e)
        {
            pictureBox8.Image = Image.FromFile("image\\next_buton.png");
        }

        private void pictureBox11_MouseDown(object sender, MouseEventArgs e)
        {
            mostrar = !mostrar;
            pictureBox11.Image = Image.FromFile("image\\app_" + (mostrar ? "" : "un") + "selected.png");
        }

        private void pictureBox13_MouseDown(object sender, MouseEventArgs e)
        {
            sound = !sound;
            pictureBox13.Image = Image.FromFile("image\\app_" + (sound ? "" : "un") + "selected.png");
        }

        private void pictureBox12_MouseDown(object sender, MouseEventArgs e)
        {
            coord = !coord;
            pictureBox12.Image = Image.FromFile("image\\app_" + (coord ? "" : "un") + "selected.png");
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            var obj = (PictureBox)sender;
            obj.Select();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            button1.Select();
        }
    }
}
