using System.Numerics; using Raylib_cs;

namespace SharpSand;

////
// TODO
// - Fix back and forth "jittering" (very obvious when burning Plant)

class Fire : Gas {
    public Fire(Vector2 position) : base(position) {
        Lifetime = 50;
        CanBeHeated = false;
        HeatingFactor = 50.0f;
        ColorOffset = 100;
        SetColor(new Color(255, 117, 56, 255));
    }

    public override void Update(Matrix matrix) {
        // 50% chance to extend the lifetime
        if (RNG.CoinFlip())
            Lifetime++;

        // Heat neighbors
        foreach (Vector2 Dir in Direction.Full) {
            if (RNG.Chance(20) && matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                ActOnOther(matrix, e);
            }
        }

        // Move upward, higher chance to move directly up
        List<Vector2> Directions = Direction.Upward.OrderBy(a => RNG.Random.Next()).ToList();
        foreach (Vector2 Dir in Directions) {
            int Chance = 5;
            if (Dir == Direction.Up)
                Chance = 25;

            if (RNG.Chance(Chance) && matrix.SwapIfEmpty(Position, Position + Dir))
                return;
        }

        // 25% chance to try to move up
        if (RNG.Chance(25) && matrix.SwapIfEmpty(Position, Position + Direction.Up))
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

    public override void ActOnOther(Matrix matrix, Element other) {
        if (other is not Air && other.CanBeHeated) {
            other.ApplyHeating(matrix, HeatingFactor);
            other.Settled = false;
            Settled = false;
            LifetimeExpire(matrix);
        }
    }

    public override void LifetimeExpire(Matrix matrix) {
        matrix.Set(Position, new Air(Position));
    }
}