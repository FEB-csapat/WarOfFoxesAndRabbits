using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;


namespace WarOfFoxesAndRabbits
{
    public class Cell
    {

        private Vector2 position;

        public Texture2D texture2d;

        private GraphicsDevice graphicsDevice;

        
        private int grass = 0;

        // the cell's 'inhabitant'. Can be Rabbit or Fox
        public Animal inhabitant;

        public Cell(Vector2 position, GraphicsDevice graphicsDevice)
        {
            this.position = position;
            this.graphicsDevice = graphicsDevice;



            setTexture();

            grass = new Random().Next(0,3);

            // randomly spawn a rabbit
            inhabitant = new Random().Next(0, 2) == 1 ? new Rabbit() : null;
        }

        // If we want to change the cell's color, we need to call this first
        public void setTexture()
        {
            Color[] data = new Color[GameVariables.cellSize * GameVariables.cellSize];

            for (int i = 0; i < data.Length; ++i)
                data[i] = Color;

            texture2d = new Texture2D(graphicsDevice, GameVariables.cellSize, GameVariables.cellSize);
            texture2d.SetData(data);
        }

        public void asdasd() {
            Color[] data = new Color[GameVariables.cellSize * GameVariables.cellSize];

            for (int i = 0; i < data.Length; ++i)
                data[i] = Color;
            texture2d.SetData<Color>(data);

                }

        public void Grow()
        {
            if (grass<2)
            {
                grass++;
            }
        }

        public int Grass { get => grass; }

        public Vector2 Position { get => position; }

        public Color Color { get {
                if (inhabitant == null)
                {
                    switch (grass)
                    {
                        case 0:
                            return new Color(130, 200, 0);
                        case 1:
                            return new Color(111, 183, 0);
                        case 2:
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

        public void Update()
        {
            inhabitant?.Update();
            Grow();
        }
    }
}
