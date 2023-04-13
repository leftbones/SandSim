using System.Numerics;
using System.Linq;
using Raylib_cs;

namespace SharpSand;

class Water : Liquid {
    public Water(Vector2 position) : base(position) {
        DispersionRate = 5;
        HeatLimit = 100.0f;
        CoolLimit = -100.0f;
        ColorOffset = 0;
        SetColor(new Color(1, 151, 244, 255));
    }

    public override void Update(Matrix matrix) {
        foreach (Vector2 Dir in Direction.Cardinal) {
            if (matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                ActOnOther(matrix, e);
            }
        }

        base.Update(matrix);
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        other.Settled = false;
        Settled = false;
    }

    public override void HeatReaction(Matrix matrix) {
        matrix.Set(Position, new Steam(Position));
    }

    public override void CoolReaction(Matrix matrix) {
        matrix.Set(Position, new Ice(Position));
    }
}