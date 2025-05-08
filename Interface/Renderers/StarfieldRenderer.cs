/* ----- ----- ----- ----- */
// StarfieldRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2025/05/08
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Chinese_Chess_v3.Interface.Renderers
{
    public class StarfieldRenderer
    {
        private class Star
        {
            public float X;
            public float Y;
            public float Size;
            public float Speed;

            public Star(Random rand, int width, int height)
            {
                X = rand.Next(width);
                Y = rand.Next(height);
                Size = rand.Next(1, 3);
                Speed = (float)(0.5 + rand.NextDouble() * 1.5);
            }
        }

        private List<Star> stars = new List<Star>();
        private readonly Random rand = new Random();
        private int starCount = 100;

        private int width;
        private int height;

        public StarfieldRenderer(int width, int height, int starCount = 100)
        {
            this.width = width;
            this.height = height;
            this.starCount = starCount;
            InitializeStars();
        }

        private void InitializeStars()
        {
            stars.Clear();
            for (int i = 0; i < starCount; i++)
            {
                stars.Add(new Star(rand, width, height));
            }
        }

        public void Update()
        {
            foreach (var star in stars)
            {
                star.Y += star.Speed;
                if (star.Y > height)
                {
                    star.X = rand.Next(width);
                    star.Y = 0;
                    star.Size = rand.Next(1, 3);
                    star.Speed = (float)(0.5 + rand.NextDouble() * 1.5);
                }
            }
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Black, 0, 0, width, height);

            foreach (var star in stars)
            {
                float alpha = Math.Max(0.2f, (float)rand.NextDouble());
                Color starColor = Color.FromArgb((int)(alpha * 255), 255, 255, 255);
                using (Brush brush = new SolidBrush(starColor))
                {
                    g.FillEllipse(brush, star.X, star.Y, star.Size, star.Size);
                }
            }
        }

        public void Resize(int newWidth, int newHeight)
        {
            width = newWidth;
            height = newHeight;
            InitializeStars();
        }
    }
}