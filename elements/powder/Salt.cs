using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Salt : Powder {
    public Salt(Vector2i position) : base(position) {
        Friction = 0.2f;
        BaseColor = new Color(215, 215, 215, 255);
        ModifyColor();
    }
}