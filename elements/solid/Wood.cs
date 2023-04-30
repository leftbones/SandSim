using Raylib_cs;

namespace SharpSand;

class Wood : Solid {
    private float GrowthChance = 0.08f;
    private bool CanGrow = true;
    private bool Acted = false;

    public Wood(Vector2i position) : base(position) {
        Health = 50.0f;
        Flammability = 0.005f;
        BurnDamageModifier = 0.12f;
        ActDirections = Direction.ShuffledCardinal;
        ForceAct = true;
        ColorOffset = 8;
        BaseColor = new Color(71, 62, 39, 255);
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

        // Attempt to create plant from nearby water
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
            } else {
                matrix.Set(Position, new Soot(Position));
                return;
            }
        }
        matrix.Set(Position, new Air(Position));
    }
}