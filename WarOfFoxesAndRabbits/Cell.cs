using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace WarOfFoxesAndRabbits
{
    public class Cell
    {
        private Vector2 position;

        public Texture2D texture2d;

        private GraphicsDevice graphicsDevice;


        private double grass = 0;

        // the cell's 'inhabitant'. Can be Rabbit or Fox
        public Animal inhabitant;

        public Cell(Vector2 position, GraphicsDevice graphicsDevice)
        {
            this.position = position;
            this.graphicsDevice = graphicsDevice;

            grass = new Random().Next(0, 3);
        }

        public void Grow()
        {
            if (grass < 2)
            {
                grass+=0.2;
            }
        }

        public double Grass { get => grass; }

        public void grassEaten()
        {
            if (grass<=1)
            {
                grass = 0;
            }
            else
            {
                grass--;
            }
        }

        public Vector2 Position { get => position; }

        public Color Color
        {
            get
            {
                if (inhabitant == null)
                {
                    
                    switch (grass)
                    {
                        case > 0 and < 1:
                            return new Color(130, 200, 0);
                        case >= 1 and < 2:
                            return new Color(111, 183, 0);
                        case >= 2:
                            return new Color(0, 183, 0);
                    }
                }
                else if (inhabitant != null)
                {
                    return inhabitant.Color;
                }
                return Color.Black;
            }
        }
    }
}
