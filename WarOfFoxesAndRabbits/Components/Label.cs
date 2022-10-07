using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    internal class Label : Component
    {
        public string text;

        public Label(string text, Vector2 position)
        {
            this.text = text;
            this.position = position;
        }
    }
}
