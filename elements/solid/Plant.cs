using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Plant : Solid {
    public float GrowthChance = 0.08f;

    public Plant(Vector2i position) : base(position) {
        Flammability = 0.01f;
        // BurnDamageModifier = 2.0f;
        ActDirections = Direction.Cardinal;
        ForceAct = true;
        BaseColor = new Color(7, 197, 102, 255);
        ModifyColor();
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        // Attempt to spread to neighboring water
        if (other.GetType() == typeof(Water)) {
            if (!OnFire && RNG.Roll(GrowthChance))
                matrix.Set(other.Position, new Plant(other.Position));
        }
    }

    public override void HeatReaction(Matrix matrix) {
        if (!OnFire)
            OnFire = true;
    }

    public override void Expire(Matrix matrix) {
        if (OnFire) {
            // Create smoke if space above is empty
            if (matrix.IsEmpty(Position + Direction.Up))
                matrix.Set(Position + Direction.Up, new Smoke(Position + Direction.Up));

            // Small chance to create an ember
            if (RNG.Chance(1)) {
                matrix.Set(Position, new Ember(Position));
                return;
            }
        }
        matrix.Set(Position, new Air(Position));
    }
}