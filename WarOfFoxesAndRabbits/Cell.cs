using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace WarOfFoxesAndRabbits
{
    public class Cell
    {
        private Vector2 position;

        public int cellX, cellY;

        public Texture2D texture2d;


        // the cell's 'inhabitant'. Can be Rabbit or Fox
        public Animal animal;

        public Matter matter;


        public Cell(Vector2 position, int cellX, int cellY, Matter matter = null, Animal animal = null )
        {
            this.position = position;
            this.cellX = cellX;
            this.cellY = cellY;
            if (matter == null)
            {
                this.matter = new Grass();
            }
            else
            {
                this.matter = matter;
            }
            this.animal = animal;
            
        }

        public Vector2 Position { get => position; }

        public Color Color
        {
            get
            {
                if (animal == null)
                {
                    return matter.Color;
                }
                else
                { 
                    return animal.Color;
                }
            }
        }
    }
}
