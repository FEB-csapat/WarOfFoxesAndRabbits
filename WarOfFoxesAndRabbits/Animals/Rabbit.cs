using Microsoft.Xna.Framework;
using System;

namespace WarOfFoxesAndRabbits
{
    class Rabbit : Animal
    {
        
        public Rabbit()
        {
            Color = new Color(102,102,102);
        }

        public override void Eat(int grass)
        {
            sate += grass;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
