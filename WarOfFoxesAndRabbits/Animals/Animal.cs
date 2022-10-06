

using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    public abstract class Animal
    {
        protected int sate;
        protected int maxSate;
        public Color Color;

        public bool hasMoved = false;
        public bool hasProduced = false;

        public int Sate
        {
            get
            {
                return sate;
            }
        }

        public virtual void Eat(int food)
        {
            sate += food;
        }

        public virtual bool Die()
        {
            if (Sate <= 0)
                return true;
            else
                return false;
        }

        // Minden ami egy generáció alatt történik, az az updateban kell legyen. pl: evés, szaporodás
        public virtual void Update()
        {
            sate--;
        }

    }
}
