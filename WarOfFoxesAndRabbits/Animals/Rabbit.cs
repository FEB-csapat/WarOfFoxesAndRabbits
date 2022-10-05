using Microsoft.Xna.Framework;
using System;

namespace WarOfFoxesAndRabbits
{
    class Rabbit : Animal
    {
        protected new const int maxSate = 5;
        public Rabbit()
        {
            Color = new Color(255,255,255);
        }

        public void Eat()
        {
            sate++;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
