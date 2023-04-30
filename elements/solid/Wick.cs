using Raylib_cs;

namespace SharpSand;

class Wick : Solid {
    public Wick(Vector2i position) : base(position) {
        Health = 10.0f;
        Flammability = 0.1f;
        ForceAct = true;
        ColorOffset = 5;
        BaseColor = new Color(48, 17, 17, 255);
        ModifyColor();
    }

    public override void HeatReaction(Matrix matrix) {
        OnFire = true;
    }

    public override void Expire(Matrix matrix) {
        if (OnFire) {
            // Create small explosion or smoke
            if (RNG.CoinFlip())
                matrix.AddExplosion(Position, 5, 10.0f);
            else
                matrix.Set(Position, new Smoke(Position));
        } else {
            matrix.Set(Position, new Air(Position));
        }
    }
}