using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Automower
{
    public partial class Form1 : Form
    {

        public delegate void UpdateTextCallback(string text, int index);

        public delegate void UpdateMowerCallback(int X, int Y);

        public delegate int GetTrackBarCallback();

        public delegate void SetPanelColorCallback(Color c, int index);

        public List<Panel> pList = new List<Panel>();
        public List<Label> lList = new List<Label>();
        Mower m;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            button1.BackColor = Color.ForestGreen;
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            button1.BackColor = Color.DarkRed;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar2.Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Controls.Remove(trackBar1);
            Controls.Remove(trackBar2);
            Controls.Remove(button1);
            Controls.Remove(label1);
            Controls.Remove(label2);
            Controls.Remove(label3);
            Controls.Remove(label4);
            Controls.Remove(label5);

            ProgressBar pb = new ProgressBar();
            pb.Location = new Point(85, 158);
            Controls.Add(pb);
            for (int i = 0; i < 100; i++)
            {
                pb.Value = i;
                System.Threading.Thread.Sleep(trackBar1.Value * trackBar2.Value / 10);
            }
            Controls.Remove(pb);

            for (int i = 0; i < trackBar1.Value * trackBar2.Value; i++)
            {
                pList.Add(new Panel());
                lList.Add(new Label());
                lList[i].Text = "0";
                lList[i].TextAlign = ContentAlignment.MiddleCenter;
                lList[i].Width = lList[i].Width - 50;
                pList[i].Width = 50;
                pList[i].Height = 50;
                pList[i].Location = new Point(12 + (i % trackBar1.Value) * 50, 12 + 50 * (i / trackBar1.Value));
                pList[i].BorderStyle = BorderStyle.FixedSingle;
                pList[i].Controls.Add(lList[i]);
                Controls.Add(pList[i]);
            }
            lList[0].Text = "1";
            pList[0].BackColor = Color.LawnGreen;
            this.Width = trackBar1.Value * 50 + 40;
            this.Height = trackBar2.Value * 50 + 62;

            m = new Mower();
            m.Location = new Point(12, 12);
            m.BackColor = Color.Black;
            m.Width = 50;
            m.Height = 50;
            Controls.Add(m);
            m.BringToFront();

            Thread th = new Thread(new ThreadStart(this.simulation));
            th.IsBackground = true;
            th.Start();
        }

        private void simulation()
        {
            Random rng = new Random();
            bool Everytilemowed = false, bumped = true;
            int Winkel = 0, Lastbump = 0, lasttile = 0, currenttile = 0; //Lastbump 0=oben, 1=rechts, 2=unten, 3=links
            double MowerLocationX = 12, MowerLocationY = 12;
            while (!Everytilemowed)
            {
                if (bumped)
                {
                    if (rng.Next(0, 10) > 3)
                        Winkel = rng.Next(1, 90);
                    else
                        Winkel = rng.Next(271, 360);
                    bumped = false;
                }
                switch (Lastbump)
                {
                    case 0:
                        if (Winkel <= 90)
                        {
                            if (!((MowerLocationX += Winkel / 100.0) + 50 < 12 + (int)trackBar1.Invoke(new GetTrackBarCallback(this.GetTrackBar1)) * 50 && (MowerLocationY += (90 - Winkel) / 100.0) + 50 < 12 + (int)trackBar2.Invoke(new GetTrackBarCallback(this.GetTrackBar2)) * 50))
                            {
                                bumped = true;
                                if (MowerLocationX + 50 >= 12 + (int)trackBar1.Invoke(new GetTrackBarCallback(this.GetTrackBar1)) * 50)
                                    Lastbump = 1;
                                else
                                    Lastbump = 2;
                                continue;
                            }
                        }
                        else
                        {
                            if (!((MowerLocationX -= (360 - Winkel) / 100.0) > 12 && (MowerLocationY += (Winkel - 270) / 100.0) + 50 < 12 + (int)trackBar2.Invoke(new GetTrackBarCallback(this.GetTrackBar2)) * 50))
                            {
                                bumped = true;
                                if (MowerLocationX <= 12)
                                    Lastbump = 3;
                                else
                                    Lastbump = 2;
                                continue;
                            }
                        }
                        break;
                    case 1:
                        if(Winkel <= 90)
                        {
                            if(!((MowerLocationX -= ((90 - Winkel) / 100.0)) > 12 && (MowerLocationY += Winkel / 100.0) + 50 < 12 + (int)trackBar2.Invoke(new GetTrackBarCallback(this.GetTrackBar2)) * 50))
                            {
                                bumped = true;
                                if (MowerLocationY + 50 >= 12 + (int)trackBar2.Invoke(new GetTrackBarCallback(this.GetTrackBar2)) * 50)
                                    Lastbump = 2;
                                else
                                    Lastbump = 3;
                                continue;
                            }
                        }
                        else
                        {
                            if (!((MowerLocationX -= ((Winkel - 270) / 100.0)) > 12 && (MowerLocationY -= (360 - Winkel) / 100.0) + 50 < 12))
                            {
                                bumped = true;
                                if (MowerLocationY <= 12)
                                    Lastbump = 0;
                                else
                                    Lastbump = 3;
                                continue;
                            }
                        }
                        break;
                    case 2:
                        if (Winkel <= 90)
                        {
                            if (!((MowerLocationX -= Winkel / 100.0) > 12 && (MowerLocationY -= (90 - Winkel) / 100.0) > 12))
                            {
                                bumped = true;
                                if (MowerLocationX <= 12)
                                    Lastbump = 3;
                                else
                                    Lastbump = 0;
                                continue;
                            }
                        }
                        else
                        {
                            if (!((MowerLocationX += (360 - Winkel) / 100.0) + 50 < 12 + (int)trackBar1.Invoke(new GetTrackBarCallback(this.GetTrackBar1)) * 50 && (MowerLocationY -= (Winkel - 270) / 100.0) > 12))
                            {
                                bumped = true;
                                if (MowerLocationX + 50 >= 12 + (int)trackBar1.Invoke(new GetTrackBarCallback(this.GetTrackBar1)) * 50)
                                    Lastbump = 1;
                                else
                                    Lastbump = 0;
                                continue;
                            }
                        }
                        break;
                    case 3:
                        if(Winkel <= 90)
                        {
                            if(!((MowerLocationX += (90 - Winkel) / 100.0) + 50 < 12 + (int)trackBar1.Invoke(new GetTrackBarCallback(this.GetTrackBar1)) * 50 && (MowerLocationY -= Winkel / 100.0) > 12))
                            {
                                bumped = true;
                                if (MowerLocationY <= 12)
                                    Lastbump = 0;
                                else
                                    Lastbump = 1;
                                continue;
                            }
                        }
                        else
                        {
                            if (!((MowerLocationX += (360 - Winkel) / 100.0) + 50 < 12 + (int)trackBar1.Invoke(new GetTrackBarCallback(this.GetTrackBar1)) * 50 && (MowerLocationY += (360 - Winkel) / 100.0) + 50 < 12 + (int)trackBar2.Invoke(new GetTrackBarCallback(this.GetTrackBar2)) * 50))
                            {
                                bumped = true;
                                if (MowerLocationY >= 12 + (int)trackBar2.Invoke(new GetTrackBarCallback(this.GetTrackBar2)) * 50)
                                    Lastbump = 2;
                                else
                                    Lastbump = 1;
                                continue;
                            }
                        }
                        break;
                }
                Thread.Sleep(10);
                m.Invoke(new UpdateMowerCallback(this.UpdateMower), new object[] {(int)MowerLocationX, (int)MowerLocationY});
                currenttile = (int)((MowerLocationX + 13) / 50) + ((int)(MowerLocationY + 13) / 50) * (int)trackBar1.Invoke(new GetTrackBarCallback(this.GetTrackBar1));
                if(currenttile!=lasttile)
                {
                    lasttile = currenttile;
                    lList[0].Invoke(new UpdateTextCallback(this.UpdateText),new object[] { (int.Parse(lList[lasttile].Text) + 1).ToString(), lasttile });
                    pList[0].Invoke(new SetPanelColorCallback(this.SetPanelColor), new object[] { Color.LawnGreen, lasttile });
                    Everytilemowed = checktiles();
                }

            }
            int max = 0, index = 0;
            for (int i = 0; i < lList.Count; i++)
            {
                if (int.Parse(lList[i].Text) > max)
                {
                    max = int.Parse(lList[i].Text);
                    index = i;
                }
            }
            pList[0].Invoke(new SetPanelColorCallback(this.SetPanelColor), new object[] { Color.Red, index });
        }

        private bool checktiles()
        {
            bool Everytilemowed = true;
            foreach (var temp in lList)
            {
                if (int.Parse(temp.Text) < 10)
                {
                    Everytilemowed = false;
                    break;
                }
            }
            return Everytilemowed;
        }

        private void UpdateText(string text, int index)
        {
            lList[index].Text = text;
        }

        private void UpdateMower(int X, int Y)
        {
            m.Location = new Point(X, Y);
        }

        private int GetTrackBar1()
        {
            return trackBar1.Value;
        }

        private int GetTrackBar2()
        {
            return trackBar2.Value;
        }

        private void SetPanelColor(Color c, int index)
        {
            pList[index].BackColor = c;
        }
    }
}
