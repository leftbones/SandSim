using Raylib_cs;

namespace SharpSand;

class Plastic : Solid {
    public Plastic(Vector2i position) : base(position) {
        Flammability = 0.005f;
        BurnDamageModifier = 0.1f;
        CorrosionResistance = 1.0f;
        ColorOffset = 5;
        BaseColor = new Color(146, 172, 172, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        // Create lots of smoke when on fire
        if (OnFire) {
            foreach (Vector2i Dir in Direction.Upward) {
                if (matrix.IsEmpty(Position + Dir))
                    matrix.Set(Position + Dir, new Smoke(Position + Dir));
            }
        }

        base.Step(matrix);
    }

    public override void Expire(Matrix matrix) {
        // Create lots of smoke when on fire
        if (OnFire) {
            foreach (Vector2i Dir in Direction.Upward) {
                if (matrix.IsEmpty(Position + Dir))
                    matrix.Set(Position + Dir, new Smoke(Position + Dir));
            }

            matrix.Set(Position, new Smoke(Position));
            return;
        }

        base.Expire(matrix);
    }
}