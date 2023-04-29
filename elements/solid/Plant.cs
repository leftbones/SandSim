using Raylib_cs;

namespace SharpSand;

class Plant : Solid {
    private float GrowthChance = 0.08f;
    private bool CanGrow = true;
    private bool Acted = false;

    public Plant(Vector2i position) : base(position) {
        Flammability = 0.04f;
        BurnDamageModifier = 1.0f;
        ActDirections = Direction.ShuffledCardinal;
        ForceAct = true;
        BaseColor = new Color(7, 197, 102, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        Acted = false;
        base.Step(matrix);
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        // Can't grow anymore
        if (!CanGrow)
            return;

        // Already acted this step
        if (Acted)
            return;

        // Attempt to grow to nearby water
        if (other.GetType() == typeof(Water)) {
            if (!OnFire && RNG.Roll(GrowthChance)) {
                if (CanGrow) {
                    matrix.Set(other.Position, new Plant(other.Position));
                    if (RNG.Roll(GrowthChance / 2.0f))
                        CanGrow = false;
                }
            } else {
                Acted = true;
            }
        }
    }

    public override void HeatReaction(Matrix matrix) {
        // OnFire = true;
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