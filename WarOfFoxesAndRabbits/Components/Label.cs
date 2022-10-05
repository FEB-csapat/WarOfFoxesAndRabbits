using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
