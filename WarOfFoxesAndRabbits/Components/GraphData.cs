using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class GraphData
    {
        public AnimalType animalType;
        public Vector2 position;

        public GraphData(AnimalType animalType, Vector2 position)
        {
            this.animalType = animalType;
            this.position = position;
        }

        public Color Color
        {
            get
            {
                if (animalType == AnimalType.RABBIT)
                {
                    return Color.White;
                }
                return Color.Red;
            }
        }

        public void Update()
        {
            position += new Vector2(-GameVariables.GraphRectSize, 0);
        }
    }
}
