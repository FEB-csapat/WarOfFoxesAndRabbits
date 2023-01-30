using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WarOfFoxesAndRabbits
{
    class Button : Component
    {
        public string Text { get; set; }
        public Action OnClick
        {
            get; private set;
        }

        public Texture2D ImageTexture { get; private set; }


        public Button() { }

        public Button(Vector2 position, Action onClick, string text = "", Texture2D image = null,
            int width = 100, int height = 50, Object id = null)
        {
            this.Text = text;
            this.OnClick = onClick;
            this.Position = position;
            this.ImageTexture = image;
            this.Width = width;
            this.Height = height;

            this.Id = id;

            this.Color = Color.Gray;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D rectangleBlock, PencilType pencilSelected)
        {
            if (Id != null && (PencilType)Id == pencilSelected)
            {

                spriteBatch.Draw(rectangleBlock,
                    new Rectangle(((int)Position.X) - 4, ((int)Position.Y) - 4, Width + 6, Height + 6), Color.Black);
                /*
                spriteBatch.Draw(rectangleBlock,
                    new Rectangle(((int)button.Position.X) - 4, ((int)button.Position.Y) - 4, button.Width + 6, button.Height + 6), Color.Black);
                */
            }

            spriteBatch.Draw(ImageTexture, Position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D rectangleBlock, PencilSizeType pencilSizeSelected)
        {
            if (Id != null && (PencilSizeType)Id == pencilSizeSelected)
            {

                spriteBatch.Draw(rectangleBlock,
                    new Rectangle(((int)Position.X) - 4, ((int)Position.Y) - 4, Width + 6, Height + 6), Color.Black);
                /*
                spriteBatch.Draw(rectangleBlock,
                    new Rectangle(((int)button.Position.X) - 4, ((int)button.Position.Y) - 4, button.Width + 6, button.Height + 6), Color.Black);
                */
            }

            spriteBatch.Draw(ImageTexture, Position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D rectangleBlock, SpriteFont spriteFont)
        {
            Draw(spriteBatch, rectangleBlock);

            spriteBatch.DrawString(spriteFont, Text,
                new Vector2(Position.X + Width / 4, Position.Y + Height / 3), Color.Black);
        }
    }
}
