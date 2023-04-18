using System.Numerics; using Raylib_cs;

namespace SharpSand;

////
// TODO
// - Fix back and forth "jittering" (very obvious when burning Plant)

class Fire : Gas {
    public Fire(Vector2 position) : base(position) {
        Lifetime = 10;
        Density = -1.0f;
        IsHeating = true;
        BaseColor = Effect.GetFireColor();
        Color = BaseColor;
    }

    public override void Step(Matrix matrix) {
        // 25% chance to extend lifetime by 1
        if (RNG.Chance(25))
            Lifetime++;

        base.Step(matrix);
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        // Apply heat to non-heating neighbors
        if (!other.IsHeating && RNG.Roll(other.HeatPotential))
            other.ReceiveHeating(matrix);
    }

    public override void Expire(Matrix matrix) {
        matrix.Set(Position, new Air(Position));
    }
}