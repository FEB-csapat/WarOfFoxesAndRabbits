using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace WarOfFoxesAndRabbits
{
    public enum AnimalType
    {
        FOX, RABBIT
    }
    public struct GraphAnimalData
    {
        


        public AnimalType animalType;
        public Vector2 position;


        public GraphAnimalData(AnimalType animalType, Vector2 position)
        {
            this.animalType = animalType;
            

              this.position = position;
        }

        public Color Color { 
            get {
                if (animalType == AnimalType.RABBIT)
                {
                    return Color.White; 
                }
                return Color.Red;
            } 
        }

        public void Update()
        {
            position.X -= GameVariables.GraphRectSize;
        }
    }



    class Graph : Component
    {

        public int width, height;

        public Texture2D image;

        public List<GraphAnimalData> datas = new List<GraphAnimalData>();  

        
        public Graph(Vector2 position, Texture2D image = null, int width = 300, int height = 150)
        {
            this.position = position;
            this.image = image;
            this.width = width;
            this.height = height;
        }

        public void AddData(AnimalType animalType, int count)
        {
            int max = GameVariables.CellsHorizontallyCount * GameVariables.CellsVerticallyCount;

            float percent = count / max;

            float posY = percent*height;

            datas.Add(new GraphAnimalData(animalType, new Vector2( position.X + width - GameVariables.GraphRectSize, posY) ));
        }

        public void Update()
        {
            foreach (GraphAnimalData d in datas)
            {
                d.Update();
            }
        }


        

    }

}
