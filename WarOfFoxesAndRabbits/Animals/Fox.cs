using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace WarOfFoxesAndRabbits
{
    class Fox : Animal
    {

        public Fox()
        {
            Color = Color.Red;
            sate = 7;
            age = 0;

            maxSate = 10;
            maxAge = 45;
        }

        public void Eat()
        {
            sate += 3;
        }

        public override void Update()
        {
            sate--;
            age++;
        }


    }
}
