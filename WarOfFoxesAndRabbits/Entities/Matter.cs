using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public abstract class Matter : IEntity
    {
        public Matter() : base() { }

        public abstract Color Color { get; }
    }
}
