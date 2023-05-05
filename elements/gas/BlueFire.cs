using System.Numerics; using Raylib_cs;

namespace SharpSand;

class BlueFire : Gas {
    public BlueFire(Vector2i position) : base(position) {
        Lifetime = 20;
        Density = -1.0f;
        OnFire = true;
        BurnDamageModifier = 0.0f;
        CoolFactor = 50.0f;
        NoFireColor = true;
        BaseColor = Effect.GetBlueFireColor();
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