using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Dirt : Powder {
    public Dirt(Vector2 position) : base(position) {
        Friction = 0.6f;
        BaseColor = new Color(150, 75, 0, 255);
        ModifyColor();
    }
}