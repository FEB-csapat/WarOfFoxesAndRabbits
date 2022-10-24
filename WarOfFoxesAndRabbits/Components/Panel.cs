using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class Panel : Component
    {
        public Color Color { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Panel(Vector2 postion, int width, int height, Color color)
        {
            Position = postion;
            Width = width;
            Height = height;
            Color = color;
        }
    }
}
