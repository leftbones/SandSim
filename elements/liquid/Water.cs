using System.Numerics;
using System.Linq;
using Raylib_cs;

namespace SharpSand;

class Water : Liquid {
    public Water(Vector2 position) : base(position) {
        DispersionRate = 5;
        ColorOffset = 0;
        BaseColor = new Color(1, 151, 244, 255);
        ModifyColor();
    }

    public override void Update(Matrix matrix) {
        // foreach (Vector2 Dir in Direction.Cardinal) {
        //     if (matrix.InBounds(Position + Dir)) {
        //         Element e = matrix.Get(Position + Dir);
        //         ActOnOther(matrix, e);
        //     }
        // }

        base.Update(matrix);
    }

    public override void ApplyHeating(Matrix matrix) {
        matrix.Set(Position, new Steam(Position));
    }

    public override void ApplyCooling(Matrix matrix) {
        matrix.Set(Position, new Ice(Position));
    }
}