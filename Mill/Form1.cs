using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mill
{
    public partial class Mill : Form
    {
        private int[,] map;
        private List<((int, int), (int, int), (int, int))> lines;
        private List<((int, int), (int, int))> neighboring_cells;
        private int size;
        private int cell_size;
        private int score1, score2;
        private int placed_chips1, placed_chips2;
        private bool first_player;
        private bool should_eat;
        private bool jump;
        private Point source_jump;
        public Mill()
        {
            InitializeComponent();
            label1.Parent = pictureBox1;
            label2.Parent = pictureBox1;
            label3.Parent = pictureBox1;
            Init();
        }
        private void Init()
        {
            size = 7;
            score1 = 9;
            score2 = 9;
            placed_chips1 = 0;
            placed_chips2 = 0;
            cell_size = Width / size - 3;
            first_player = true;
            jump = false;
            should_eat = false;
            InitNeighbours();
            InitLines();            
            InitMap();
            pictureBox1.Invalidate();
        }
        private void InitNeighbours()
        {
            neighboring_cells = new List<((int, int), (int, int))>();
            neighboring_cells.Add(((0, 0), (0, 3)));
            neighboring_cells.Add(((0, 0), (3, 0)));

            neighboring_cells.Add(((0, 3), (1, 3)));
            neighboring_cells.Add(((0, 3), (0, 6)));

            neighboring_cells.Add(((0, 6), (3, 6)));

            neighboring_cells.Add(((1, 1), (1, 3)));
            neighboring_cells.Add(((1, 1), (3, 1)));

            neighboring_cells.Add(((1, 3), (1, 5)));
            neighboring_cells.Add(((1, 3), (2, 3)));

            neighboring_cells.Add(((1, 5), (3, 5)));

            neighboring_cells.Add(((2, 2), (2, 3)));
            neighboring_cells.Add(((2, 2), (3, 2)));

            neighboring_cells.Add(((2, 3), (2, 4)));

            neighboring_cells.Add(((2, 4), (3, 4)));

            neighboring_cells.Add(((3, 0), (3, 1)));
            neighboring_cells.Add(((3, 0), (6, 0)));

            neighboring_cells.Add(((3, 1), (5, 1)));
            neighboring_cells.Add(((3, 1), (3, 2)));

            neighboring_cells.Add(((3, 2), (4, 2)));

            neighboring_cells.Add(((3, 4), (4, 4)));
            neighboring_cells.Add(((3, 4), (3, 5)));

            neighboring_cells.Add(((3, 5), (5, 5)));
            neighboring_cells.Add(((3, 5), (3, 6)));

            neighboring_cells.Add(((3, 6), (6, 6)));

            neighboring_cells.Add(((4, 2), (4, 3)));

            neighboring_cells.Add(((4, 3), (4, 4)));
            neighboring_cells.Add(((4, 3), (5, 3)));

            neighboring_cells.Add(((1, 3), (1, 5)));
            neighboring_cells.Add(((1, 3), (2, 3)));

            neighboring_cells.Add(((5, 1), (5, 3)));

            neighboring_cells.Add(((5, 3), (5, 5)));
            neighboring_cells.Add(((5, 3), (6, 3)));

            neighboring_cells.Add(((6, 0), (6, 3)));

            neighboring_cells.Add(((6, 3), (6, 6)));
        }
        private void InitLines()
        {
            lines = new List<((int, int), (int, int), (int, int))>
            {
                ((0, 0), (0, 3), (0, 6)),
                ((1, 1), (1, 3), (1, 5)),
                ((2, 2), (2, 3), (2, 4)),
                ((3, 0), (3, 1), (3, 2)),
                ((3, 4), (3, 5), (3, 6)),
                ((4, 2), (4, 3), (4, 4)),
                ((5, 1), (5, 3), (5, 5)),
                ((6, 0), (6, 3), (6, 6)),
                ((0, 0), (3, 0), (6, 0)),
                ((1, 1), (3, 1), (5, 1)),
                ((2, 2), (3, 2), (4, 2)),
                ((0, 3), (1, 3), (2, 3)),
                ((4, 3), (5, 3), (6, 3)),
                ((2, 4), (3, 4), (4, 4)),
                ((1, 5), (3, 5), (5, 5)),
                ((0, 6), (3, 6), (6, 6))
            };
        }
        private void InitMap()
        {
            map =  new int[,]{
                { 0, -1, -1, 0, -1, -1, 0},
                { -1, 0, -1, 0, -1, 0, -1},
                { -1, -1, 0, 0, 0, -1, -1},
                { 0, 0, 0, -1, 0, 0, 0},
                { -1, -1, 0, 0, 0, -1, -1},
                { -1, 0, -1, 0, -1, 0, -1},
                { 0, -1, -1, 0, -1, -1, 0}
            };
        }
        private void DrawMap(Graphics g)
        {
            Image wood = new Bitmap(Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).FullName.ToString(), @"Sprites\wood.jpg"));
            g.DrawImage(wood,
                new Rectangle(0, 0, Width, Height),
                new Rectangle(0, 0, wood.Width, wood.Height),
                GraphicsUnit.Pixel);

            g.DrawRectangle(new Pen(Color.FromArgb(23, 22, 21), 5),
                cell_size / 2, cell_size / 2, 6 * cell_size, 6 * cell_size);
            g.DrawRectangle(new Pen(Color.FromArgb(23, 22, 21), 5),
                cell_size + cell_size / 2, cell_size + cell_size / 2, 4 * cell_size, 4 * cell_size);
            g.DrawRectangle(new Pen(Color.FromArgb(23, 22, 21), 5),
                2 * cell_size + cell_size / 2, 2 * cell_size + cell_size / 2, 2 * cell_size, 2 * cell_size);

            g.DrawLine(new Pen(Color.FromArgb(23, 22, 21), 5),
                3 * cell_size + cell_size / 2, cell_size / 2, 3 * cell_size + cell_size / 2, 3 * cell_size - cell_size / 2);
            g.DrawLine(new Pen(Color.FromArgb(23, 22, 21), 5),
                3 * cell_size + cell_size / 2, 4 * cell_size + cell_size / 2, 3 * cell_size + cell_size / 2, 6 * cell_size + cell_size / 2);

            g.DrawLine(new Pen(Color.FromArgb(23, 22, 21), 5),
                cell_size / 2, 3 * cell_size + cell_size / 2, 3 * cell_size - cell_size / 2, 3 * cell_size + cell_size / 2);
            g.DrawLine(new Pen(Color.FromArgb(23, 22, 21), 5),
                4 * cell_size + cell_size / 2, 3 * cell_size + cell_size / 2, 6 * cell_size + cell_size / 2, 3 * cell_size + cell_size / 2);

            for (int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    if(map[j, i] == 0)
                    {
                        g.FillEllipse(new SolidBrush(Color.FromArgb(102, 96, 93)),
                            i * cell_size + cell_size / 4,
                            j * cell_size + cell_size / 4,
                            cell_size / 2, cell_size / 2);
                        g.FillEllipse(new SolidBrush(Color.FromArgb(23, 22, 21)),
                            i * cell_size + cell_size / 4 + cell_size / 8,
                            j * cell_size + cell_size / 4 + cell_size / 8,
                            cell_size / 4, cell_size / 4);
                    }
                    else if(map[j, i] == 1)
                    {
                        g.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 255)),
                            i * cell_size + cell_size / 4,
                            j * cell_size + cell_size / 4,
                            cell_size / 2, cell_size / 2);
                    }
                    else if(map[j, i] == 2)
                    {
                        g.FillEllipse(new SolidBrush(Color.FromArgb(0, 0, 0)),
                            i * cell_size + cell_size / 4,
                            j * cell_size + cell_size / 4,
                            cell_size / 2, cell_size / 2);
                    }
                }
            }
        }
        private void WriteScore()
        {
            label1.Text = score1.ToString();
            label2.Text = score2.ToString();
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {

                int cell_x = e.Location.X / cell_size;
                int cell_y = e.Location.Y / cell_size;
                if (first_player)
                {
                    if (should_eat)
                    {
                        if(map[cell_y, cell_x] == 2)
                        {
                            map[cell_y, cell_x] = 0;
                            score2--;
                            if(score2 <= 2)
                            {
                                pictureBox1.Invalidate();
                                if (MessageBox.Show("Белые победили.\nНачать заново?", "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    Init();
                                }
                                else
                                {
                                    Application.Exit();
                                }
                            }
                            should_eat = false;
                            first_player = false;
                        }
                    }
                    else if(placed_chips1 < 9)
                    {
                        if(map[cell_y, cell_x] == 0)
                        {
                            map[cell_y, cell_x] = 1;
                            placed_chips1++;
                            should_eat = IsMakeMill(cell_x, cell_y);
                            if (!should_eat)
                            {
                                first_player = false;
                            }
                        }
                    }
                    else
                    {
                        if (!jump)
                        {
                            if (map[cell_y, cell_x] == 1)
                            {
                                source_jump.X = cell_x;
                                source_jump.Y = cell_y;
                                jump = true;
                            }
                        }
                        else
                        {
                            if (map[cell_y, cell_x] == 1)
                            {
                                source_jump.X = cell_x;
                                source_jump.Y = cell_y;
                            }
                            else if (map[cell_y, cell_x] == 0 && IsInNeighbours(cell_x, cell_y))
                            {
                                map[source_jump.Y, source_jump.X] = 0;
                                map[cell_y, cell_x] = 1;
                                jump = false;
                                should_eat = IsMakeMill(cell_x, cell_y);
                                if (!should_eat)
                                {
                                    first_player = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (should_eat)
                    {
                        if (map[cell_y, cell_x] == 1)
                        {
                            map[cell_y, cell_x] = 0;
                            score1--;
                            if (score1 <= 2)
                            {
                                pictureBox1.Invalidate();
                                if (MessageBox.Show("Черные победили.\nНачать заново?", "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    Init();
                                }
                                else
                                {
                                    Application.Exit();
                                }
                            }
                            should_eat = false;
                            first_player = true;
                        }
                    }
                    else if (placed_chips2 < 9)
                    {
                        if (map[cell_y, cell_x] == 0)
                        {
                            map[cell_y, cell_x] = 2;
                            should_eat = IsMakeMill(cell_x, cell_y);
                            placed_chips2++;
                            if (!should_eat)
                            {
                                first_player = true;
                            }
                        }
                    }
                    else
                    {
                        if (!jump)
                        {
                            if (map[cell_y, cell_x] == 2)
                            {
                                source_jump.X = cell_x;
                                source_jump.Y = cell_y;
                                jump = true;
                            }
                        }
                        else
                        {
                            if(map[cell_y, cell_x] == 2)
                            {
                                source_jump.X = cell_x;
                                source_jump.Y = cell_y;
                            }
                            else if(map[cell_y, cell_x] == 0 && IsInNeighbours(cell_x, cell_y))
                            {
                                map[source_jump.Y, source_jump.X] = 0;
                                map[cell_y, cell_x] = 2;
                                jump = false;
                                should_eat = IsMakeMill(cell_x, cell_y);
                                if (!should_eat)
                                {
                                    first_player = true;
                                }
                            }
                        }
                    }
                }
                pictureBox1.Invalidate();
            }
        }
        private bool IsMakeMill(int cell_x, int cell_y)
        {
            int player = map[cell_y, cell_x];
            foreach(((int, int) first, (int, int) second, (int, int) third) in lines)
            {
                if(first.Item1 == cell_y && first.Item2 == cell_x || second.Item1 == cell_y && second.Item2 == cell_x || third.Item1 == cell_y && third.Item2 == cell_x)
                {
                    if(map[first.Item1, first.Item2] == player && map[second.Item1, second.Item2] == player && map[third.Item1, third.Item2] == player)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool IsInNeighbours(int cell_x, int cell_y)
        {
            foreach(((int, int) first, (int, int) second) in neighboring_cells)
            {
                if(first.Item1 == source_jump.Y && first.Item2 == source_jump.X && second.Item1 == cell_y && second.Item2 == cell_x)
                {
                    return true;
                }
                else if(second.Item1 == source_jump.Y && second.Item2 == source_jump.X && first.Item1 == cell_y && first.Item2 == cell_x)
                {
                    return true;
                }
            }
            return false;
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawMap(e.Graphics);
            WriteScore();
            if (jump)
            {
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 0)),
                            source_jump.X * cell_size + cell_size / 4 - cell_size / 14,
                            source_jump.Y * cell_size + cell_size / 4 - cell_size / 14,
                            cell_size / 2 + cell_size / 7, cell_size / 2 + cell_size / 7);
                if (map[source_jump.Y, source_jump.X] == 1)
                {
                    e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 255)),
                        source_jump.X * cell_size + cell_size / 4,
                        source_jump.Y * cell_size + cell_size / 4,
                        cell_size / 2, cell_size / 2);
                }
                else if (map[source_jump.Y, source_jump.X] == 2)
                {
                    e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 0, 0)),
                        source_jump.X * cell_size + cell_size / 4,
                        source_jump.Y * cell_size + cell_size / 4,
                        cell_size / 2, cell_size / 2);
                }
            }
            if (first_player)
            {
                label3.Text = "Ход белых";
            }
            else
            {
                label3.Text = "Ход черных";
            }
        }
    }
}
