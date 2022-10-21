using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class Water : Matter
    {
        private float depth;

        public Water(float depth) : base()
        {
            this.depth = depth;
        }

        public override Color Color{
            get{ 
                return MapColor();
            }
        }

        private Color MapColor()
        {
            double h =1- GameConstants.minWaterDepth;

            double p = (1- depth) / h;

            return new Color(((int)((31 ) * p )), ((int)((181 - 57) * p + 57)), ((int)((255 - 166) * p + 166)));
        }
    }
}
