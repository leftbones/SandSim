using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Stone : Solid {
    public Stone(Vector2i position) : base(position) {
        ColorOffset = 15;
        BaseColor = new Color(127, 128, 118, 255);
        ModifyColor();
    }
}