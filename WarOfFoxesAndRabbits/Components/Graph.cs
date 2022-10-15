using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace WarOfFoxesAndRabbits
{
    class Graph : Component
    {
        List<GraphData> datas = new List<GraphData>();

        public int Width { get; private set; }
        public int Height { get; private set; }

        public List<GraphData> Datas => datas;

        public Graph(Vector2 position, int width = 300, int height = 150)
        {
            this.Position = position;
            this.Width = width;
            this.Height = height;
        }

        public void AddData(AnimalType animalType, int count)
        {
            int max = GameVariables.CellsHorizontallyCount * GameVariables.CellsVerticallyCount;

            double percent = (double) count / max;
            double posY = Position.Y + Height - GameVariables.GraphRectSize - percent*Height*10;

            datas.Add(new GraphData(animalType, new Vector2( Position.X + Width , (float) posY) ));
        }

        public void Update()
        {
            for (int i = 0; i< datas.Count; i++)
            {
                if (datas[i].position.X < Position.X+GameVariables.GraphRectSize)
                {
                    datas.RemoveAt(i);
                }
                else
                {
                    datas[i].Update();
                }
            }
        }
    }
}
