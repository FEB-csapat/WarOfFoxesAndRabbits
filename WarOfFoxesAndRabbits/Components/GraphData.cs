using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class GraphData : Component
    {
        public AnimalType AnimalType { get; private set; }

        public GraphData(AnimalType animalType, Vector2 position)
        {
            this.AnimalType = animalType;
            this.Position = position;

            Width = GameConstants.GRAPH_RECT_SIZE;
            Height = GameConstants.GRAPH_RECT_SIZE;
        }

        public override Color Color => AnimalType == AnimalType.RABBIT ? Color.White : Color.Red;

        public void Update()
        {
            Position += new Vector2(-GameConstants.GRAPH_RECT_SIZE, 0);
        }
    }
}
