using Microsoft.Xna.Framework;
using System.Formats.Asn1;
using System.Runtime.InteropServices;

namespace WarOfFoxesAndRabbits
{
    class Rabbit : Animal
    {
        protected new const int maxSate = 5;

        public Rabbit()
        {
            Color = new Color(255, 255, 255);
            sate = 1;
        }

        public void Eat()
        {
            sate++;
        }

        public override void Update()
        {
            sate--;
        }
    }
}
