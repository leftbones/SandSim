using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Ice : Solid {
    public Ice(Vector2 position) : base(position) {
        ColorOffset = 15;
        BaseColor = new Color(165, 242, 243, 255);
        ModifyColor();
    }

    public override void Update(Matrix matrix) {
        // Cool neighbors
        foreach (Vector2 Dir in Direction.Cardinal) {
            if (matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                ActOnOther(matrix, e);
            }
        }
    }

    public override bool ActOnOther(Matrix matrix, Element other) {
        if (other is not Air) {
            other.ApplyCooling(matrix);
            other.Settled = false;
            Settled = false;
            return true;
        }
        return false;
    }

    public override void ApplyHeating(Matrix matrix) {
        matrix.Set(Position, new Water(Position));
    }
}