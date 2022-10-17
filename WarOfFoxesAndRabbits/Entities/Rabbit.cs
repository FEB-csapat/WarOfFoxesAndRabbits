using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    class Rabbit : Animal
    {
        public Rabbit() : base()
        {
            InitialSate = 3;
            MaxSate = 12;
            MaxAge = 90;

            Initialize();
        }

        public override Color Color => Color.White;

        public override bool CanBreed() => Sate >= 4;
        
        public override bool CanEat() => Sate < 7;

        public void Eat(int amount)
        {
            if (Sate < MaxSate)
            {
                Sate += amount;
            }
        }
    }
}
