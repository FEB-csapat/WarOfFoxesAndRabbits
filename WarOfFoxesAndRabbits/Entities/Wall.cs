using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public class Wall : Matter
    {
        public Wall() : base() { }

        public override Color Color => new Color(60, 60, 60);
    }
}
