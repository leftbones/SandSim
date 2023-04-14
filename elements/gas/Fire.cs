using System.Numerics; using Raylib_cs;

namespace SharpSand;

////
// TODO
// - Fix back and forth "jittering" (very obvious when burning Plant)

class Fire : Gas {
    public Fire(Vector2 position) : base(position) {
        Lifetime = 30;
        BaseColor = Effect.GetFireColor();
        Color = BaseColor;
    }

    public override void Update(Matrix matrix) {
        // 50% chance to extend the lifetime
        if (RNG.CoinFlip())
            Lifetime++;

        // Heat neighbors
        foreach (Vector2 Dir in Direction.Full) {
            if (RNG.Chance(33) && matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                ActOnOther(matrix, e);
            }
        }

        // Move upward, higher chance to move directly up
        List<Vector2> Directions = Direction.Upward.OrderBy(a => RNG.Random.Next()).ToList();
        foreach (Vector2 Dir in Directions) {
            int Chance = 1;
            if (Dir == Direction.Up)
                Chance = 20;

            if (RNG.Chance(Chance) && matrix.SwapIfEmpty(Position, Position + Dir))
                return;
        }

        // 50% chance to try to move up
        if (RNG.CoinFlip() && matrix.SwapIfEmpty(Position, Position + Direction.Up))
            return;

        // Move either left or right if it can't move up
        if (RNG.CoinFlip()) {
            if (matrix.SwapIfEmpty(Position, Position + Direction.Left))
                return;

            if (matrix.SwapIfEmpty(Position, Position + Direction.Right))
                return;
        } else {
            if (matrix.SwapIfEmpty(Position, Position + Direction.Right))
                return;

            if (matrix.SwapIfEmpty(Position, Position + Direction.Left))
                return;
        }
    }

    public override bool ActOnOther(Matrix matrix, Element other) {
        if (other is not Air && RNG.Chance(other.GetIgniteChance())) {
            other.ApplyHeating(matrix);

            if (other is Water)
                Expire(matrix);

            other.Settled = false;
            Settled = false;
            return true;
        }
        return false;
    }

    public override void Expire(Matrix matrix) {
        // Last ditch effor to spread to nearby flammable elements before burning out
        foreach (Vector2 Dir in Direction.Full) {
            if (matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                if (e is not Air && RNG.Chance(Math.Min(e.GetIgniteChance(), 0)))
                    e.ApplyHeating(matrix);
            }
        }
        matrix.Set(Position, new Air(Position));
    }
}