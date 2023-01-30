using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarOfFoxesAndRabbits
{
    public class Cell
    {
        public int RowPosition { get; private set; }
        public int ColumnPosition { get; private set; }

        public int PositionX { get => RowPosition * GameConstants.CELL_SIZE; }
        public int PositionY { get => ColumnPosition * GameConstants.CELL_SIZE; }

        public Animal Animal { get; set; }
        public Matter Matter { get; set; }

        public Cell(int rowPosition, int columnPosition, Matter matter = null, Animal animal = null)
        {
            RowPosition = rowPosition;
            ColumnPosition = columnPosition;

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


        public void Draw(SpriteBatch spriteBatch, Texture2D rectangleBlock)
        {
            spriteBatch.Draw(rectangleBlock,
                new Rectangle(PositionX, PositionY,
                                GameConstants.CELL_SIZE, GameConstants.CELL_SIZE), Color);
        }
    }
}
