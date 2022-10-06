using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace WarOfFoxesAndRabbits
{
    internal class Button : Component
    {
        public string text;
        public Action onClick;

        public int width, height;

        public Texture2D image;

        public bool isSelected;

        public Button() { }

        public Button(Vector2 position, Action onClick, string text = "", Texture2D image = null, int width = 100, int height = 50, bool isSelected = false)
        {
            this.text = text;
            this.onClick = onClick;
            this.position = position;
            this.image = image;
            this.width = width;
            this.height = height;
        }
    }
}
