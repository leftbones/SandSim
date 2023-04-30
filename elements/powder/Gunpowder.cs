using System.Linq;
using Raylib_cs;

namespace SharpSand;

class Gunpowder : Powder {
    public Gunpowder(Vector2i position) : base(position) {
        Friction = 0.1f;
        Flammability = 0.5f;
        BaseColor = new Color(24, 24, 26, 255);
        ModifyColor();
    }

    public override void Expire(Matrix matrix) {
        int multi = Math.Max(1, matrix.GetNeighbors(Position).Where(e => e != null && e.GetType() == typeof(Gunpowder)).Count());
        matrix.AddExplosion(Position, 5 * multi, 75.0f * multi);
        base.Expire(matrix);
    }
}