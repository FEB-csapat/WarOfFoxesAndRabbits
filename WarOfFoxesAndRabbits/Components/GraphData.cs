using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class GraphData
    {
        public AnimalType AnimalType { get; private set; }
        public Vector2 Position { get; private set; }

        public GraphData(AnimalType animalType, Vector2 position)
        {
            this.AnimalType = animalType;
            this.Position = position;
        }

        public Color Color
        {
            get
            {
                if (AnimalType == AnimalType.RABBIT)
                {
                    return Color.White;
                }
                return Color.Red;
            }
        }

        public void Update()
        {
            Position += new Vector2(-GameConstants.GraphRectSize, 0);
        }
    }
}
