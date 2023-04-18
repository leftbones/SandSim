using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Soot : Powder {
    public Soot(Vector2 position) : base(position) {
        Density = 0.1f;
        BaseColor = new Color(93, 92, 94, 255);
        ModifyColor();
    }
}