using System.Numerics;
using Raylib_cs;

namespace SharpSand;


class Lava : Liquid {
    public Lava(Vector2 position) : base(position) {
        Spread = 2.0f;
        CoolPotential = 0.001f;
        IsHeating = true;
        BaseColor = Effect.DarkenColor(Effect.GetFireColor(), 50);
        Color = BaseColor;
    }

    public override void Step(Matrix matrix) {
        // Chance to create fire if the space above is empty
        if (RNG.Roll(0.001f) && matrix.SwapIfEmpty(Position, Position + Direction.Up)) {
            var Fire = new Fire(Position + Direction.Up);
            Fire.Lifetime /= 2;
            matrix.Set(Position + Direction.Up, Fire);
        }

        // Chance to change color
        if (RNG.Chance(1))
            Color = Effect.DarkenColor(Effect.GetFireColor(), 50);

        base.Step(matrix);
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        // Apply heat to non-heating neighbors, destroy them if they are on fire
        if (!other.IsHeating && RNG.Roll(other.HeatPotential)) {
            if (other.OnFire)
                other.ReplaceWith(matrix, new Smoke(other.Position));
            else
                other.ReceiveHeating(matrix);
        }
    }

    public override void ReceiveCooling(Matrix matrix) {
        matrix.Set(Position, new Air(Position));
    }
}