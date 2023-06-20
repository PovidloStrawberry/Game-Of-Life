using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGameOfLife
{
    public partial class Form1 : Form
    {

        private Graphics graphics;
        private int resoluton;
        private int[,] field;
        private int[,] newField;
        private int[,] cellNeiboursCount;
        private int rows;
        private int cols;
        private Brush[] brushes = { Brushes.White, Brushes.Black };
        private List<Point> aliveCells = new List<Point>();
        private enum EBrushes
        {
            alive,
            dead
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            Stop();

        }

        private void CellDrawing()
        {
            graphics.Clear(Color.Black);
            foreach (Point aliveCell in aliveCells)
            {
                graphics.FillRectangle(Brushes.White, aliveCell.X * resoluton, aliveCell.Y * resoluton, resoluton, resoluton);
            }
            pictureBox1.Refresh();
        }

        private void Stop()
        {
            if (!timer1.Enabled) return;
            timer1.Stop();
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    field[i, j] = 0;

                }
            }
            aliveCells.Clear();
            CellDrawing();
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }


        /*
        private void CellDrawing()
        {
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    var hasLife = Convert.ToBoolean(field[i, j]);

                    graphics.FillRectangle(brushes[field[i, j]], i * resoluton, j * resoluton, resoluton, resoluton);
                    ///
                    if (hasLife) graphics.FillRectangle(Brushes.White, i * resoluton, j * resoluton, resoluton, resoluton);
                    else graphics.FillRectangle(Brushes.Black, i * resoluton, j * resoluton, resoluton, resoluton);
                    ///

                }
            }
            pictureBox1.Refresh();
        }
        */

        private void NextGenegation()
        {
            Array.Clear(cellNeiboursCount, 0, cols * rows);
            foreach (Point aliveCell in aliveCells)
            {
                SetCountOfNeighbours(aliveCell.X, aliveCell.Y);
            }
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    //var neighbourCount = CountNeighbours(ref i, ref j);
                    var hasLife = Convert.ToBoolean(field[i, j]);

                    if (!hasLife && cellNeiboursCount[i, j] == 3)
                    {
                        newField[i, j] = 1;
                        aliveCells.Add(new Point(i, j));
                    }
                    else if (hasLife && cellNeiboursCount[i, j] < 2 || cellNeiboursCount[i, j] > 3)
                    {
                        newField[i, j] = 0;
                        aliveCells.Remove(new Point(i, j));
                    }
                    else
                    {
                        newField[i, j] = field[i, j];
                    }
                    /*
                    if (i == cols - 1 || j == rows - 1)
                        newField[i, j] = 0;
                    */
                }
            }
            field = newField.Clone() as int[,];
            CellDrawing();
        }

        private int col;
        private int row;
        private bool isSelfChecking;
        private bool hasLife;
        private int count;

        private int CountNeighbours(ref int x, ref int y)
        {
            count = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    col = (x + i + cols) % cols;
                    row = (y + j + rows) % rows;
                    isSelfChecking = col == x && row == y;
                    hasLife = Convert.ToBoolean(field[col, row]);
                    if (hasLife && !isSelfChecking) count++;
                }
            }
            return count;
        }

        private int[,] SetCountOfNeighboursOperations =
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };

        private void SetCountOfNeighbours(int x, int y)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    col = (x + i + cols) % cols;
                    row = (y + j + rows) % rows;
                    cellNeiboursCount[col, row] += SetCountOfNeighboursOperations[i + 1, j + 1];
                    /*
                    isSelfChecking = col == x && row == y;
                    if (!isSelfChecking) cellNeiboursCount[col, row]++;
                    */
                }
            }
        }



        private void StartGame()
        {
            //if (timer1.Enabled) return;
            aliveCells.Clear();
            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            resoluton = (int)nudResolution.Value;
            rows = pictureBox1.Height / resoluton;
            cols = pictureBox1.Width / resoluton;
            field = new int[cols, rows];
            newField = new int[cols, rows]; 
            cellNeiboursCount = new int[cols, rows];

            Random random = new Random();
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    field[i, j] = Convert.ToInt32(random.Next((int)nudDensity.Value) == 0);
                    if(field[i, j] == 1)
                    {
                        aliveCells.Add(new Point(i, j));
                    }
                }
            }
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGenegation();
        }

        private void breload_Click(object sender, EventArgs e)
        {
            if (!timer1.Enabled) return;
            Stop();
            StartGame();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var pos1 = PictureBox.MousePosition.X / resoluton - 287 / resoluton;
            var pos2 = PictureBox.MousePosition.Y / resoluton - 30 / resoluton;
            if (field[(pos1 + cols) % cols, (pos2 + rows) % rows] == 0)
            {
                field[(pos1 + cols) % cols, (pos2 + rows) % rows] = 1;
                aliveCells.Add(new Point((pos1 + cols) % cols, (pos2 + rows) % rows));
            }
            else
            {
                field[(pos1 + cols) % cols, (pos2 + rows) % rows] = 0;
                aliveCells.Remove(new Point((pos1 + cols) % cols, (pos2 + rows) % rows));
            }

            //field[(pos1 + cols) % cols, (pos2 + rows - 1) % rows] = 1;
            //field[(pos1 + cols + 1) % cols, (pos2 + rows) % rows] = 1;
            //field[(pos1 + cols) % cols, (pos2 + rows + 1) % rows] = 1;
            //field[(pos1 + cols - 1) % cols, (pos2 + rows + 1) % rows] = 1;
            //field[(pos1 + cols + 1) % cols, (pos2 + rows + 1) % rows] = 1;

            //if (field[pos1, pos2] == 0) field[pos1, pos2] = 1;
            //else field[pos1, pos2] = 0;
            CellDrawing();
        }

        //bStart.BackgroundImage = Image.FromFile(@"D:\2.jpg", false);
        //bStart.BackgroundImageLayout = ImageLayout.Center;

        //для заполнения пикселя
        //field[(pos1 + cols) % cols, (pos2 + rows) % rows] = 1;

        private void bPause_Click(object sender, EventArgs e)
        {
            // if (!timer1.Enabled) return;

            if (timer1.Enabled) timer1.Stop();
            else timer1.Start();
        }


        private void bNextFrame_Click(object sender, EventArgs e)
        {
            NextGenegation();
            CellDrawing();
        }
    }
}
