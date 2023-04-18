using System.Numerics;
using Raylib_cs;

namespace SharpSand;

abstract class Solid : Element {
    public Solid(Vector2 position) : base(position) {
        Type = ElementType.Solid;
    }

    public override void Step(Matrix matrix) { }
}