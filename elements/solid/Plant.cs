using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Plant : Solid {
    public float GrowthChance = 0.08f;

    public Plant(Vector2 position) : base(position) {
        HeatPotential = 1.0f;
        BurnDamageModifier = 2.0f;
        ActDirections = Direction.Cardinal;
        ForceAct = true;
        BaseColor = new Color(7, 197, 102, 255);
        ModifyColor();
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        // Attempt to spread to neighboring water
        if (other is Water) {
            if (!OnFire && RNG.Roll(GrowthChance))
                matrix.Set(other.Position, new Plant(other.Position));
        }
    }

    public override void ReceiveHeating(Matrix matrix) {
        if (!OnFire)
            OnFire = true;
    }

    public override void Expire(Matrix matrix) {
        if (OnFire) {
            // Small chance to create an ember
            if (RNG.Chance(1))
                matrix.Set(Position, new Ember(Position));

            // Create smoke if space above is empty
            if (matrix.IsEmpty(Position + Direction.Up))
                matrix.Set(Position + Direction.Up, new Smoke(Position + Direction.Up));
        }

        matrix.Set(Position, new Air(Position));
    }
}