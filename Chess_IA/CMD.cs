using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Chess_IA
{
    public class CMD
    {
        public static StringBuilder cmdoutput = null;
        string path = @"crafty.exe";
        public Process p;
        public StreamWriter SW;
        static int nj;
        public string TiempoS = "0.01";
        public int Tiempo = 250;
        static string player = "Black(";
        static string sal;
        System.Windows.Forms.Timer timer;
        BackgroundWorker bWWrite;

        public CMD()
        {
            sal = "";
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 250;
            timer.Enabled = false;
            timer.Tick += timer_Tick;
            bWWrite = new BackgroundWorker();
            bWWrite.DoWork += bW_DoWork;
            Form1_Load();
        }

        public void Form1_Load()
        {
            cmdoutput = new StringBuilder("");
            p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += SortOutputHandler;
            p.Start();
            SW = p.StandardInput;
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            SW.WriteLine("batch off");
            SW.WriteLine("log off");
            SW.WriteLine("ponder off");
            SW.WriteLine("output long");
            SW.WriteLine("st " + TiempoS);
        }

        public void send_command(double t)
        {
            TiempoS = t.ToString().Replace(',', '.');
            string line = "st " + TiempoS;
            Tiempo = (int)(t * 1000.0F) + 200;

            timer.Interval = Tiempo;
            SW.WriteLine(line);
        }

        public static void SortOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
                if (outLine.Data.Contains(player + nj + "): "))
                    sal = outLine.Data;
        }

        public void close()
        {
            SW.WriteLine("exit");
            p.Close();
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                bWWrite.RunWorkerAsync();
            }
            catch { }
        }

        public event EventHandler<argumento> Argg;
        public class argumento : EventArgs
        {
            string arg2;
            public argumento(string arg)
            {
                arg2 = arg;
            }
            public string arg1 { get { return arg2; } }
        }

        public void bW_DoWork(object sender, DoWorkEventArgs e)
        {
            int ttt = 1;
            do
            {
                if (ttt != 1)
                    Thread.Sleep(100);
                if (sal != "")
                {
                    timer.Enabled = false;
                    var indx = sal.IndexOf(player + nj + "): ") + (player + nj + "): ").Length;
                    var subs = sal.Substring(indx);

                    Arg = subs;
                    if (Argg != null)
                        Argg(Arg, new argumento(Arg));
                }
                ttt = 0;
            } while (timer.Enabled);
        }

        public void read(string line)
        {
            if (line == "move")
                player = ("Black(" == player) ? "White(" : "Black(";
            else if (line == "new")
            {
                SW.WriteLine(line);
                nj = 0;
            }
            else
            {
                SW.WriteLine(line);
                nj++;
                timer.Enabled = true;
            }
        }

        public string Arg { get; set; }
    }
}
