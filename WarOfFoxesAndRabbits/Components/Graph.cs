using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace WarOfFoxesAndRabbits
{
    class Graph : Component
    {
        List<GraphData> datas = new List<GraphData>();

        private List<GraphData> Datas => datas;

        public Graph(Vector2 position, int width = 300, int height = 200)
        {
            this.Position = position;
            this.Width = width;
            this.Height = height;

            Color = Color.Gray;
        }

        public void AddData(AnimalType animalType, int count)
        {
            int max = GameConstants.CELLS_HORIZONTALLY_COUNT * GameConstants.CELLS_VERTICALLY_COUNT;

            double percent = (double)count / max;
            double posY = Position.Y + Height - GameConstants.GRAPH_RECT_SIZE - percent * Height * 10;

            datas.Add(new GraphData(animalType, new Vector2(Position.X + Width, (float)posY)));
        }

        public void Update()
        {
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].Position.X < Position.X + GameConstants.GRAPH_RECT_SIZE)
                {
                    datas.RemoveAt(i);
                }
                else
                {
                    datas[i].Update();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D rectangleBlock)
        {
            base.Draw(spriteBatch, rectangleBlock);
            Datas.ForEach(x => x.Draw(spriteBatch, rectangleBlock));

        }
    }
}
