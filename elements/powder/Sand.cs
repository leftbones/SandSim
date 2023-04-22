using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Sand : Powder {
    public Sand(Vector2i position) : base(position) {
        Friction = 0.1f;
        BaseColor = new Color(255, 206, 92, 255);
        ModifyColor();
    }
}