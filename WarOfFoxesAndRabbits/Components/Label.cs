using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class Label : Component
    {
        public string Text { get; set; }

        public Label(string text, Vector2 position)
        {
            this.Text = text;
            this.Position = position;
        }
    }
}
