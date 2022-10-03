

using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public abstract class Animal
    {
        protected int sate;
        protected readonly int maxSate;
        public Color Color;

        public int Sate { get {
                return sate;
            }
        }

        public virtual void Eat(int food)
        {
            sate += food;
        }

        // Minden ami egy generáció alatt történik, az az updateban kell legyen. pl: Lépés, evés, szaporodás
        public abstract void Update();

    }
}
