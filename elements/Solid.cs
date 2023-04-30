using System.Numerics;
using Raylib_cs;

namespace SharpSand;

abstract class Solid : Element {
    public Solid(Vector2i position) : base(position) {
        Health = 50.0f;
        Type = ElementType.Solid;
        Settled = true;
    }

    public override void Step(Matrix matrix) { }
}