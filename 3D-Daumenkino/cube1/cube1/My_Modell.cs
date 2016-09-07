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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cube1
{
    public class My_Modell
    {
        int nr_punkte;
        int nr_vektoren;

        int[] cX = new int[250];
        int[] cY = new int[250];
        int[] cZ = new int[250];

        int[,] vektor = new int[2, 250];
        
        public My_Modell(int model)
        {
            
            if (model == 1)
            {
                int[] x =
                {
                    -10, 10, -10, 10, -10, 10, -10, 10,
                    0, -4, -8, -8, -4, 0, 0, 0, 4, 4, -4, 8, -4, 8,
                    10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
                    10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10
                };
                int[] y =
                {
                    -10, -10, 10, 10, -10, -10, 10, 10,
                    -8, -8, -4, 4, 8, 8, -6, 6, -6, 6, -2, -2, 2, 2,
                    4, 2, 2, 4, 6, 8, 8, 6, 6, 8,
                    8, 2, 4, 6, 8, 8, 6, 4, 2, 2, 4
                };
                int[] z =
                {
                    10, 10, 10, 10, -10, -10, -10, -10,
                    -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10,
                    -2, -4, -6, -8, -8, -6, -4, -2, -6, -2,
                    0, 0, 2, 0, 4, 6, 8, 6, 6, 4, 4
                };

                nr_punkte = x.Length;

                int[] p1 =
                {
                    0, 0, 2, 1, 4, 4, 6, 5, 0, 1,
                    2, 3, 8, 9, 10, 11, 12, 14, 16, 18,
                    20, 22, 23, 24, 25, 26, 27, 28, 29, 29,
                    32, 33, 34, 36, 37, 38, 39, 40, 41, 42
                };

                int[] p2 =
                {
                    1, 2, 3, 3, 5, 6, 7, 7, 4, 5,
                    6, 7, 9, 10, 11, 12, 13, 15, 17, 19,
                    21, 23, 24, 25, 26, 27, 28, 29, 30, 31,
                    33, 34, 35, 37, 38, 39, 40, 41, 36, 39
                };

                nr_vektoren = p1.Length;

                for (int i = 0; i < nr_punkte; i++)
                {
                    cX[i] = x[i];
                    cY[i] = y[i];
                    cZ[i] = z[i];
                }

                for (int i = 0; i < nr_vektoren; i++)
                {
                    vektor[0, i] = p1[i];
                    vektor[1, i] = p2[i];
                }
            }

            if (model == 2)
            {
                int[] x =
                {
                    10,6,6,6,6,2,2,2,2,-2,-2,-2,-2,
                    -6,-6,-6,-6,-7,-7,-7,-7,-10,-10,-10,-10
                };
                int[] y =
                {
                    0,-3,0,3,0,-4,0,4,0,-3,0,3,0,
                    -2,0,2,0,-5,0,5,0,-3,0,3,0
                };
                int[] z =
                {
                    0,0,-3,0,3,0,-4,0,4,0,-3,0,3,
                    0,-2,0,2,0,-5,0,5,0,-3,0,3
                };

                nr_punkte = x.Length;

                int[] p1 =
                {
                    0,0,0,0,1,2,3,4,1,2,3,4,5,6,7,8,5,6,7,8,
                    9,10,11,12,9,10,11,12,13,14,15,16,13,14,15,16,
                    21,22,23,24,17,18,19,20
                };

                int[] p2 =
                {
                    1,2,3,4,2,3,4,1,5,6,7,8,6,7,8,5,9,10,11,12,
                    10,11,12,9,13,14,15,16,14,15,16,13,21,22,23,24,
                    17,18,19,20,9,10,11,12
                };

                nr_vektoren = p1.Length;

                for (int i = 0; i < nr_punkte; i++)
                {
                    cX[i] = x[i];
                    cY[i] = y[i];
                    cZ[i] = z[i];
                }

                for (int i = 0; i < nr_vektoren; i++)
                {
                    vektor[0, i] = p1[i];
                    vektor[1, i] = p2[i];
                }
            }

            // Grid Modell
            if (model == 99)
            {

                nr_punkte = 216;

                nr_vektoren = 0;

                int counter = 0;
                for (int i = -10 ; i <= 10; i += 4)
                {
                    for (int j = -10; j <= 10; j += 4)
                    {
                        for (int k = -10; k <= 10; k += 4)
                        {
                            cX[counter] = i;
                            cY[counter] = j;
                            cZ[counter] = k;
                            counter++;
                        }
                    }
                    
                }
                
            }
            if (model == 0)
            {
                int[] x =
                {
                    0
                };
                int[] y =
                {
                    0
                };
                int[] z =
                {
                    0
                };

                nr_punkte = 1;

                int[] p1 =
                {
                    0
                };

                int[] p2 =
                {
                    0
                };

                nr_vektoren = 1;

                for (int i = 0; i < nr_punkte; i++)
                {
                    cX[i] = x[i];
                    cY[i] = y[i];
                    cZ[i] = z[i];
                }

                for (int i = 0; i < nr_vektoren; i++)
                {
                    vektor[0, i] = p1[i];
                    vektor[1, i] = p2[i];
                }
            }

        }

        public int getPointX(int pnr)
        {
            return cX[pnr];
        }
        public void setPointX(int pnr, int value)
        {
            cX[pnr] = value;
        }

        public int getPointY(int pnr)
        {
            return cY[pnr];
        }
        public void setPointY(int pnr, int value)
        {
            cY[pnr] = value;
        }

        public int getPointZ(int pnr)
        {
            return cZ[pnr];
        }
        public void setPointZ(int pnr, int value)
        {
            cZ[pnr] = value;
        }

        public int getVek1(int vnr)
        {
            return vektor[0, vnr];
        }
        public void setVek1(int vnr, int value)
        {
            vektor[0, vnr] = value;
        }

        public int getVek2(int vnr)
        {
            return vektor[1, vnr];
        }
        public void setVek2(int vnr, int value)
        {
            vektor[1, vnr] = value;
        }

        public int getPunkte()
        {
            return nr_punkte;
        }
        public void setPunkte(int value)
        {
            nr_punkte = value;
        }

        public int getVektoren()
        {
            return nr_vektoren;
        }
        public void setVektoren(int value)
        {
            nr_vektoren = value;
        }

    }

}

