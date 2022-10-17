using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class Cell
    {
        private Vector2 Position { get; set; }

        public int PosX { get => (int)Position.X; }
        public int PosY { get => (int)Position.Y; }

        public Animal Animal { get; set;}
        public Matter Matter{get; set;}

        public Cell(Vector2 position, Matter matter = null, Animal animal = null)
        {
            this.Position = position;
            
            if (matter == null)
            {
                this.Matter = new Grass();
            }
            else
            {
                this.Matter = matter;
            }
            this.Animal = animal;
        }

        public Color Color
        {
            get
            {
                if (Animal == null)
                {
                    return Matter.Color;
                }
                else
                {
                    return Animal.Color;
                }
            }
        }
    }
}
