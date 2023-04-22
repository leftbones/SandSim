using System.Numerics;
using Raylib_cs;

namespace SharpSand;

abstract class Solid : Element {
    public Solid(Vector2i position) : base(position) {
        Type = ElementType.Solid;
        Settled = true;
    }

    public override void Step(Matrix matrix) { }
}