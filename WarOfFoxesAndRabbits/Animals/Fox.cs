using Microsoft.Xna.Framework;
using System;

namespace WarOfFoxesAndRabbits
{
    class Fox : Animal
    {

        public Fox()
        {
            Color = Color.Red;
        }

        // eats rabbit
        public void Eat()
        {
            sate += 3;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
