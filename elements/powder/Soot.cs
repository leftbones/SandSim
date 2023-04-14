using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Soot : Powder {
    private int DissolveChance = 1;

    public Soot(Vector2 position) : base(position) {
        Friction = 1;
        BaseColor = new Color(93, 92, 94, 255);
        ModifyColor();
    }

    public override void Update(Matrix matrix) {
        List<Element> Neighbors = matrix.GetNeighbors(Position);
        if (Neighbors.OfType<Water>().Any() && RNG.Chance(DissolveChance))
            matrix.Set(Position, new Water(Position));

        base.Update(matrix);
    }
}