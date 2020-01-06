using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using Help;
using About;
using NewGame;
using Config;
using System.Threading;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Win32;

namespace Chess_IA
{
    public partial class game : Form
    {
        public about form3;
        public new_game form4;
        public config form5;
        public help form6;
        public CMD cmd;
        RegistryKey rk;

        PictureBox[][] A;
        chess aj;
        int cxClient, cyClient;
        int bandera = 0;
        bool vs_computer = true;
        int mx, my;
        Par PX = new Par(-1, -1), PY = new Par(-1, -1);
        bool Tinv = false;
        string rutat = "image\\board3\\", rutap = "image\\e0\\";
        string[] pieces = { "bb", "bk", "bn", "bp", "bq", "br", "wb", "wk", "wn", "wp", "wq", "wr" };
        LChess laj;
        List<Par> List_Mov;
        List<Par> Ult_Mov;
        string Algebra = "";
        bool mostrarmov = true;
        int sizex, sizey, sizem;
        float sizep, sizeb, sizet;
        bool sound = true;
        bool mostrarcoord = true;
        int dificultad = 1;
        bool juego_p_blancas = true;

        public game()
        {
            InitializeComponent();
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - 10;
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            cmd = new CMD();

            if (Registry.CurrentUser.OpenSubKey("Software\\IA_Chess_Dota") == null)
            {
                rk = Registry.CurrentUser.CreateSubKey("Software\\IA_Chess_Dota");
                guardar();
            }
            else Cargar();
            cmd.Argg += cmd_Argg;
        }

        void cmd_Argg(object sender, CMD.argumento e)
        {
            string line = e.arg1;
            Text = line;
            if (line.Contains("O-O"))
            {
                Par enrr, enrr1;
                if (aj.juega)
                    enrr = new Par(aj.get_pos("bk"));
                else
                    enrr = new Par(aj.get_pos("wk"));

                bandera = 2;
                pictureBox2_MouseDown(A[enrr.X][enrr.Y], new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));

                if (line.Count() > 3)
                    enrr1 = new Par(enrr.X, enrr.Y - 2);
                else
                    enrr1 = new Par(enrr.X, enrr.Y + 2);

                pictureBox2_MouseDown(A[enrr1.X][enrr1.Y], new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
                bandera = 0;
            }
            else
            {
                string nextmove = line.ToString();

                string next = "";
                int i, j;
                for (i = 0; i < nextmove.Count(); i++)
                    next += nextmove[i];

                int t = 0;
                if (next[0].ToString() == next[0].ToString().ToUpper())
                {
                    next = next.Remove(0, 1);
                    t++;
                }

                j = nextmove[t] - 'a';
                next = next.Remove(0, 1);
                t++;
                i = 56 - nextmove[t];
                next = next.Remove(0, 1);
                t++;
                bandera = 2;
                pictureBox2_MouseDown(A[i][j], new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
                if (next[0] == 'x')
                {
                    next = next.Remove(0, 1);
                    t++;
                }
                j = nextmove[t] - 'a';
                next = next.Remove(0, 1);
                t++;
                i = 56 - nextmove[t];
                next = next.Remove(0, 1);
                t++;
                pictureBox2_MouseDown(A[i][j], new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
                bandera = 0;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel2.Location = new Point(0, -50);
            cxClient = this.Width;
            cyClient = this.Height;
            sizex = cxClient - 59;
            sizey = cyClient - 81;
            sizem = Math.Min(sizex, sizey);
            ntl.Width = sizem;
            ntn.Height = sizem;
            panel1.Size = new Size(sizem, sizem);
            panel3.Location = new Point(panel1.Width + 30 + 18, panel3.Location.Y);

            sizep = sizem / 8.0f;
            sizeb = 16.0f * sizep / 100.0f;
            sizet = 96.0f * sizep / 100.0f;

            imageList1.Images.Clear();
            for (int i = 0; i < 12; i++)
            {
                imageList1.Images.Add(Image.FromFile(rutap + pieces[i] + "1.png"));
                imageList1.Images.SetKeyName(i, pieces[i]);
            }

            List_Mov = new List<Par>();
            Ult_Mov = new List<Par>();

            panel1.BackgroundImage = Image.FromFile(rutat + "t.jpg");

            aj = new chess();
            laj = new LChess();
            aj.reset();
            laj.insert(aj);
            mx = my = -1;

            A = new PictureBox[8][];
            for (int i = 0; i < 8; i++)
                A[i] = new PictureBox[8];

            paint1();

            int[] arr = new int[4];
            arr[0] = 0;
            arr[1] = 1;
            arr[2] = 6;
            arr[3] = 7;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 8; j++)
                    colocar(arr[i], j);
        }

        public void cambiar_piezas()
        {
            imageList1.Images.Clear();
            for (int i = 0; i < 12; i++)
            {
                imageList1.Images.Add(Image.FromFile(rutap + pieces[i] + "1.png"));
                imageList1.Images.SetKeyName(i, pieces[i]);
            }

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    colocar(i, j);
        }

        public void cambiar_tablero()
        {
            panel1.BackgroundImage = Image.FromFile(rutat + "t.jpg");
        }

        public void tablero_inv()
        {
            Tinv = !Tinv;
            if (Tinv)
            {
                Point tmp = groupBoxb.Location;
                groupBoxb.Location = groupBoxw.Location;
                groupBoxw.Location = tmp;
                ntl.LoadAsync("image\\board\\ntl1.png");
                ntn.LoadAsync("image\\board\\ntn1.png");

                int i1, j1;
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                    {
                        i1 = 7 - i;
                        j1 = 7 - j;
                        A[i1][j1].Location = new Point((int)(sizeb + sizet * j), (int)(sizeb + sizet * i));
                    }
            }
            else
            {
                Point tmp = groupBoxb.Location;
                groupBoxb.Location = groupBoxw.Location;
                groupBoxw.Location = tmp;
                ntl.LoadAsync("image\\board\\ntl.png");
                ntn.LoadAsync("image\\board\\ntn.png");

                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        A[i][j].Location = new Point((int)(sizeb + sizet * j + 0.5f), (int)(sizeb + sizet * i + 0.5f));

            }

            if (PX.X != -1)
            {
                PX = new Par(7 - PX.X, 7 - PX.Y);
                PY = new Par(7 - PY.X, 7 - PY.Y);
            }

            //for (int i = 0; i < 8; i++)
            //    for (int j = 0; j < 8; j++)
            //        paint(i, j, "null");

            Ult_Mov.Clear();
            mx = my = -1;
            actualizar();
        }

        public void actualizar()
        {
            if (aj.juega)
            {
                pictureBox7.LoadAsync("image\\turn2.png");
                label_10.ForeColor = Color.Navy;
                label9.ForeColor = Color.Silver;
                pictureBox6.LoadAsync("image\\turn.png");
            }
            else
            {
                pictureBox6.LoadAsync("image\\turn2.png");
                label9.ForeColor = Color.Navy;
                label_10.ForeColor = Color.Silver;
                pictureBox7.LoadAsync("image\\turn.png");
            }

            if (PX.X != -1)
            {
                paint(PX.X, PX.Y, "null");
                paint(PY.X, PY.Y, "null");
            }

            if (aj.inicio.X != -1)
            {
                paint(aj.inicio.X, aj.inicio.Y, "u");
                paint(aj.fin.X, aj.fin.Y, "u");
            }

            if (aj.inicio != new Par(-1, -1))
            {
                paint(aj.inicio.X, aj.inicio.Y, "u");
                paint(aj.fin.X, aj.fin.Y, "u");
                PX = aj.inicio;
                PY = aj.fin;
            }

            aj.igual();
            if (!aj.juega)
            {
                if (aj.jaque("wk"))
                {
                    checkw.BringToFront();
                    checkb.SendToBack();
                    aj.wkcheck = true;
                    aj.bkcheck = false;
                    Algebra += "+";
                }
                else
                {
                    checkw.SendToBack();
                    checkb.SendToBack();
                    aj.wkcheck = false;
                    aj.bkcheck = false;
                }
            }
            else
            {
                if (aj.jaque("bk"))
                {
                    checkb.BringToFront();
                    checkw.SendToBack();
                    aj.bkcheck = true;
                    aj.wkcheck = false;
                    Algebra += "+";
                }
                else
                {
                    checkb.SendToBack();
                    checkw.SendToBack();
                    aj.bkcheck = false;
                    aj.wkcheck = false;
                }
            }

            bool hay_mov = aj.hay_mov();
            if (!hay_mov && (aj.bkcheck || aj.wkcheck))
            {
                label_10.ForeColor = Color.Navy;
                label9.ForeColor = Color.Navy;
                //QMessageBox* jaquem = new QMessageBox(this);
                //jaquem->setIconPixmap(QPixmap(":/other/image/checkmate.png"));
                if (aj.bkcheck)
                {
                    //jaquem->setWindowTitle("THE WHITE PIECES WON");
                    pictureBox4.LoadAsync("image\\lose.png");
                    pictureBox4.BringToFront();
                    pictureBox5.LoadAsync("image\\win.png");
                    pictureBox5.BringToFront();
                }
                else
                {
                    //jaquem->setWindowTitle("THE BLACK PIECES WON");
                    pictureBox4.LoadAsync("image\\win.png");
                    pictureBox4.BringToFront();
                    pictureBox5.LoadAsync("image\\lose.png");
                    pictureBox5.BringToFront();
                }
                checkw.SendToBack();
                checkb.SendToBack();
                Algebra = Algebra.Replace('+', '#');
                //jaquem->exec();
            }
            else if (!hay_mov && !(aj.bkcheck && aj.wkcheck))
            {
                label_10.ForeColor = Color.Navy;
                label9.ForeColor = Color.Navy;
                //QMessageBox* jaquem = new QMessageBox(this);
                //jaquem->setIconPixmap(QPixmap(":/other/image/stalemate.png"));
                //jaquem->setWindowTitle("THE GAME IS DRAW");
                pictureBox4.LoadAsync("image\\draw.png");
                pictureBox4.BringToFront();
                pictureBox5.LoadAsync("image\\draw.png");
                pictureBox5.BringToFront();
                checkw.SendToBack();
                checkb.SendToBack();
                //jaquem->exec();
            }
            else
            {
                pictureBox4.LoadAsync("image\\null.png");
                pictureBox5.LoadAsync("image\\null.png");
            }
        }

        public void act()
        {
            aj.leng_alg = Algebra;
            aj.jugada++;
            ListViewItem item;
            if (aj.jugada % 2 == 1)
                item = new System.Windows.Forms.ListViewItem(new string[] { "", (aj.jugada / 2 + 1).ToString() + ":", aj.leng_alg }, aj.tab[aj.fin.X][aj.fin.Y]);
            else item = new System.Windows.Forms.ListViewItem(new string[] { "", "", aj.leng_alg }, aj.tab[aj.fin.X][aj.fin.Y]);
            listView1.Items.Add(item);
            listView1.Items[listView1.Items.Count - 1].EnsureVisible();
            if (vs_computer)
                enviar();
        }

        public void button_reset()
        {
            aj.reset();
            //laj.reset();
            //laj.insert(aj);
            listView1.Items.Clear();

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    paint(i, j, "null");
                    colocar(i, j);
                }

            Ult_Mov.Clear();
            mx = my = -1;

            if (Tinv)
                tablero_inv();
            else actualizar();
        }

        public void paint(int i, int j, string c)
        {
            if (c == "null")
                A[i][j].BackgroundImage = null;
            else if (c == "m")
                A[i][j].BackgroundImage = Image.FromFile("image\\m.png");

            if (mostrarmov)
            {
                if (c == "u")
                    A[i][j].BackgroundImage = Image.FromFile("image\\u.png");
                else if (bandera == 0)
                    A[i][j].BackgroundImage = Image.FromFile("image\\" + c + ".png");
            }
        }

        public void colocar(int i, int j)
        {
            A[i][j].Image = Image.FromFile(rutap + aj.tab[i][j] + ".png");
        }

        public void paint1()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    A[i][j] = new PictureBox();
                    A[i][j].BackColor = Color.Transparent;
                    A[i][j].Location = new Point((int)(sizeb + sizet * j + 0.5f), (int)(sizeb + sizet * i + 0.5f));
                    A[i][j].Size = new Size((int)(sizet + 0.5f), (int)(sizet + 0.5f));
                    A[i][j].SizeMode = PictureBoxSizeMode.Zoom;
                    A[i][j].BackgroundImageLayout = ImageLayout.Zoom;
                    A[i][j].Name = i.ToString() + " " + j.ToString();
                    A[i][j].MouseLeave += pictureBox2_MouseLeave;
                    A[i][j].MouseEnter += pictureBox2_MouseEnter;
                    A[i][j].MouseDown += pictureBox2_MouseDown;
                    panel1.Controls.Add(A[i][j]);
                }
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            panel2.Location = new Point(0, -50);
            if (bandera != 0)
                if (bandera != 2)
                    return;

            var obj = (PictureBox)sender;
            int row = obj.Name[0] - 48;
            int column = obj.Name[2] - 48;

            foreach (Par p1 in Ult_Mov)
            {
                int i = p1.X;
                int j = p1.Y;
                if (p1 != aj.fin && p1 != aj.inicio)
                    paint(i, j, "null");
                else paint(i, j, "u");
            }

            if (mx != -1 && aj.inicio != new Par(mx, my))
                paint(mx, my, "null");

            bool band = false;
            if (mx != -1 && Ult_Mov.Count > 0)
            {
                foreach (Par p1 in Ult_Mov)
                    if (p1 == new Par(row, column))
                    {
                        band = true;
                        break;
                    }
            }
            if (band)
            {
                aj.pmov = aj.tab[mx][my];
                Algebra = "";

                if ((aj.tab[mx][my] == "bp" && mx == 6) || (aj.tab[mx][my] == "wp" && mx == 1)) // coronacion
                {
                    string cor = (aj.tab[mx][my] == "bp") ? "b" : "w";
                    //QMessageBox* Mensaje = new QMessageBox(this);
                    //Mensaje->setWindowTitle("Coronación");
                    //Mensaje->setText("Seleccione una pieza para coronar como:");
                    //QPushButton* cab = Mensaje->addButton("",QMessageBox::YesRole);
                    //QPushButton* alf = Mensaje->addButton("",QMessageBox::YesRole);
                    //QPushButton* tor = Mensaje->addButton("",QMessageBox::YesRole);
                    //QPushButton* dam = Mensaje->addButton("",QMessageBox::YesRole);

                    //cab->setIconSize(QSize(75, 75));
                    //cab->setIcon(QIcon(rutap + cor + "n"));
                    //alf->setIconSize(QSize(75, 75));
                    //alf->setIcon(QIcon(rutap + cor + "b"));
                    //tor->setIconSize(QSize(75, 75));
                    //tor->setIcon(QIcon(rutap + cor + "r"));
                    //dam->setIconSize(QSize(75, 75));
                    //dam->setIcon(QIcon(rutap + cor + "q"));

                    //Mensaje->setDefaultButton(dam);
                    //Mensaje->exec();

                    //if(Mensaje->clickedButton() == cab)
                    //    cor += "n";
                    //else if(Mensaje->clickedButton() == alf)
                    //    cor += "b";
                    //else if(Mensaje->clickedButton() == tor)
                    //    cor += "r";
                    //else cor += "q";

                    cor += "q";

                    Algebra += (char)(my + 'a');
                    Algebra += (8 - mx).ToString();
                    if (aj.tab[row][column] != "null")
                    {
                        Algebra += "x";
                    }
                    Algebra += (char)(column + 'a');
                    Algebra += (8 - row).ToString();

                    Algebra += "=";
                    Algebra += cor[1].ToString().ToUpper();

                    aj.set_tab(row, column, cor);
                    aj.set_tab(mx, my, "null");

                    colocar(mx, my);
                    colocar(row, column);
                }
                else if (aj.tab[mx][my][1] == 'p' && aj.tab[row][column] == "null" && my != column) // peon al paso
                {
                    Algebra += (char)(my + 'a');
                    Algebra += (8 - mx).ToString();
                    Algebra += "x";
                    Algebra += (char)(column + 'a');
                    Algebra += (8 - row).ToString();

                    aj.set_tab(row, column, aj.tab[mx][my]);
                    aj.set_tab(mx, my, "null");
                    aj.set_tab(mx, column, "null");

                    colocar(mx, my);
                    colocar(mx, column);
                    colocar(row, column);
                }
                else if (aj.tab[mx][my] == "bk" && Math.Abs(column - my) == 2) // enrroque del rey negro
                {
                    aj.set_tab(row, column, aj.tab[mx][my]);
                    aj.set_tab(mx, my, "null");

                    if (my > column)
                    {
                        aj.set_tab(0, 0, "null");
                        aj.set_tab(0, 3, "br");
                        colocar(0, 0);
                        colocar(0, 3);
                        aj.bt1mov = true;
                        Algebra = "O-O-O";
                    }
                    else
                    {
                        aj.set_tab(0, 7, "null");
                        aj.set_tab(0, 5, "br");
                        colocar(0, 7);
                        colocar(0, 5);
                        aj.bt2mov = true;
                        Algebra = "O-O";
                    }
                    colocar(mx, my);
                    colocar(row, column);
                    aj.bkmov = true;
                }
                else if (aj.tab[mx][my] == "wk" && Math.Abs(column - my) == 2) // enrroque del rey blanco
                {
                    aj.set_tab(row, column, aj.tab[mx][my]);
                    aj.set_tab(mx, my, "null");

                    if (my > column)
                    {
                        aj.set_tab(7, 0, "null");
                        aj.set_tab(7, 3, "wr");
                        colocar(7, 0);
                        colocar(7, 3);
                        aj.wt1mov = true;
                        Algebra = "O-O-O";
                    }
                    else
                    {
                        aj.set_tab(7, 7, "null");
                        aj.set_tab(7, 5, "wr");
                        colocar(7, 7);
                        colocar(7, 5);
                        aj.wt2mov = true;
                        Algebra = "O-O";
                    }
                    colocar(mx, my);
                    colocar(row, column);
                    aj.wkmov = true;
                }
                else
                {
                    if (aj.tab[mx][my] == "br" && my == 0)
                        aj.bt1mov = true;
                    else if (aj.tab[mx][my] == "br" && my == 7)
                        aj.bt2mov = true;
                    else if (aj.tab[mx][my] == "wr" && my == 0)
                        aj.wt1mov = true;
                    else if (aj.tab[mx][my] == "wr" && my == 7)
                        aj.wt2mov = true;
                    else if (aj.tab[mx][my] == "bk")
                        aj.bkmov = true;
                    else if (aj.tab[mx][my] == "wk")
                        aj.wkmov = true;

                    if (aj.tab[mx][my][1] != 'p')
                        Algebra += aj.tab[mx][my][1].ToString().ToUpper();
                    Algebra += (char)(my + 'a');
                    Algebra += (7 - mx + 1).ToString();

                    if (aj.tab[row][column] != "null")
                        Algebra += "x";

                    Algebra += (char)(column + 'a');
                    Algebra += (7 - row + 1).ToString();

                    aj.set_tab(row, column, aj.tab[mx][my]);
                    aj.set_tab(mx, my, "null");
                    colocar(mx, my);
                    colocar(row, column);
                }
                aj.juega = !aj.juega;

                aj.ini_ = aj.inicio;
                aj.fin_ = aj.fin;

                aj.inicio = new Par(mx, my);
                aj.fin = new Par(row, column);

                if (aj.ini_ != new Par(-1, -1))
                {
                    int x, y;
                    x = aj.ini_.X;
                    y = aj.ini_.Y;
                    paint(x, y, "null");
                    x = aj.fin_.X;
                    y = aj.fin_.Y;
                    paint(x, y, "null");
                }

                paint(mx, my, "u");
                paint(row, column, "u");

                mx = my = -1;
                List_Mov.Clear();
                Ult_Mov.Clear();

                laj.insert(aj);
                actualizar();
                act();
                return;
            }

            Ult_Mov.Clear();
            if (aj.i_can_play(row, column, ref List_Mov))
            {
                if (mx == -1)
                {
                    paint(row, column, "m");
                    mx = row;
                    my = column;
                    foreach (Par p1 in List_Mov)
                    {
                        int i = p1.X;
                        int j = p1.Y;

                        if (p1 != aj.fin && p1 != aj.inicio && aj.tab[p1.X][p1.Y] == "null")
                            paint(i, j, "d");
                        else if (aj.tab[p1.X][p1.Y][0] != aj.tab[row][column][0] && p1 != aj.inicio && p1 != aj.fin)
                            paint(i, j, "a");
                        else if (p1 == aj.fin)
                            paint(i, j, "t");
                        else paint(i, j, "q");
                    }
                }
                else if (mx == row && my == column)
                {
                    int x, y;
                    if (aj.inicio != new Par(-1, -1))
                    {
                        x = aj.inicio.X;
                        y = aj.inicio.Y;
                        paint(x, y, "u");
                        x = aj.fin.X;
                        y = aj.fin.Y;
                        paint(x, y, "u");
                    }
                    mx = my = -1;
                }
                else
                {
                    paint(row, column, "m");
                    mx = row;
                    my = column;
                    foreach (Par p1 in List_Mov)
                    {
                        int i = p1.X;
                        int j = p1.Y;
                        if (p1 != aj.fin && p1 != aj.inicio && aj.tab[p1.X][p1.Y] == "null")
                            paint(i, j, "d");
                        else if (aj.tab[p1.X][p1.Y][0] != aj.tab[row][column][0] && p1 != aj.inicio && p1 != aj.fin)
                            paint(i, j, "a");
                        else if (p1 == aj.fin)
                            paint(i, j, "t");
                        else paint(i, j, "q");
                    }
                }
            }
            else mx = my = -1;
            Ult_Mov = new List<Par>(List_Mov);
            List_Mov.Clear();
        }

        public void enviar()
        {
            string move;
            move = Algebra.ToString();
            if (bandera == 0)
                cmd.read(Algebra);
            bandera = 1;
        }

        public void button_deshacer()
        {
            aj.deshacer(laj.deshacer());
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    paint(i, j, "null");
            Ult_Mov.Clear();
            mx = my = -1;
            actualizar();
        }

        bool contain(List<Par> L, Par a)
        {
            foreach (Par p1 in L)
                if (p1 == a)
                    return true;
            return false;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            var obj = (PictureBox)sender;
            int x = obj.Name[0] - 48;
            int y = obj.Name[2] - 48;
            if (aj.get_tab(x, y) != "null")
                obj.BackColor = Color.FromArgb(50, 50, 50, 255);
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            var obj = (PictureBox)sender;
            int x = obj.Name[0] - 48;
            int y = obj.Name[2] - 48;
            obj.BackColor = Color.FromArgb(0, 255, 255, 255);
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            menu.Image = Image.FromFile("image\\menu1.png");
        }

        private void menu_MouseLeave(object sender, EventArgs e)
        {
            menu.Image = Image.FromFile("image\\menu.png");
        }

        private void menu_Click(object sender, EventArgs e)
        {
            panel2.Location = new Point(0, 0);
            if (vs_computer)
                pictureBox9.Image = Image.FromFile("image\\prew1.png");
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            cxClient = this.Width;
            cyClient = this.Height;
            sizex = cxClient - 59;
            sizey = cyClient - 81;
            sizem = Math.Max(300, Math.Min(sizex, sizey));
            ntl.Width = sizem;
            ntn.Height = sizem;
            panel1.Size = new Size(sizem, sizem);

            sizep = sizem / 8.0f;
            sizeb = 16.0f * sizep / 100.0f;
            sizet = 96.0f * sizep / 100.0f;

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (Tinv)
                        A[7 - i][7 - j].Location = new Point((int)(sizeb + sizet * j + 0.5f), (int)(sizeb + sizet * i + 0.5f));
                    else A[i][j].Location = new Point((int)(sizeb + sizet * j + 0.5f), (int)(sizeb + sizet * i + 0.5f));
                    A[i][j].Size = new Size((int)(sizet + 0.5f), (int)(sizet + 0.5f));
                }
            panel2.Width = cxClient;
            panel3.Location = new Point(panel1.Width + 30 + 18, panel3.Location.Y);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            tablero_inv();
            panel2.Location = new Point(0, -50);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            panel2.Location = new Point(0, -50);
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            if (!vs_computer)
                button_deshacer();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            panel2.Location = new Point(0, -50);
            form3 = new about();
            form3.ShowDialog();
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            form4 = new new_game(vs_computer, juego_p_blancas, dificultad);
            if (form4.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dificultad = form4.dif;
                cmd.read("new");
                double diff = Math.Round(0.01F * (Math.Pow(2.0F, dificultad - 1)), 2);
                cmd.send_command(diff);
                bandera = 0;
                juego_p_blancas = form4.blancas;
                vs_computer = form4.versus;
                listView1.Items.Clear();
                button_reset();
            }
            panel2.Location = new Point(0, -50);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            form5 = new config(rutat, rutap, sound, mostrarmov, mostrarcoord);
            if (form5.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rutat = "image\\board" + form5.t + "\\";
                rutap = "image\\e" + form5.p + "\\";
                sound = form5.sound;
                mostrarmov = form5.mostrar;
                mostrarcoord = form5.coord;
                actualizar();
                cambiar_piezas();
                cambiar_tablero();
            }
            panel2.Location = new Point(0, -50);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cmd.close();
            guardar();
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(panel1.Width, panel1.Height);
            panel1.DrawToBitmap(bmp, this.DisplayRectangle);
            using (System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog())
            {
                sfd.Filter = "PNG | *.png | JPG | *.jpg | BMP | *.bmp";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (Path.GetExtension(sfd.FileName) == "*.jpg")
                        bmp.Save(sfd.FileName, ImageFormat.Jpeg);
                    else if (Path.GetExtension(sfd.FileName) == "*.bmp")
                        bmp.Save(sfd.FileName, ImageFormat.Bmp);
                    else
                        bmp.Save(sfd.FileName, ImageFormat.Png);
                }
            }
            panel2.Location = new Point(0, -50);
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            form6 = new help();
            form6.ShowDialog();
            panel2.Location = new Point(0, -50);
        }

        void guardar()
        {
            rk.SetValue("vs_computer", vs_computer);
            rk.SetValue("rutat", rutat);
            rk.SetValue("rutap", rutap);
            rk.SetValue("mostrarmov", mostrarmov);
            rk.SetValue("sound", sound);
            rk.SetValue("mostrarcoord", mostrarcoord);
            rk.SetValue("dificultad", dificultad);
            rk.SetValue("juego_p_blancas", juego_p_blancas);
        }
        void Cargar()
        {
            rk = Registry.CurrentUser.CreateSubKey("Software\\IA_Chess_Dota");
            vs_computer = Convert.ToBoolean(rk.GetValue("vs_computer", true));
            rutat = (string)rk.GetValue("rutat", "image\\board3\\");
            rutap = (string)rk.GetValue("rutap", "image\\e0\\");
            mostrarmov = Convert.ToBoolean(rk.GetValue("mostrarmov", true));
            sound = Convert.ToBoolean(rk.GetValue("sound", true));
            mostrarcoord = Convert.ToBoolean(rk.GetValue("mostrarcoord", true));
            dificultad = (int)rk.GetValue("dificultad", 1);
            juego_p_blancas = Convert.ToBoolean(rk.GetValue("juego_p_blancas", true));
        }
    }
}