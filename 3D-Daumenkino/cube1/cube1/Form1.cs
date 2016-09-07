/*
 * 3D Daumenkino Factory
 * Editor fuer animierte 3D-Vektorgrafik aus Frames
 * Autor: A. Warta
 * Version:0.2
 * 
 * Copyright Andreas Warta 2015
 * 
 * This file is part of 3D-Daumenkino.
 * 
 * 3D-Daumenkino is free software: you can redistribute it and/or modify   
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or  
 * (at your option) any later version.
 * 3D-Daumenkino is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * You should have received a copy of the GNU General Public License
 * along with 3D-Daumenkino.  If not, see <http://www.gnu.org/licenses/>.
 
 * Diese Datei ist Teil von 3D-Daumenkino.
 * 
 * 3D-Daumenkino ist Freie Software: Sie können es unter den Bedingungen
 * der GNU General Public License, wie von der Free Software Foundation,
 * Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren
 * veröffentlichten Version, weiterverbreiten und/oder modifizieren.
 * 3D-Daumenkino wird in der Hoffnung, dass es nützlich sein wird, aber
 * OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite
 * Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK.
 * Siehe die GNU General Public License für weitere Details.
 * Sie sollten eine Kopie der GNU General Public License zusammen mit 3D-Daumenkino erhalten haben. 
 * Wenn nicht, siehe <http://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Windows.Forms.VisualStyles;

namespace cube1
{
    public partial class Form1 : Form
    {
        // Multithreading Vorbereitung
        private ThreadStart del = null;
        private Thread thread = null;

        // Grafikvorbereitung
        private Pen myPen = new Pen(Color.White);
        private Graphics g = null;

        // Variablen fuer Positionskorrektur
        private static int center_x, center_y;

        // Kontrollvariablen
        private static double size = 13;
        private static double xsize = 1;
        private static double ysize = 1;
        private static double zsize = 1;

        private static int distanz = 450;
        private static int cdistanz = 500;
        private static double angle = 0;
        private static int height = -50;
        private static int speed = 1;
        private static int viewpoint = -50;

        private static bool dauerdrehen = true;
        private static bool animate = false;

        private static bool grid = false;
        private static bool edit = false;

        private static int cursorx = 0;
        private static int cursory = 0;
        private static int cursorz = 0;

        private static int modelnr = 2;

        private static int activep = 0;
        private static int activev = 0;
        private static int validvek = -1;

        private static int animstart = 1;
        private static int animend = 8;

        private static My_Modell[] m = new My_Modell[51]
        {
            new My_Modell(0), new My_Modell(1), new My_Modell(2),
            new My_Modell(2), new My_Modell(2), new My_Modell(2), new My_Modell(2), new My_Modell(2),
            new My_Modell(2), new My_Modell(2), new My_Modell(0), new My_Modell(0), new My_Modell(0),

            new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0),
            new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0),
            new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0),
            new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0),

            new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0),
            new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0),
            new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0), new My_Modell(0),
            new My_Modell(0), new My_Modell(0), new My_Modell(0)
        };

        private static My_Modell gridmodel = new My_Modell(99);
        private static My_Modell buffermodell = new My_Modell(0);

        // Arrays fuer Koordinaten der Punkte auf der Projektionsebene
        private static int[] cpx = new int[250];
        private static int[] cpy = new int[250];

        // Initialisierung des Fensters
        public Form1()
        {
            InitializeComponent();
            center_x = canvas.Width/2;
            center_y = canvas.Height/2;
            myPen.Width = 1;

            labelcx.Text = "Cx: " + cursorx;
            labelcy.Text = "Cy: " + cursory;
            labelcz.Text = "Cz: " + cursorz;

            custom.Text = "Animation Frame 1";

            aktpkt.Text = "P: " + activep;
            pktx.Text = "X: " + m[modelnr].getPointX(activep);
            pkty.Text = "Y: " + m[modelnr].getPointY(activep);
            pktz.Text = "Y: " + m[modelnr].getPointZ(activep);

            veknr.Text = "V: " + activev;
            vekp1.Text = "a: " + m[modelnr].getVek1(activev);
            vekp2.Text = "b: " + m[modelnr].getVek2(activev);

            range.Text = "Startframe: " + animstart + " Endframe: " + animend;
            
            // Animierten Punkt setzen
            m[2].setPointY(0, -1);
            m[3].setPointY(0, -1);
            m[4].setPointZ(0, -1);
            m[5].setPointZ(0, -1);
            m[6].setPointY(0, 1);
            m[7].setPointY(0, 1);
            m[8].setPointZ(0, 1);
            m[9].setPointZ(0, 1);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // Timer für autorefresh
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 70;
            timer1.Tick += new System.EventHandler(timer1_Tick);
            timer1.Start();

            // Tooltips fuer Button
            ToolTip toolTip1 = new ToolTip();

            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;

            toolTip1.SetToolTip(this.distplus, "Entfernung vergroessern");
            toolTip1.SetToolTip(this.distminus, "Entfernung verkleinern");
            toolTip1.SetToolTip(this.camplus, "Brennweite vergroessern");
            toolTip1.SetToolTip(this.camminus, "Brennweite verkleinern");
            toolTip1.SetToolTip(this.angplus, "Nach links drehen");
            toolTip1.SetToolTip(this.angminus, "Nach rechts drehen");
            toolTip1.SetToolTip(this.up, "Kamera hoeher");
            toolTip1.SetToolTip(this.down, "Kamera niedriger");
            toolTip1.SetToolTip(this.toggle, "Naechster Frame");
            toolTip1.SetToolTip(this.tiggle, "Voriger Frame");
            toolTip1.SetToolTip(this.startplus, "Startframe + 1");
            toolTip1.SetToolTip(this.startminus, "Startframe - 1");
            toolTip1.SetToolTip(this.endplus, "Endframe + 1");
            toolTip1.SetToolTip(this.endminus, "Endframe - 1");
            toolTip1.SetToolTip(this.speedplus, "Drehwinkel pro Frame erhoehen");
            toolTip1.SetToolTip(this.speedminus, "Drehwinkel pro Frame verringern");
            toolTip1.SetToolTip(this.turnbutton, "Automatisches Rotieren bei Animation ein/aus");
            toolTip1.SetToolTip(this.rotthread, "Animation ein/aus");
            toolTip1.SetToolTip(this.vup, "Hoehenkorrektur nach oben");
            toolTip1.SetToolTip(this.vdwn, "Hoehenkorrektur nach unten");
            toolTip1.SetToolTip(this.xplus, "X-Achse strecken");
            toolTip1.SetToolTip(this.xminus, "X-Achse stauchen");
            toolTip1.SetToolTip(this.yplus, "Y-Achse strecken");
            toolTip1.SetToolTip(this.yminus, "Y-Achse stauchen");
            toolTip1.SetToolTip(this.zplus, "Z-Achse strecken");
            toolTip1.SetToolTip(this.zminus, "Z-Achse stauchen");
            toolTip1.SetToolTip(this.xplus, "X-Achse strecken");
            toolTip1.SetToolTip(this.set0, "Drehwinkel zuruecksetzen");
            toolTip1.SetToolTip(this.xplus, "X-Achse strecken");
            toolTip1.SetToolTip(this.gridbutton, "Editieren an/aus");
            toolTip1.SetToolTip(this.gridon, "3D-Matrix an/aus");
            toolTip1.SetToolTip(this.delvek, "Aktiven Vektor loeschen");
            toolTip1.SetToolTip(this.delpkt, "Aktiven Punkt loeschen");
            toolTip1.SetToolTip(this.save, "Frame speichern");
            toolTip1.SetToolTip(this.load, "Frame laden");
            toolTip1.SetToolTip(this.animsave, "In dieser Version leider nicht verfuegbar.");
            toolTip1.SetToolTip(this.animload, "In dieser Version leider nicht verfuegbar.");
            toolTip1.SetToolTip(this.curxplus, "Cursor X-Position + 1");
            toolTip1.SetToolTip(this.curxminus, "Cursor X-Position - 1");
            toolTip1.SetToolTip(this.curyplus, "Cursor Y-Position + 1");
            toolTip1.SetToolTip(this.curyminus, "Cursor Y-Position - 1");
            toolTip1.SetToolTip(this.curzplus, "Cursor Z-Position + 1");
            toolTip1.SetToolTip(this.curzminus, "Cursor Z-Position - 1");
            toolTip1.SetToolTip(this.ctop, "Cursor an Position des aktiven Punktes");
            toolTip1.SetToolTip(this.addpkt, "Punkt an Cursorposition hinzufuegen");
            toolTip1.SetToolTip(this.addvektor, "Vorgeschlagenen Vektor hinzufuegen");
            toolTip1.SetToolTip(this.pktplus, "Aktiver Punkt + 1");
            toolTip1.SetToolTip(this.pktminus, "Aktiver Punkt - 1");
            toolTip1.SetToolTip(this.vektorplus, "Aktiver Vektor + 1");
            toolTip1.SetToolTip(this.vektorminus, "Aktiver Vektor - 1");
            toolTip1.SetToolTip(this.p1plus, "Punkt a des aktiven Vektors + 1");
            toolTip1.SetToolTip(this.p1minus, "Punkt a des aktiven Vektors - 1");
            toolTip1.SetToolTip(this.p2plus, "Punkt b des aktiven Vektors + 1");
            toolTip1.SetToolTip(this.p2minus, "Punkt b des aktiven Vektors - 1");
            toolTip1.SetToolTip(this.pktXplus, "X-Koordinate des Aktiven Punkts + 1");
            toolTip1.SetToolTip(this.pktXminus, "X-Koordinate des Aktiven Punkts - 1");
            toolTip1.SetToolTip(this.pktYplus, "Y-Koordinate des Aktiven Punkts + 1");
            toolTip1.SetToolTip(this.pktYminus, "Y-Koordinate des Aktiven Punkts - 1");
            toolTip1.SetToolTip(this.pktZplus, "Z-Koordinate des Aktiven Punkts + 1");
            toolTip1.SetToolTip(this.pktZminus, "Z-Koordinate des Aktiven Punkts - 1");
            toolTip1.SetToolTip(this.neu, "Frame zuruecksetzen");
            toolTip1.SetToolTip(this.copy, "MFrame kopieren");
            toolTip1.SetToolTip(this.paste, "Frame einfuegen");
            toolTip1.SetToolTip(this.resetcamera, "Kameraeinstellungen zuruecksetzen");
            toolTip1.SetToolTip(this.resetscale, "Skalierung zuruecksetzen");
            toolTip1.SetToolTip(this.resetview, "Hoehenkorrektur zuruecksetzen");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (animate)
            {
                canvas.Refresh();
            }
        }

        // Kamera zuruecksetzen
        private void resetcamera_Click(object sender, EventArgs e)
        {
            distanz = 450;
            cdistanz = 500;
            height = -50;
            canvas.Refresh();
        }

        // Hoehenkorrektur zuruecksetzen
        private void resetview_Click(object sender, EventArgs e)
        {
            viewpoint = -50;
            canvas.Refresh();
        }

        // Skalierung zuruecksetzen
        private void resetscale_Click(object sender, EventArgs e)
        {
            xsize = 1;
            ysize = 1;
            zsize = 1;
            canvas.Refresh();
        }

        // X konvertieren
        private int convertX(int xk, int zk)
        {
            return Convert.ToInt32(Math.Round((xk * size * xsize * Math.Cos(angle)
            + (zk * size * zsize) * Math.Sin(angle)) * (cdistanz / ((0 - xk * size * xsize) * Math.Sin(angle)
            + ((zk * size * zsize) * Math.Cos(angle)) + distanz))) + center_x);
        }

        // Y konvertieren
        private int convertY(int xk, int yk, int zk)
        {
            return Convert.ToInt32(Math.Round((yk * size * ysize - height) * (cdistanz / ((0 - xk * size * xsize)
                * Math.Sin(angle) + ((zk * size * zsize) * Math.Cos(angle)) + distanz))) + center_y) + height + viewpoint;
        }

        // Paint Methode zum Durchfuehren der 3D-Projektion
        private void canvas_Paint(object sender, PaintEventArgs e)
        {

            g = canvas.CreateGraphics();

            center_x = canvas.Width/2;
            center_y = canvas.Height/2;

            // Konvertierung der 3D-Punkte des Modells  
            // und Anpassungen aus Kontrollvariablen -> 2D-Koordinaten
            for (int i = 0; i < m[modelnr].getPunkte(); i++)
            {
                cpx[i] = convertX(m[modelnr].getPointX(i), m[modelnr].getPointZ(i));
                cpy[i] = convertY(m[modelnr].getPointX(i), m[modelnr].getPointY(i), m[modelnr].getPointZ(i));
            }
            DrawCube(m[modelnr]);


            // Konvertierung der 3D-Punkte des Gitters und des Cursors  
            // und Anpassungen aus Kontrollvariablen -> 2D-Koordinaten
            if (true)
            {
                if (edit)
                {
                    for (int i = 0; i < gridmodel.getPunkte(); i++)
                    {
                        cpx[i] = convertX(gridmodel.getPointX(i), gridmodel.getPointZ(i));
                        cpy[i] = convertY(gridmodel.getPointX(i), gridmodel.getPointY(i), gridmodel.getPointZ(i));
                    }
                    DrawGrid(gridmodel);
                }

                int x = convertX(cursorx, cursorz);
                int y = convertY(cursorx, cursory, cursorz);

                // Aktiven Vektor anzeigen

                if (grid)
                {
                    int x1 = convertX(m[modelnr].getPointX(m[modelnr].getVek1(activev)),
                        m[modelnr].getPointZ(m[modelnr].getVek1(activev)));
                    int y1 = convertY(m[modelnr].getPointX(m[modelnr].getVek1(activev)),
                        m[modelnr].getPointY(m[modelnr].getVek1(activev)),
                        m[modelnr].getPointZ(m[modelnr].getVek1(activev)));

                    int x2 = convertX(m[modelnr].getPointX(m[modelnr].getVek2(activev)),
                        m[modelnr].getPointZ(m[modelnr].getVek2(activev)));
                    int y2 = convertY(m[modelnr].getPointX(m[modelnr].getVek2(activev)),
                        m[modelnr].getPointY(m[modelnr].getVek2(activev)),
                        m[modelnr].getPointZ(m[modelnr].getVek2(activev)));

                    myPen.Width = 1;
                    myPen.Color = Color.HotPink;

                    drawLine(x1, y1, x2, y2);
                }

                // Aktiven Punkt anzeigen
                if (grid)
                {
                    int x4 = convertX(m[modelnr].getPointX(activep), m[modelnr].getPointZ(activep));
                    int y4 = convertY(m[modelnr].getPointX(activep), m[modelnr].getPointY(activep),
                        m[modelnr].getPointZ(activep));

                    myPen.Width = 1;
                    myPen.Color = Color.HotPink;

                    drawLine(x4 - 4, y4 - 4, x4 + 4, y4 + 4);
                    drawLine(x4 - 4, y4 + 4, x4 + 4, y4 - 4);
                }

                // Moeglichen Vektor anzeigen
                if (grid)
                {
                    if (validvek != -1)
                    {
                        myPen.Width = 2;
                        myPen.Color = Color.LightBlue;

                        int x3 = convertX(m[modelnr].getPointX(activep), m[modelnr].getPointZ(activep));
                        int y3 = convertY(m[modelnr].getPointX(activep), m[modelnr].getPointY(activep),
                            m[modelnr].getPointZ(activep));

                        drawLine(x, y, x3, y3);
                    }

                    myPen.Width = 1;
                    myPen.Color = Color.Red;

                    drawLine(x - 5, y, x + 5, y);
                    drawLine(x, y + 5, x, y - 5);
                }
            }
        }

       

        private void DrawCube(My_Modell m)
        {
            myPen.Width = 1;
            myPen.Color = Color.White;

            for (int i = 0; i < m.getVektoren(); i++)
            {
                drawLine(cpx[m.getVek1(i)], cpy[m.getVek1(i)], cpx[m.getVek2(i)], cpy[m.getVek2(i)]);
            }
        }

        private void DrawGrid(My_Modell m)
        {
            myPen.Width = 1;
            myPen.Color = Color.LightGray;

            for (int i = 0; i < m.getPunkte(); i++)
            {
                drawLine(cpx[i] - 1, cpy[i], cpx[i] + 1, cpy[i]);
                drawLine(cpx[i], cpy[i] + 1, cpx[i], cpy[i] - 1);
            }
        }


        private void drawLine(int x1, int y1, int x2, int y2)
        {

            // Linie zeichnen
            Point[] points =
            {
                new Point(x1, y1),
                new Point(x2, y2)
            };

            g.DrawLines(myPen, points);

        }

        // Entfernung vergroessern
        private void distplus_Click(object sender, EventArgs e)
        {
            distanz += 10;
            canvas.Refresh();
        }

        // Entfernung verringern
        private void distminus_Click(object sender, EventArgs e)
        {
            distanz -= 10;
            canvas.Refresh();
        }

        // Brennweite vergroessern
        private void camplus_Click(object sender, EventArgs e)
        {
            cdistanz += 10;
            canvas.Refresh();
        }

        // Brennweite verringern
        private void camminus_Click(object sender, EventArgs e)
        {
            cdistanz -= 10;
            canvas.Refresh();
        }

        // Wuerfel nach links drehen
        private void angplus_Click(object sender, EventArgs e)
        {
            angle += (Math.PI/36);
            canvas.Refresh();
        }

        // Wuerfel nach rechts drehen
        private void angminus_Click(object sender, EventArgs e)
        {
            angle -= (Math.PI/36);
            canvas.Refresh();
        }

        // Animieren
        private void turnbutton_Click(object sender, EventArgs e)
        {
            if (dauerdrehen)
            {
                dauerdrehen = false;
            }
            else
            {
                dauerdrehen = true;
            }
        }

        // Animation
        private void RotateThreadStart()
        {
            Animiere();
        }

        private void rotthread_Click(object sender, EventArgs e)
        {
            if (animate)
            {
                animate = false;
                thread.Abort();
                canvas.Refresh();
            }
            else
            {
                animate = true;
                thread = new Thread(new ThreadStart(RotateThreadStart));
                thread.Start();
            }
        }

        private static void Animiere()
        {
            while (true)
            {
                if (dauerdrehen)
                {
                    angle -= (Math.PI/36)*speed;
                }

                if (modelnr >= (animstart +1))
                {
                    modelnr++;
                }

                if (modelnr > animend + 1)
                {
                    modelnr = animstart + 1;
                }
                
                Thread.Sleep(70);
            }
        }

        // Rotation Speed
        private void speedplus_Click(object sender, EventArgs e)
        {
            speed += 1;
            canvas.Refresh();
        }

        private void speedminus_Click(object sender, EventArgs e)
        {
            speed -= 1;
            canvas.Refresh();
        }

        // Anzeige zwischen Modellen wechseln
        private void toggle_Click(object sender, EventArgs e)
        {
            modelnr += 1;
            if (modelnr > 50)
            {
                modelnr = 0;
            }
            if (modelnr == 0)
            {
                custom.Text = "Editor";
                grid = true;
            }
            
            if (modelnr > 0 && modelnr < 51)
            {
                custom.Text = "Animation Frame " + (modelnr - 1);
                grid = false;
            }

            canvas.Refresh();
        }

        private void tiggle_Click(object sender, EventArgs e)
        {
            modelnr -= 1;
            if (modelnr < 0)
            {
                modelnr = 50;
            }
            if (modelnr == 0)
            {
                custom.Text = "Editor";
                grid = true;
            }
            
            if (modelnr > 1 && modelnr < 51)
            {
                custom.Text = "Animation Frame " + (modelnr - 1);
                grid = false;
            }

            canvas.Refresh();
        }

        // Kamera anheben
        private void up_Click(object sender, EventArgs e)
        {
            height -= 10;
            if (height < -150)
                height = -150;
            canvas.Refresh();
        }

        // Kamera senken
        private void down_Click(object sender, EventArgs e)
        {
            height += 10;
            if (height > 150)
                height = 150;
            canvas.Refresh();
        }

        // Grid ein/ausschalten
        private void gridbutton_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                grid = false;
            }
            else
            {
                grid = true;
            }
            canvas.Refresh();
        }

        // Cursorposition ändern
        private void curxplus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                cursorx++;

                if (cursorx > 10)
                    cursorx = 10;
                labelcx.Text = "Cx: " + cursorx;
                validcheck();
                canvas.Refresh();
            }
        }

        private void curxminus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                cursorx--;

                if (cursorx < -10)
                    cursorx = -10;
                labelcx.Text = "Cx: " + cursorx;
                validcheck();
                canvas.Refresh();
            }
        }

        private void curyplus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                cursory++;

                if (cursory > 10)
                    cursory = 10;
                labelcy.Text = "Cy: " + cursory;
                validcheck();
                canvas.Refresh();
            }
        }

        private void curyminus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                cursory--;

                if (cursory < -10)
                    cursory = -10;
                labelcy.Text = "Cy: " + cursory;
                validcheck();
                canvas.Refresh();
            }
        }

        private void curzplus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                cursorz++;

                if (cursorz > 10)
                    cursorz = 10;
                labelcz.Text = "Cz: " + cursorz;
                validcheck();
                canvas.Refresh();
            }
        }

        private void curzminus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                cursorz--;

                if (cursorz < -10)
                    cursorz = -10;
                labelcz.Text = "Cz: " + cursorz;
                validcheck();
                canvas.Refresh();
            }
        }

        // Pruefen ob zwischen dem Cursor und dem aktiven Punkt ein Vektor
        // gezogen werden kann
        private void validcheck()
        {
            validvek = -1;
            int checkp = -1;

            for (int i = 0; i < m[modelnr].getPunkte(); i++)
            {
                if (m[modelnr].getPointX(i) == cursorx
                    && m[modelnr].getPointY(i) == cursory
                    && m[modelnr].getPointZ(i) == cursorz)
                {
                    checkp = i;
                }
            }

            if (checkp != -1)
            {
                for (int i = 0; i < m[modelnr].getVektoren(); i++)
                {
                    if ((m[modelnr].getVek1(i) == activep && m[modelnr].getVek2(i) == checkp)
                        || (m[modelnr].getVek2(i) == activep && m[modelnr].getVek1(i) == checkp))
                    {
                        validvek = -1;
                    }
                    else
                    {
                        if (m[modelnr].getVektoren() < 250)
                            validvek = checkp;
                    }
                }
            }
        }

        //Achsen skalieren
        private void xplus_Click(object sender, EventArgs e)
        {
            xsize += .1;
            if (xsize > 5)
                xsize = 5;
            canvas.Refresh();
        }

        private void xminus_Click(object sender, EventArgs e)
        {
            xsize -= .1;
            canvas.Refresh();
        }

        private void yplus_Click(object sender, EventArgs e)
        {
            ysize += .1;
            canvas.Refresh();
        }

        private void yminus_Click(object sender, EventArgs e)
        {
            ysize -= .1;
            canvas.Refresh();
        }

        private void zplus_Click(object sender, EventArgs e)
        {
            zsize += .1;
            canvas.Refresh();
        }

        private void zminus_Click(object sender, EventArgs e)
        {
            zsize -= .1;
            canvas.Refresh();
        }

        // Rotation zuruecksetzen
        private void set0_Click(object sender, EventArgs e)
        {
            angle = 0;
            canvas.Refresh();
        }

        // Bei Fenster Resize Grafik neu zeichnen
        private void Form1_Resize(object sender, EventArgs e)
        {
            canvas.Refresh();
        }

        // Modell aus file laden
        private void load_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "3d vector models|*.3dvm";
            openFileDialog1.Title = "3D-Modell laden";
            openFileDialog1.DefaultExt = "3dvm";

            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    byte[] modeldata = File.ReadAllBytes(file);

                    //Debug
                    Console.WriteLine("Loading " + modeldata.Length + " Bytes");
                    Console.WriteLine("Gespeicherte Bytes:");
                    for (int i = 0; i < modeldata.Length; i++)
                        Console.WriteLine(modeldata[i]);

                    m[modelnr].setPunkte(modeldata[0]);
                    m[modelnr].setVektoren(modeldata[1]);

                    for (int i = 0; i < modeldata[0]; i++)
                    {
                        m[modelnr].setPointX(i, modeldata[i + 2] - 127);
                        m[modelnr].setPointY(i, modeldata[i + 2 + modeldata[0]] - 127);
                        m[modelnr].setPointZ(i, modeldata[i + 2 + (2 * modeldata[0])] - 127);
                    }

                    for (int i = 0; i < modeldata[1]; i++)
                    {
                        m[modelnr].setVek1(i, modeldata[i + 2 + (3 * modeldata[0])]);
                    }

                    for (int i = 0; i < modeldata[1]; i++)
                    {
                        m[modelnr].setVek2(i, modeldata[i + 2 + (3 * modeldata[0]) + modeldata[1]]);
                    }
                }
                catch (IOException)
                {
                }
               
            }
            canvas.Refresh();
        }

        // Modell in file speichern
        private void save_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "3d vector models|*.3dvm";
            saveFileDialog1.Title = "3D-Modell speichern";
            saveFileDialog1.DefaultExt = "3dvm";
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string name = saveFileDialog1.FileName;

            byte[] encmodel = new byte[2 + m[modelnr].getPunkte()*3 + m[modelnr].getVektoren()*2];
            Console.WriteLine("Saving " + encmodel.Length + " Bytes");

            encmodel[0] = (byte) m[modelnr].getPunkte();
            encmodel[1] = (byte) m[modelnr].getVektoren();

            for (int i = 0; i < encmodel[0]; i++)
            {
                encmodel[i + 2] = (byte) (m[modelnr].getPointX(i) + 127);
            }

            for (int i = 0; i < encmodel[0]; i++)
            {
                encmodel[i + 2 + encmodel[0]] = (byte) (m[modelnr].getPointY(i) + 127);
            }

            for (int i = 0; i < encmodel[0]; i++)
            {
                encmodel[i + 2 + (encmodel[0]*2)] = (byte) (m[modelnr].getPointZ(i) + 127);
            }

            for (int i = 0; i < encmodel[1]; i++)
            {
                encmodel[i + 2 + (encmodel[0]*3)] = (byte) m[modelnr].getVek1(i);
            }

            for (int i = 0; i < encmodel[1]; i++)
            {
                encmodel[i + 2 + (encmodel[0]*3) + encmodel[1]]
                    = (byte) m[modelnr].getVek2(i);
            }

            //Debug
            Console.WriteLine("Gespeicherte Bytes:");
            for (int i = 0; i < encmodel.Length; i++)
                Console.WriteLine(encmodel[i]);

            File.WriteAllBytes(name, encmodel);
            MessageBox.Show("Modell gespeichert");
        }

        // Punkt hinzufuegen
        private void addpkt_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                m[modelnr].setPunkte(m[modelnr].getPunkte() + 1);
                if (m[modelnr].getPunkte() > 250)
                {
                    m[modelnr].setPunkte(250);
                    MessageBox.Show("Maximale Punktezahl. Punkt hinzufuegen nicht moeglich.");
                }
                else
                {
                    bool given = false;
                    for (int i = 0; i < m[modelnr].getPunkte(); i++)
                    {
                        if (m[modelnr].getPointX(i) == cursorx
                            && m[modelnr].getPointY(i) == cursory
                            && m[modelnr].getPointZ(i) == cursorz)
                        {
                            given = true;
                        }
                    }

                    if (given == true)
                    {
                        m[modelnr].setPunkte(m[modelnr].getPunkte() - 1);
                        MessageBox.Show("Punkt bereits vorhanden!");
                    }
                    else
                    {
                        int helper = activep;
                        activep = m[modelnr].getPunkte() - 1;
                        m[modelnr].setPointX(activep, cursorx);
                        m[modelnr].setPointY(activep, cursory);
                        m[modelnr].setPointZ(activep, cursorz);
                        activep = helper;
                        aktpkt.Text = "P: " + activep;
                        pktx.Text = "X: " + m[modelnr].getPointX(activep);
                        pkty.Text = "Y: " + m[modelnr].getPointY(activep);
                        pktz.Text = "Y: " + m[modelnr].getPointZ(activep);
                        validcheck();
                        canvas.Refresh();
                    }
                }
            }
        }

        // Vektor hinzufuegen
        private void addvektor_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                validcheck();
                if (grid && validvek != -1)
                {
                    if (m[modelnr].getVektoren() == 1 && m[modelnr].getVek1(0) == 0 && m[modelnr].getVek2(0) == 0)
                    {
                        m[modelnr].setVek1(0, activep);
                        m[modelnr].setVek2(0, validvek);
                    }
                    else
                    {
                        m[modelnr].setVektoren(m[modelnr].getVektoren() + 1);
                        m[modelnr].setVek1(m[modelnr].getVektoren() - 1, activep);
                        m[modelnr].setVek2(m[modelnr].getVektoren() - 1, validvek);
                    }
                }
                validcheck();
                canvas.Refresh();
            }
        }

        // Naechsten Punkt aktivieren
        private void pktplus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                activep++;
                if (activep == m[modelnr].getPunkte())
                {
                    activep = 0;
                }
                aktpkt.Text = "P: " + activep;
                pktx.Text = "X: " + m[modelnr].getPointX(activep);
                pkty.Text = "Y: " + m[modelnr].getPointY(activep);
                pktz.Text = "Y: " + m[modelnr].getPointZ(activep);
                validcheck();
                canvas.Refresh();
            }
        }

        // Vorigen Punkt aktivieren
        private void pktminus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                activep--;
                if (activep < 0)
                {
                    activep = m[modelnr].getPunkte() - 1;
                }
                aktpkt.Text = "P: " + activep;
                pktx.Text = "X: " + m[modelnr].getPointX(activep);
                pkty.Text = "Y: " + m[modelnr].getPointY(activep);
                pktz.Text = "Y: " + m[modelnr].getPointZ(activep);
                validcheck();
                canvas.Refresh();
            }
        }

        // Punktkoordinate X + 1 fuer aktiven Punkt
        private void pktXplus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (m[modelnr].getPointX(activep) < 11)
                {
                    m[modelnr].setPointX(activep, m[modelnr].getPointX(activep) + 1);
                    pktx.Text = "X: " + m[modelnr].getPointX(activep);
                    canvas.Refresh();
                }
            }
        }

        // Punktkoordinate X - 1 fuer aktiven Punkt
        private void pktXminus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (m[modelnr].getPointX(activep) > -11)
                {
                    m[modelnr].setPointX(activep, m[modelnr].getPointX(activep) - 1);
                    pktx.Text = "X: " + m[modelnr].getPointX(activep);
                    canvas.Refresh();
                }
            }
        }

        // Punktkoordinate Y + 1 fuer aktiven Punkt
        private void pktYplus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (m[modelnr].getPointY(activep) < 11)
                {
                    m[modelnr].setPointY(activep, m[modelnr].getPointY(activep) + 1);
                    pkty.Text = "Y: " + m[modelnr].getPointY(activep);
                    canvas.Refresh();
                }
            }
        }

        // Punktkoordinate Y - 1 fuer aktiven Punkt
        private void pktYminus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (m[modelnr].getPointY(activep) > -11)
                {
                    m[modelnr].setPointY(activep, m[modelnr].getPointY(activep) - 1);
                    pkty.Text = "Y: " + m[modelnr].getPointY(activep);
                    canvas.Refresh();
                }
            }
        }

        // Punktkoordinate Z + 1 fuer aktiven Punkt
        private void pktZplus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (m[modelnr].getPointZ(activep) < 11)
                {
                    m[modelnr].setPointZ(activep, m[modelnr].getPointZ(activep) + 1);
                    pktz.Text = "Z: " + m[modelnr].getPointZ(activep);
                    canvas.Refresh();
                }
            }
        }

        // Punktkoordinate Z - 1 fuer aktiven Punkt
        private void pktZminus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (m[modelnr].getPointZ(activep) > -11)
                {
                    m[modelnr].setPointZ(activep, m[modelnr].getPointZ(activep) - 1);
                    pktz.Text = "Z: " + m[modelnr].getPointZ(activep);
                    canvas.Refresh();
                }
            }
        }

        // Naechsten Vektor aktivieren
        private void vektorplus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (activev < m[modelnr].getVektoren() - 1)
                {
                    activev++;
                }
                else
                {
                    activev = 0;
                }
                veknr.Text = "V: " + activev;
                canvas.Refresh();
            }
        }

        // Vorigen Vektor aktivieren
        private void vektorminus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (activev > 0)
                {
                    activev--;
                }
                else
                {
                    activev = m[modelnr].getVektoren() - 1;
                }
                veknr.Text = "V: " + activev;
                canvas.Refresh();
            }
        }

        // Cursor zum aktiven Punkt
        private void ctop_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                cursorx = m[modelnr].getPointX(activep);
                cursory = m[modelnr].getPointY(activep);
                cursorz = m[modelnr].getPointZ(activep);
                labelcx.Text = "Cx: " + cursorx;
                labelcy.Text = "Cy: " + cursory;
                labelcz.Text = "Cz: " + cursorz;
                validcheck();
                canvas.Refresh();
            }
        }

        // Aktiver Vektor p1 + 1
        private void p1plus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (m[modelnr].getVek1(activev) == m[modelnr].getVektoren() - 1)
                {
                    m[modelnr].setVek1(activev, 0);
                }
                else
                {
                    m[modelnr].setVek1(activev, m[modelnr].getVek1(activev) + 1);
                }
                canvas.Refresh();
            }
        }

        // Aktiver Vektor p1 - 1
        private void p1minus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (m[modelnr].getVek1(activev) == 0)
                {
                    m[modelnr].setVek1(activev, m[modelnr].getVektoren() - 1);
                }
                else
                {
                    m[modelnr].setVek1(activev, m[modelnr].getVek1(activev) - 1);
                }
                canvas.Refresh();
            }
        }

        // Aktiver Vektor p2 + 1
        private void p2plus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (m[modelnr].getVek2(activev) == m[modelnr].getVektoren() - 1)
                {
                    m[modelnr].setVek2(activev, 0);
                }
                else
                {
                    m[modelnr].setVek2(activev, m[modelnr].getVek2(activev) + 1);
                }
                canvas.Refresh();
            }
        }

        // Aktiver Vektor p2 - 1
        private void p2minus_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                if (m[modelnr].getVek2(activev) == 0)
                {
                    m[modelnr].setVek2(activev, m[modelnr].getVektoren() - 1);
                }
                else
                {
                    m[modelnr].setVek2(activev, m[modelnr].getVek2(activev) - 1);
                }
                canvas.Refresh();
            }
        }

        // Aktiven Vektor loeschen
        private void delvek_Click(object sender, EventArgs e)
        {
            if (grid)
            {
                DialogResult result = MessageBox.Show("Aktiven Vektor wirklich loeschen?","Achtung!", 
                    MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (m[modelnr].getVektoren() == 1)
                    {
                        m[modelnr].setVek1(0, 0);
                        m[modelnr].setVek2(0, 0);
                    }
                    else
                    {
                        m[modelnr].setVektoren(m[modelnr].getVektoren() - 1);

                        for (int i = activev; i < m[modelnr].getVektoren(); i++)
                        {
                            m[modelnr].setVek1(i, m[modelnr].getVek1(i + 1));
                            m[modelnr].setVek2(i, m[modelnr].getVek2(i + 1));
                        }
                    }
                    validcheck();
                    canvas.Refresh();
                }
            }
        }

        // Ansicht heben
        private void vup_Click(object sender, EventArgs e)
        {
            viewpoint -= 10;
            canvas.Refresh();
        }

        // Ansicht senken
        private void vdwn_Click(object sender, EventArgs e)
        {
            viewpoint += 10;
            canvas.Refresh();
        }

        // Aktiven Punkt loeschen
        private void delpkt_Click(object sender, EventArgs e)
        {
            bool connected = false;

            for (int i = 0; i < m[modelnr].getPunkte(); i++)
            {
                if (m[modelnr].getVek1(i) == activep || m[modelnr].getVek2(i) == activep)
                {
                    connected = true;
                }
            }

            if (connected)
            {
                MessageBox.Show("Bitte erst verbundene Vektoren loeschen!");
            }
            else
            {
               
                DialogResult result = MessageBox.Show("Aktiven Punkt wirklich loeschen?","Achtung!", 
                    MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    for (int i = 0; i < m[modelnr].getVektoren(); i++)
                    {
                        if (m[modelnr].getVek1(i) > activep)
                        {
                            m[modelnr].setVek1(i, m[modelnr].getVek1(i) - 1);
                        }
                        if (m[modelnr].getVek2(i) > activep)
                        {
                            m[modelnr].setVek2(i, m[modelnr].getVek2(i) - 1);
                        }
                    }


                    for (int i = activep; i < m[modelnr].getPunkte(); i++)
                    {
                        m[modelnr].setPointX(i, m[modelnr].getPointX(i + 1));
                        m[modelnr].setPointY(i, m[modelnr].getPointY(i + 1));
                        m[modelnr].setPointZ(i, m[modelnr].getPointZ(i + 1));
                    }
                    m[modelnr].setPunkte(m[modelnr].getPunkte() - 1);
                }

            }
            validcheck();
            canvas.Refresh();
        }

        private void copy_Click(object sender, EventArgs e)
        {
            buffermodell.setPunkte(m[modelnr].getPunkte());
            buffermodell.setVektoren(m[modelnr].getVektoren());

            for (int i = 0; i < buffermodell.getPunkte(); i++)
            {
                buffermodell.setPointX(i, m[modelnr].getPointX(i));
                buffermodell.setPointY(i, m[modelnr].getPointY(i));
                buffermodell.setPointZ(i, m[modelnr].getPointZ(i));
            }

            for (int i = 0; i < buffermodell.getVektoren(); i++)
            {
                buffermodell.setVek1(i, m[modelnr].getVek1(i));
                buffermodell.setVek2(i, m[modelnr].getVek2(i));
            }
        }

        private void paste_Click(object sender, EventArgs e)
        {
            
                DialogResult result = MessageBox.Show("Modell/Frame wirklich ueberschreiben?", "Achtung!",
                    MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    m[modelnr].setPunkte(buffermodell.getPunkte());
                    m[modelnr].setVektoren(buffermodell.getVektoren());

                    for (int i = 0; i < buffermodell.getPunkte(); i++)
                    {
                        m[modelnr].setPointX(i, buffermodell.getPointX(i));
                        m[modelnr].setPointY(i, buffermodell.getPointY(i));
                        m[modelnr].setPointZ(i, buffermodell.getPointZ(i));
                    }

                    for (int i = 0; i < buffermodell.getVektoren(); i++)
                    {
                        m[modelnr].setVek1(i, buffermodell.getVek1(i));
                        m[modelnr].setVek2(i, buffermodell.getVek2(i));
                    }
                    canvas.Refresh();
                }
            
        }

        private void neu_Click(object sender, EventArgs e)
        {
            
                DialogResult result = MessageBox.Show("Modell/Frame wirklich zuruecksetzen?", "Achtung!",
                    MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    m[modelnr].setPunkte(1);
                    m[modelnr].setVektoren(1);


                    m[modelnr].setPointX(0, 0);
                    m[modelnr].setPointY(0, 0);
                    m[modelnr].setPointZ(0, 0);

                    m[modelnr].setVek1(0, 0);
                    m[modelnr].setVek2(0, 0);

                    canvas.Refresh();
                }
            
        }

        // Anim Start- und Endframe Einstellung
        private void startplus_Click(object sender, EventArgs e)
        {
            animstart++;
            if (animstart > animend)
            {
                animstart = animend;
            }
            range.Text = "Startframe: " + animstart + " Endframe: " + animend;
        }

        private void startminus_Click(object sender, EventArgs e)
        {
            animstart--;
            if (animstart < 0)
            {
                animstart = 0;
            }
            range.Text = "Startframe: " + animstart + " Endframe: " + animend;
        }

        private void endplus_Click(object sender, EventArgs e)
        {
            animend++;
            if (animend > 49)
            {
                animend = 49;
            }
            range.Text = "Startframe: " + animstart + " Endframe: " + animend;
        }

        private void endminus_Click(object sender, EventArgs e)
        {
            animend--;
            if (animend < animstart)
            {
                animend = animstart;
            }
            range.Text = "Startframe: " + animstart + " Endframe: " + animend;
        }

        // Animation in files speichern
        private void animsave_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In dieser Version leider nicht verfuegbar.");
            /*
            if (animend > animstart)
            {
                saveFileDialog2.Filter = "3d vector animation|*.3dva";
                saveFileDialog2.Title = "Animation speichern";
                saveFileDialog2.DefaultExt = "3dva";
                saveFileDialog2.ShowDialog();
            }
             */
        }

        private void saveFileDialog2_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*
            string name = saveFileDialog2.FileName;

            byte[] animmodel = new byte[2];

            animmodel[0] = (byte)animstart;
            animmodel[1] = (byte)animend;

            File.WriteAllBytes(name, animmodel);

            for (int j = 0; j < (animend - animstart); j++)
            {
                name = saveFileDialog2.FileName + j;

                byte[] encmodel = new byte[2 + m[modelnr].getPunkte() * 3 + m[modelnr].getVektoren() * 2];

                encmodel[0] = (byte)m[animstart+j].getPunkte();
                encmodel[1] = (byte)m[animstart + j].getVektoren();

                for (int i = 0; i < encmodel[0]; i++)
                {
                    encmodel[i + 2] = (byte)(m[animstart + j].getPointX(i) + 127);
                }

                for (int i = 0; i < encmodel[0]; i++)
                {
                    encmodel[i + 2 + encmodel[0]] = (byte)(m[animstart + j].getPointY(i) + 127);
                }

                for (int i = 0; i < encmodel[0]; i++)
                {
                    encmodel[i + 2 + (encmodel[0] * 2)] = (byte)(m[animstart + j].getPointZ(i) + 127);
                }

                for (int i = 0; i < encmodel[1]; i++)
                {
                    encmodel[i + 2 + (encmodel[0] * 3)] = (byte)m[animstart + j].getVek1(i);
                }

                for (int i = 0; i < encmodel[1]; i++)
                {
                    encmodel[i + 2 + (encmodel[0] * 3) + encmodel[1]]
                        = (byte)m[animstart + j].getVek2(i);
                }

                File.WriteAllBytes(name, encmodel);
            }

            MessageBox.Show("Animation gespeichert");
             */
        }

        // Animation aus file laden
        private void animload_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In dieser Version leider nicht verfuegbar.");
            /*
            openFileDialog2.Filter = "3d vector animation|*.3dva";
            openFileDialog2.Title = "Animation laden";
            openFileDialog2.DefaultExt = "3dva";

            DialogResult result = openFileDialog2.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog2.FileName;
                try
                {
                    byte[] animdata = File.ReadAllBytes(file);

                    animstart = animdata[0];
                    animend = animdata[1];

                    for (int j = 0; j < (animend - animstart); j++)
                    {
                        try
                        {
                            byte[] modeldata = File.ReadAllBytes(file + j);

                            m[animstart + j].setPunkte(modeldata[0]);
                            m[animstart + j].setVektoren(modeldata[1]);

                            for (int i = 0; i < modeldata[0]; i++)
                            {
                                m[animstart + j].setPointX(i, modeldata[i + 2] - 127);
                                m[animstart + j].setPointY(i, modeldata[i + 2 + modeldata[0]] - 127);
                                m[animstart + j].setPointZ(i, modeldata[i + 2 + (2 * modeldata[0])] - 127);
                            }

                            for (int i = 0; i < modeldata[1]; i++)
                            {
                                m[animstart + j].setVek1(i, modeldata[i + 2 + (3 * modeldata[0])]);
                            }

                            for (int i = 0; i < modeldata[1]; i++)
                            {
                                m[animstart + j].setVek2(i, modeldata[i + 2 + (3 * modeldata[0]) + modeldata[1]]);
                            }
                        }
                        catch (IOException)
                        {
                        }
                    }
                }
                catch (IOException)
                {
                }

            }
            canvas.Refresh();
             */
        }

        private void hilfeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string hilfetext = "3d Daumenkino Factory Frame Editor / Animator\n\n" +
                               "Mit diesem Programm kannst du...\n" +
                               "* 3D-Vektorgrafik aus Punkten und Vektoren konstruieren und edieren\n" +
                               "* Serien von Frames als Animation abspielen\n\n" +
                               "* 50 Animationsframes fuer bis zu 2,5 Sekunden Animation \n" +
                               "* Raster/Matrixsystem fuer einfache Positionierung\n" +
                               "* Pro Frame bis zu je 250 Punkte und Vektoren \n\n" +  
                               "* Frame wechseln: Frm +/Frm - \n" +
                               "  (rechts unten auf der Zeichenflaeche)\n\n" +
                               "* Einstellungen fuer Kamera, Perspektive und Skalierung: Oberes Panel links\n\n" +
                               "* Einstellungen fuer Animation: Oberes Panel rechts\n" +
                               "* Animation wird wiederolt von Startframe bis Endframe abgespielt\n\n" +
                               "* Editiermodus an/aus: Taste Edit (Unteres Panel links)\n" +
                               "* Steuerung des 3D-Cursor (ROTes Kreuz) im Editiermodus: Unteres Panel links\n" +
                               "* Einstellungen fuer den aktiven Punkt (PINKfarbenes Andreaskreuz)\n" +
                               "  im Editiermodus: Unteres Panel Mitte links\n" +
                               "* Einstellungen fuer den aktiven Vektor (PINKfarbener Vektor)\n" +
                               "  im Editiermodus: Unteres Panel Mitte rechts\n\n" +
                               "* Mit AddP und AddV Punkte und Vektoren hinzufuegen\n" +
                               "* Um Vektoren ziehen zu koennen, muss man erst Punkte setzen\n" +
                               "* Wenn der Cursor auf einen Punkt zeigt, den keinen Vektor mit dem\n" +
                               "  aktiven Punkt verbindet, wird der Vektor in HELLBLAU vorgeschlagen\n" +
                               "  und kann mit Button addV hinzugefuegt werden\n\n" +
                               "* Laden und Speichern von Frames: Unteres Panel rechts\n" +
                               "* Copy, Paste Frames: Unteres Panel rechts\n\n" +
                               "Viel Spass!";
                MessageBox.Show(hilfetext);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("3D-Daumenkino Factory.\nVersion 0.2\nCopyright Andreas Warta 2015\n" +
                            "Copying is permitted under GNU Public License.\n" +
                            "Vervielfaeltigung unter GNU Public License erlaubt.");
        }

        //Gitternetz ein/-ausschalten
        private void gridon_Click(object sender, EventArgs e)
        {
            if (edit)
            {
                edit = false;
            }
            else
            {
                edit = true;
            }
            canvas.Refresh();

        }

        
    }
}
    