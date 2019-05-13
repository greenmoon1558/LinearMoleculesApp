//#define FIG1
#define FIG34

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Drawing2D;

namespace LinearMoleculesApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int n = 1;
        string q = Environment.NewLine;
        // The height of a hexagon.
        private const float HexHeight = 50;

        // Selected hexagons.
        private List<PointF> Hexagons = new List<PointF>();
        int hexagonsIndex = 1;
        private List<(PointF, PointF)> hexNumbers = new List<(PointF, PointF)>();
        private List<(PointF, PointF)> connections = new List<(PointF, PointF)>();
        private List<(PointF, int)> pointToIndex = new List<(PointF, int)>();

#if FIG34
        // The selected search rectangle.
        // Used to draw Figures 3 and 4.
        private List<RectangleF> TestRects = new List<RectangleF>();
#endif
        private void setMatrixUsigHex(List<(int, int)> intsConnections)
        {
            if (n != 0)
            {
                textBox1.Text += q + "Задається випадковими числами матриця..." + "\r\n";
                progressBar1.Show();
                progressBar1.Maximum = n * n;
                progressBar1.Value = 0;
                Random x = new Random();
                for (short i = 0; i < n; i++)
                  // var rows = intsConnections.FindAll(conns => (conns.Item1 == i+1) || (conns.Item2 == i+1));

                    for (short j = (short)(i + 1); j < n; j++)
                    {
                        progressBar1.Value++;
                        if (intsConnections.FindAll(conns =>
                        ((conns.Item1 == (i)) && (conns.Item2 == (j))) ||
                        ((conns.Item2 == (i)) && (conns.Item1 == (j)))).Count != 0)
                        {
                            dataGridView1[i, j].Value = 1;
                        }
                        //x.Next(0, 20);
                        dataGridView1[j, i].Value = dataGridView1[i, j].Value;
                        textBox1.Text += "Додано ребро: (" + i.ToString() + "; " + j.ToString() + ") вагою " + dataGridView1[i, j].Value + "\r\n";
                    }

                progressBar1.Value = progressBar1.Maximum;
                progressBar1.Value = progressBar1.Minimum;
                progressBar1.Hide();
            }
            else
            {
                textBox1.Text += q + "Збільшіть розмір матриці" + "\r\n";
            }

        }
        // Redraw the grid.
        private void picGrid_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            // Draw the grid.
            DrawHexGrid(e.Graphics, Pens.LightGray,
                0, picGrid.ClientSize.Width,
                0, picGrid.ClientSize.Height,
                HexHeight);
            // Draw the selected hexagons.
            hexagonsIndex = 1;
            hexNumbers = new List<(PointF, PointF)>();
            connections = new List<(PointF, PointF)>();
            pointToIndex = new List<(PointF, int)>();
            foreach (PointF point in Hexagons)
            {
                PointF[] points = HexToPoints(HexHeight, point.X, point.Y);
                e.Graphics.FillPolygon(Brushes.LightBlue, points);
                e.Graphics.DrawPolygon(Pens.Black, points);
                createConnection(points);
                foreach ( (PointF realP, PointF drawingP) in HexToPointsForNumbers(HexHeight, point.X, point.Y))
                {
                    hexNumbers.Add((realP, drawingP));
                    DrawStringFloatFormat(e, hexagonsIndex.ToString(), drawingP.X, drawingP.Y);
                    pointToIndex.Add((realP, hexagonsIndex));
                    hexagonsIndex++;
                }
            }
            List<(int, int)> intsConnections = new List<(int, int)>();
           // pointToIndex;
            foreach ((PointF point1, PointF point2) in connections)
            {
                
                var index1 = pointToIndex.Find(currPoint => ((currPoint.Item1.X == point1.X) && (currPoint.Item1.Y == point1.Y))).Item2;
                var index2 = pointToIndex.Find(currPoint => ((currPoint.Item1.X == point2.X) && (currPoint.Item1.Y == point2.Y))).Item2;
                intsConnections.Add((index1, index2));
            }
            numericUpDown1.Value = hexagonsIndex;
            numericUpDown1_ValueChanged(sender, e);
            setMatrixUsigHex(intsConnections);
        }

        public void DrawStringFloatFormat(PaintEventArgs e,string drawString, float x, float y )
        {
            // Create font and brush.
            Font drawFont = new Font("Arial", 9);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            // Set format of string.
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = StringFormatFlags.DisplayFormatControl;

            // Draw string to screen.
            e.Graphics.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);
        }
        // Draw a hexagonal grid for the indicated area.
        // (You might be able to draw the hexagons without
        // drawing any duplicate edges, but this is a lot easier.)
        private void createConnection(PointF[] points)
        {
            for (int i = 0; i < points.Count(); i++)
            {
                int next = 1 + i;
                if (next == points.Count()) next = 0;
                connections.Add((points[i], points[next]));
            }
        }
        private void DrawHexGrid(Graphics gr, Pen pen,
            float xmin, float xmax, float ymin, float ymax,
            float height)
        {
            // Loop until a hexagon won't fit.
            for (int row = 0; ; row++)
            {
                // Get the points for the row's first hexagon.
                PointF[] points = HexToPoints(height, row, 0);
                
                // If it doesn't fit, we're done.
                if (points[4].Y > ymax) break;

                // Draw the row.
                for (int col = 0; ; col++)
                {
                    // Get the points for the row's next hexagon.
                    points = HexToPoints(height, row, col);

                    // If it doesn't fit horizontally,
                    // we're done with this row.
                    if (points[3].X > xmax) break;

                    // If it fits vertically, draw it.
                    if (points[4].Y <= ymax)
                    {
                        gr.DrawPolygon(pen, points);

#if FIG1
                        // Label the hexagon (for Figure 1).
                        using (StringFormat sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;
                            float x = (points[0].X + points[3].X) / 2;
                            float y = (points[1].Y + points[4].Y) / 2;
                            string label = "(" + row.ToString() + ", " +
                                col.ToString() + ")";
                            gr.DrawString(label, this.Font,
                                Brushes.Black, x, y, sf);
                        }
#endif
                    }
                }
            }
        }

        private void picGrid_Resize(object sender, EventArgs e)
        {
            picGrid.Refresh();
        }

        // Display the row and column under the mouse.
        private void picGrid_MouseMove(object sender, MouseEventArgs e)
        {
            int row, col;
            PointToHex(e.X, e.Y, HexHeight, out row, out col);
            this.Text = "(" + row + ", " + col + ")";
        }
        private bool isHexExist(float row, float col)
        {
            foreach(PointF point in Hexagons)
            {
                if (point.X == row && point.Y == col)
                    return true;
            }
            return false;
        }
        // Add the clicked hexagon to the Hexagons list.
        private void picGrid_MouseClick(object sender, MouseEventArgs e)
        {
            int row, col;
            PointToHex(e.X, e.Y, HexHeight, out row, out col);
            if (!isHexExist(row, col))
                Hexagons.Add(new PointF(row, col));
            else
                Hexagons.Remove(new PointF(row, col));

#if FIG34
            // Used to draw Figures 3 and 4.
            PointF[] points = HexToPoints(HexHeight, row, col);
            TestRects.Add(new RectangleF(
                points[0].X, points[1].Y,
                0.75f * (points[3].X - points[0].X),
                points[4].Y - points[1].Y));
         
#endif

            picGrid.Refresh();
        }

        // Return the width of a hexagon.
        private float HexWidth(float height)
        {
            return (float)(4 * (height / 2 / Math.Sqrt(3)));
        }

        // Return the row and column of the hexagon at this point.
        private void PointToHex(float x, float y, float height,
            out int row, out int col)
        {
            // Find the test rectangle containing the point.
            float width = HexWidth(height);
            col = (int)(x / (width * 0.75f));

            if (col % 2 == 0)
                row = (int)(y / height);
            else
                row = (int)((y - height / 2) / height);

            // Find the test area.
            float testx = col * width * 0.75f;
            float testy = row * height;
            if (col % 2 == 1) testy += height / 2;

            // See if the point is above or
            // below the test hexagon on the left.
            bool is_above = false, is_below = false;
            float dx = x - testx;
            if (dx < width / 4)
            {
                float dy = y - (testy + height / 2);
                if (dx < 0.001)
                {
                    // The point is on the left edge of the test rectangle.
                    if (dy < 0) is_above = true;
                    if (dy > 0) is_below = true;
                }
                else if (dy < 0)
                {
                    // See if the point is above the test hexagon.
                    if (-dy / dx > Math.Sqrt(3)) is_above = true;
                }
                else
                {
                    // See if the point is below the test hexagon.
                    if (dy / dx > Math.Sqrt(3)) is_below = true;
                }
            }

            // Adjust the row and column if necessary.
            if (is_above)
            {
                if (col % 2 == 0) row--;
                col--;
            }
            else if (is_below)
            {
                if (col % 2 == 1) row++;
                col--;
            }
        }
        private float[] getXYHeight(float height, float row, float col)
        {
            // Start with the leftmost corner of the upper left hexagon.
            float width = HexWidth(height);
            float y = height / 2;
            float x = 0;

            // Move down the required number of rows.
            y += row * height;

            // If the column is odd, move down half a hex more.
            if (col % 2 == 1) y += height / 2;

            // Move over for the column number.
            x += col * (width * 0.75f);
            return new float[] { x, y, width, height };
        }
        // Return the points that define the indicated hexagon.
        private PointF[] HexToPoints(float height, float row, float col)
        {
            float[] arrXYWH = getXYHeight(height, row, col);
            float x = arrXYWH[0], y = arrXYWH[1], width = arrXYWH[2];
            height = arrXYWH[3];

            // Generate the points.
            return new PointF[]
                {
                    new PointF(x, y),
                    new PointF(x + width * 0.25f, y - height / 2),
                    new PointF(x + width * 0.75f, y - height / 2),
                    new PointF(x + width, y),
                    new PointF(x + width * 0.75f, y + height / 2),
                    new PointF(x + width * 0.25f, y + height / 2),
                };
        }
      
        private List<(PointF, PointF)> HexToPointsForNumbers(float height, float row, float col)
        {
            float[] arrXYWH = getXYHeight(height, row, col);
            float x = arrXYWH[0], y = arrXYWH[1], width = arrXYWH[2];
            height = arrXYWH[3];
            // Generate the points.
            PointF[] realPoints = new PointF[]
                {
                    new PointF(x, y),
                    new PointF(x + width * 0.25f, y - height / 2 ),
                    new PointF(x + width * 0.75f, y - height / 2 ),
                    new PointF(x + width, y),
                    new PointF(x + width * 0.75f, y + height / 2),
                    new PointF(x + width * 0.25f, y + height / 2),
                };
            PointF[] drawingPoints = new PointF[]
               {
                    new PointF(x + 2, y - 7),
                    new PointF(x - 2 + width * 0.25f, y - height / 2 ),
                    new PointF(x - 12 + width * 0.75f, y - height / 2 ),
                    new PointF(x + width - 18, y - 7),
                    new PointF(x - 12 + width * 0.75f, y - 14 + height / 2),
                    new PointF(x - 2 + width * 0.25f, y - 14 + height / 2),
               };
            List<(PointF, PointF)> resultPoints = new List<(PointF, PointF)>();
            for (int i = 0; i < realPoints.Count(); i++) {
                if (hexNumbers.FindAll(currPoint => ((currPoint.Item1.X == realPoints[i].X)
                && (currPoint.Item1.Y == realPoints[i].Y))).Count == 0)
                {
                    resultPoints.Add((realPoints[i], drawingPoints[i]));
                }
            }
            return resultPoints;


            }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            n = (byte)numericUpDown1.Value;
            dataGridView1.ColumnCount = n;
            dataGridView1.RowCount = n;
            for (short i = 0; i < n; i++)
            {
                dataGridView1[i, i].Style.BackColor = Color.Gray;
                dataGridView1[i, i].Value = 0;
                dataGridView1[i, i].ReadOnly = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDown1_ValueChanged(sender, e);
            progressBar1.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == e.RowIndex) e.Cancel = true;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            bool flag1 = true;
            try
            {
                Convert.ToDouble(dataGridView1[e.ColumnIndex, e.RowIndex].Value);
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Black;
            }
            catch
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Red;
                flag1 = false;
            }
            if (flag1) dataGridView1[e.RowIndex, e.ColumnIndex].Value = dataGridView1[e.ColumnIndex, e.RowIndex].Value;
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (n!=0) {
                textBox1.Text += q + "Задається випадковими числами матриця..." + "\r\n";
                progressBar1.Show();
                progressBar1.Maximum = n * n;
                progressBar1.Value = 0;
                Random x = new Random();
                for (short i = 0; i < n; i++)
                    for (short j = (short)(i + 1); j < n; j++)
                    {
                        progressBar1.Value++;
                        dataGridView1[i, j].Value = x.Next(0, 20);
                        dataGridView1[j, i].Value = dataGridView1[i, j].Value;
                        textBox1.Text += "Додано ребро: (" + i.ToString() + "; " + j.ToString() + ") вагою " + dataGridView1[i, j].Value + "\r\n";
                    }

                progressBar1.Value = progressBar1.Maximum;
                progressBar1.Value = progressBar1.Minimum;
                progressBar1.Hide();
            } else
            {
                textBox1.Text += q + "Збільшіть розмір матриці" + "\r\n";
            }
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
           double[,] matrixArray = new double[dataGridView1.RowCount, dataGridView1.ColumnCount+2];
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                   
                    if (dataGridView1.Rows[i].Cells[j].Value != null)
                        matrixArray[i, j] = double.Parse(dataGridView1.Rows[i].Cells[j].Value.ToString());
                    else matrixArray[i, j] = 0;
                }
                matrixArray[i, dataGridView1.ColumnCount] = 1;
                matrixArray[i, dataGridView1.ColumnCount + 1] = 0;
            }
            try
            {
                double[] results = LinearEquationSolver.Solve(matrixArray);
                for(int i = 0; i<results.Length; i++)
                {
                    textBox1.Text += "x"+i.ToString()+"="+results[i] + " \r\n";
                }
            }
            catch (Exception ex)
            {
                textBox1.Text += ex.Message + " \r\n";
            }

        }
    }
}
