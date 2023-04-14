using System.Numerics;
using Raylib_cs;

namespace SharpSand;

abstract class Gas : Element {
    public Gas(Vector2 position) : base(position) {
        Type = ElementType.Gas;
        DispersionRate = 5;
    }

    public override void Update(Matrix matrix) {
        List<Element> Neighbors = matrix.GetNeighbors(Position);
        if (Neighbors.OfType<Air>().Any())
            Settled = false;

        // Spread outward if up is not an option
        if (matrix.IsEmpty(Position + Direction.Up)) {
            if (matrix.Tick % 2 == 0) {
                for (int i = 0; i < DispersionRate; i++) {
                    if (RNG.CoinFlip() && !matrix.SwapIfEmpty(Position, Position + Direction.Left)) {
                        Settled = true;
                        return;
                    }
                }
            } else {
                for (int i = 0; i < DispersionRate; i++) {
                    if (RNG.CoinFlip() && !matrix.SwapIfEmpty(Position, Position + Direction.Right)) {
                        Settled = true;
                        return;
                    }
                }
            }
        }

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