using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Emit;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace ArtCalculator
{
    public partial class Form1 : Form
    {
        private Rectangle bounds;
        private Bitmap screenshot;

        private Point firstClick = Point.Empty; // Первый клик
        private Point secondClick = Point.Empty; // Второй клик

        private double gridDist;

        private double targetDistance;

        private void Form1_Load(object sender, EventArgs e){}
        private void label2_Click(object sender, EventArgs e){}
        private void label6_Click(object sender, EventArgs e) { }
        private void label10_Click(object sender, EventArgs e) { }

        public Form1()
        {
            InitializeComponent();
            UpdateViewPointStatus();
            label1.BackColor = Color.Red;
        }

        private void button1_Click(object sender, EventArgs e) // Make Screen Shot
        {
            bounds = Screen.PrimaryScreen.Bounds;

            int right = bounds.Width; 
            int bottom = bounds.Height; 
            int left = right - (bounds.Width / int.Parse(textBox3.Text)); 
            int top = bottom - (bounds.Height / int.Parse(textBox1.Text)); 

            // Создайте Bitmap для хранения скриншота
            screenshot = new Bitmap(bounds.Width / int.Parse(textBox4.Text), bounds.Height / int.Parse(textBox4.Text));

            using (Graphics graphics = Graphics.FromImage(screenshot))
            {
                graphics.CopyFromScreen(left, top, 0, 0, screenshot.Size);
            }

            pictureBox1.Image = screenshot;
        }
        private void pictureBox1_Click(object sender, EventArgs e) // Screen Shot IMG
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (firstClick == Point.Empty)
            {
                firstClick = me.Location;
            }
            else if (secondClick == Point.Empty)
            {
                secondClick = me.Location;
            }
            if (firstClick != Point.Empty & secondClick != Point.Empty)
            {
                if (gridDist == 0)
                {
                    WriteGridLine();
                }
                else
                {
                    WriteDistanceLine();
                }

                firstClick = Point.Empty;
                secondClick = Point.Empty;
            }
            UpdateViewPointStatus();
        }
        private void WriteGridLine()
        {
            using (Graphics g = pictureBox1.CreateGraphics())
            {
                Pen pen = new Pen(Color.Green, 2); 
                g.DrawLine(pen, firstClick, secondClick);
            }
            using (Graphics g = pictureBox1.CreateGraphics())
            {
                Font font = new Font("Arial", 10);
                SolidBrush brush = new SolidBrush(Color.Green);
                g.DrawString("Grid Size", font, brush, secondClick.X + 5, secondClick.Y + 5); 
            }
            gridDist = CalculateDistance(firstClick, secondClick);
            label1.Text = "Grid Size: " + Math.Round(gridDist).ToString();
            label1.BackColor = Color.Green;
        }
        private void WriteDistanceLine()
        {
            Color lineColor = GetRandomColor();

            using (Graphics g = pictureBox1.CreateGraphics())
            {
                Pen pen = new Pen(lineColor, 2); 
                g.DrawLine(pen, firstClick, secondClick);
            }

            targetDistance = CalculateDistance(firstClick, secondClick);
            label3.Text = "Line Distance: " + Math.Round(targetDistance).ToString();

            if (textBox2.Text != "")
            {
                label5.Text = Math.Round(CalculateDistanceInGame(Double.Parse(textBox2.Text), gridDist, targetDistance)).ToString() + "m";
            }
            if (textBox5.Text != "")
            {
                label10.Text = "Arrival time: " + (CalculateDistanceInGame(Double.Parse(textBox2.Text), gridDist, targetDistance) / Double.Parse(textBox5.Text)).ToString("F2") + "s";
            }
            label12.Text = "Azimuth: " + CalculateAzimuth(firstClick, secondClick).ToString("F2");

            using (Graphics g = pictureBox1.CreateGraphics())
            {
                Font font = new Font("Arial", 10);
                SolidBrush blackBrush = new SolidBrush(Color.Black);
                SolidBrush textBrush = new SolidBrush(IncreaseBrightness(lineColor, 100)); 

                SizeF textSize = g.MeasureString($"{label5.Text}\n{label12.Text}", font);

                float rectangleWidth = textSize.Width + 10;
                float rectangleHeight = textSize.Height + 10;
                float rectangleX = secondClick.X + 5; 
                float rectangleY = secondClick.Y + 5; 

                g.FillRectangle(blackBrush, rectangleX, rectangleY, rectangleWidth, rectangleHeight);

                g.DrawString($"{label5.Text}\n{label12.Text}", font, textBrush, rectangleX + 5, rectangleY + 5); 

                font.Dispose();
                blackBrush.Dispose();
                textBrush.Dispose();
            }

        }
        private double CalculateDistance(Point p1, Point p2)
        {
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        private double CalculateAzimuth(Point p1, Point p2)
        {
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;

            double radians = Math.Atan2(dy, dx);

            double degrees = radians * (180 / Math.PI);
            degrees = (degrees + 360) % 360; 

            degrees += 90;
            degrees %= 360;

            return degrees;
        }


        private void button2_Click(object sender, EventArgs e) // Set Grid
        {
            gridDist = 0;
            label1.BackColor = Color.Red;
            label1.Text = "Grid Size: ";
            firstClick = Point.Empty; 
            secondClick = Point.Empty; 
            pictureBox1.Invalidate();
        }

        private double CalculateDistanceInGame(double gameGridSize, double gridDist, double targetDistance)
        {
            return (gameGridSize / gridDist) * targetDistance;
        }

        private void button3_Click(object sender, EventArgs e) // Clear Lines
        {
            pictureBox1.Invalidate();
        }    
        private void UpdateViewPointStatus()
        {
            if (firstClick == Point.Empty)
                label11.BackColor = Color.Red;
            else
                label11.BackColor = Color.Green;
        }
        private Color GetRandomColor()
        {
            Random random = new Random();
            int red = random.Next(256); 
            int green = random.Next(256);
            int blue = random.Next(256);
            Color randomColor = Color.FromArgb(red, green, blue);
            return randomColor;
        }
        private Color IncreaseBrightness(Color color, int increment)
        {
            int red = Math.Min(color.R + increment, 255);
            int green = Math.Min(color.G + increment, 255);
            int blue = Math.Min(color.B + increment, 255);
            return Color.FromArgb(red, green, blue);
        }
    }
}
