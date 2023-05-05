using System.Numerics;
using Raylib_cs;

namespace SharpSand;


class Lava : Liquid {
    public Lava(Vector2i position) : base(position) {
        Spread = 2.0f;
        Density = 101.0f;
        OnFire = true;
        BurnDamageModifier = 0.0f;
        HeatFactor = 10.0f;
        NoFireColor = true;
        BaseColor = Effect.DarkenColor(Effect.GetFireColor(), 50);
        Color = BaseColor;
    }

    public override void Step(Matrix matrix) {
        // Chance to create fire if the space above is empty
        if (RNG.Roll(0.001f) && matrix.IsEmpty(Position + Direction.Up)) {
            var Fire = new Fire(Position + Direction.Up);
            Fire.Lifetime /= 2;
            matrix.Set(Position + Direction.Up, Fire);
        }

        // Chance to change color
        if (RNG.Chance(1))
            Color = Effect.DarkenColor(Effect.GetFireColor(), 50);

        base.Step(matrix);
    }

    public override void CoolReaction(Matrix matrix) {
        matrix.Set(Position, new Stone(Position));
    }
}