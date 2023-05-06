using Raylib_cs;

namespace SharpSand;

class ConcreteMix : Powder {
    private bool Wet = false;
    private int DryingTime = 1000;

    public ConcreteMix(Vector2i position) : base(position) {
        Friction = 0.01f;
        BaseColor = new Color(177, 178, 168, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        if (!Wet) {
            var Neighbors = matrix.GetNeighbors(Position);
            foreach (Element N in Neighbors) {
                Type N_Type = N.GetType();
                if (N_Type == typeof(Water) || N_Type == typeof(Saltwater)) {
                    Wet = true;
                    Color = Effect.DarkenColor(Color, 100);
                    matrix.Set(N.Position, new Air(N.Position));
                    break;
                }
            }
        } else {
            if (DryingTime == 0) {
                if (Settled)
                    matrix.Set(Position, new Stone(Position));
            } else {
                DryingTime--;
            }
        }

        base.Step(matrix);
    }
}