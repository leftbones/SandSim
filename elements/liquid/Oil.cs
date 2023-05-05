using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Oil : Liquid {
    public Oil(Vector2i position) : base(position) {
        Flammability = 0.04f;
        BurnDamageModifier = 0.01f;
        Spread = 7.5f;
        Density = 99.0f;
        ForceAct = true;
        ColorOffset = 0;
        BaseColor = new Color(239, 71, 111, 255);
        ModifyColor();
    }

    public override void HeatReaction(Matrix matrix) {
        OnFire = true;
    }

    public override void Expire(Matrix matrix) {
        if (OnFire) {
            // Small chance to cause explosion
            if (RNG.Roll(0.0005f))
                matrix.AddExplosion(Position, 10, 100.0f);
            
            // Attempt to create smoke
            foreach (Vector2i Dir in Direction.Upward) {
                if (matrix.IsEmpty(Position + Dir))
                    matrix.Set(Position + Dir, new Smoke(Position + Dir));
            }
        }

        matrix.Set(Position, new Smoke(Position));
    }
}