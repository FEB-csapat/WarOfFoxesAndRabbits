using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WarOfFoxesAndRabbits
{
    public abstract class Component
    {
        public Object Id { get; protected set; }
        public Vector2 Position { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public virtual Color Color { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D rectangleBlock)
        {
            spriteBatch.Draw(rectangleBlock,
                    new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color);
        }
    }
}
