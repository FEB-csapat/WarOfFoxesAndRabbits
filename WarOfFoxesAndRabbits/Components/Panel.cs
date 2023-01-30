using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class Panel : Component
    {
        public Panel(Vector2 postion, int width, int height, Color color)
        {
            Position = postion;
            Width = width;
            Height = height;
            Color = color;
        }
    }
}
