using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Plant : Solid {
    public int SpreadChance = 10;

    public Plant(Vector2 position) : base(position) {
        SetColor(new Color(7, 197, 102, 255));
    }

    public override void Update(Matrix matrix) {
        foreach (Vector2 Dir in Direction.Cardinal) {
            if (RNG.Chance(10) && matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                ActOnOther(matrix, e);
            }
        }
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        if (other is Water) {
            matrix.Set(other.Position, new Plant(other.Position));
            other.Settled = false;
            Settled = false;
        }
    }

    public override void HeatReaction(Matrix matrix) {
        matrix.Set(Position, new Fire(Position));

        if (matrix.IsEmpty(Position + Direction.Up))
            matrix.Set(Position + Direction.Up, new Smoke(Position + Direction.Up));

        if (RNG.CoinFlip() && matrix.IsEmpty(Position + Direction.Down))
            matrix.Set(Position + Direction.Up, new Ember(Position + Direction.Down));
    }
}