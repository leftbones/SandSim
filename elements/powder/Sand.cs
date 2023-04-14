using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Sand : Powder {
    public Sand(Vector2 position) : base(position) {
        Friction = 2;
        BaseColor = new Color(255, 206, 92, 255);
        ModifyColor();
    }
}