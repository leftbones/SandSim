using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Salt : Powder {
    public Salt(Vector2i position) : base(position) {
        Dissolvable = 10;
        Friction = 0.2f;
        BaseColor = new Color(215, 215, 215, 255);
        ModifyColor();
    }

    public override void Dissolve(Matrix matrix) {
        matrix.Set(Position, new Saltwater(Position));
    }
}