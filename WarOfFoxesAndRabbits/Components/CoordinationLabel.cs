using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WarOfFoxesAndRabbits
{
    public class CoordinationLabel : Label
    {
        public CoordinationLabel(string text, Vector2 position) : base(text, position) { }

        public void DrawCoordinate(SpriteBatch spriteBatch, Texture2D rectangleBlock, SpriteFont spriteFont)
        {
            if (Position.X - 10 <= GameConstants.GAME_CANVAS_WIDTH
            && Position.X - 10 >= 0
            && Position.Y - 10 >= 0
            && Position.Y - 10 <= GameConstants.GAME_CANVAS_WIDTH)
            {
                Draw(spriteBatch, spriteFont);
            }
        }
        public void UpdateLabel(MouseState currentMouseState)
        {
            Position = new Vector2(currentMouseState.X + 12, currentMouseState.Y + 12);
            Text = $"({1 + currentMouseState.X / GameConstants.CELL_SIZE}," +
                              $" {1 + currentMouseState.Y / GameConstants.CELL_SIZE})";
        }
    }
}
