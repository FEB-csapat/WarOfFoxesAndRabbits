using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WarOfFoxesAndRabbits
{
    public class Cell
    {
        private Vector2 position;


        public Texture2D texture2d;

        // the cell's 'inhabitant'. Can be Rabbit or Fox
        public Animal animal;

        public Matter matter;

        public Cell(Vector2 position, Matter matter = null, Animal animal = null)
        {
            this.position = position;
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
