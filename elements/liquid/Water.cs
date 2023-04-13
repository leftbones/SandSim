using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Water : Liquid {
    public Water(Vector2 position) : base(position) {
        Color = new Color(1, 151, 244, 255);
    }

    public override void Update(Matrix matrix) { 
        if (Settled)
            if (
                matrix.IsValid(Position + Direction.Down) || 
                matrix.IsValid(Position + Direction.Left) || 
                matrix.IsValid(Position + Direction.Right)
            )
                Settled = false;
            else
                return;

        if (matrix.Swap(Position, Position + Direction.Down))
            return;

        if (RNG.Chance(99) && matrix.Swap(Position, Position + Direction.Left))
            return;

        if (RNG.Chance(99) && matrix.Swap(Position, Position + Direction.Right))
            return;

        Settled = true;
    }
}