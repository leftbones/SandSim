using Raylib_cs;

namespace SharpSand;

class Methane : Gas {
    public Methane(Vector2i position) : base(position) {
        Density = 0.0f;
        Drift = 0.4f;
        Flammability = 1.0f;
        BaseColor = new Color(128, 168, 217, 150);
        ModifyColor();
    }

    public override void Expire(Matrix matrix) {
        int multi = Math.Max(1, matrix.GetNeighbors(Position).Where(e => e != null && e.GetType() == typeof(Methane)).Count());
        matrix.AddExplosion(Position, 8 * multi, 75.0f * multi);
        base.Expire(matrix);
    }
}