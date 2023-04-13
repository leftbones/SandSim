using System.Numerics;
using Raylib_cs;

namespace SharpSand;

abstract class Gas : Element {
    public Gas(Vector2 position) : base(position) {
        Type = ElementType.Gas;
    }

    public override void Update(Matrix matrix) {
        List<Element> Neighbors = matrix.GetNeighbors(Position);
        if (Neighbors.OfType<Air>().Any())
            Settled = false;

        // Disperse in any direction, higher chance to go upward
        List<Vector2> Directions = Direction.Full.OrderBy(a => RNG.Random.Next()).ToList();
        foreach (Vector2 Dir in Directions) {
            int Chance = 10;
            if (Direction.UpperHalf.Contains(Dir))
                Chance = 50;

            if (RNG.Chance(Chance) && matrix.SwapIfEmpty(Position, Position + Dir))
                return;
        }

        Settled = true;
    }
}


////
// Air doesn't really need it's own file because it doesn't do anything
class Air : Gas {
    public Air(Vector2 position) : base(position) {
        Color = Color.BLACK;
    }

    public override void Update(Matrix matrix) { }
}