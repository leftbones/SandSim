using System.Numerics; using Raylib_cs;

namespace SharpSand;

class Fire : Gas {
    public Fire(Vector2i position) : base(position) {
        Lifetime = 20;
        Density = -1.0f;
        OnFire = true;
        BurnDamageModifier = 0.0f;
        HeatFactor = 5.0f;
        NoFireColor = true;
        BaseColor = Effect.GetFireColor();
        Color = BaseColor;
    }

    public override void Step(Matrix matrix) {
        // 25% chance to extend lifetime by 1
        if (RNG.Chance(25))
            Lifetime++;

        base.Step(matrix);
    }

    public override void Expire(Matrix matrix) {
        matrix.Set(Position, new Air(Position));
    }
}