using Microsoft.Xna.Framework;

namespace WarOfFoxesAndRabbits
{
    class Rabbit : Animal
    {
        public Rabbit() : base()
        {
            Sate = 3;
            Age = 0;
            MaxSate = 9;
            MaxAge = 60;

            HasAte = true;
            HasMoved = true;
            HasProduced = true;
        }

        public override Color Color => Color.White;

        public override bool CanBreed() => Sate >= 4;
        
        public override bool CanEat() => Sate < 5;

        public void Eat(int amount)
        {
            if (Sate < MaxSate)
            {
                Sate += amount;
            }
        }
    }
}
