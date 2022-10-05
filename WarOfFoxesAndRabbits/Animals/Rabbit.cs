using Microsoft.Xna.Framework;

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

        public override bool Die()
        {
            if (Sate == 0)
                return true;
            else
                return false;
        }
        public override void Update()
        {
            sate--;
        }
        
    }
}
