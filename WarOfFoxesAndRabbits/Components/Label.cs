using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarOfFoxesAndRabbits
{
    public class Label : Component
    {
        public string Text { get; set; }

        public Label(string text, Vector2 position)
        {
            this.Text = text;
            this.Position = position;

            Color = Color.Gray;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.DrawString(spriteFont, Text,
            new Vector2(Position.X, Position.Y), Color.Black);
        }
    }
}
