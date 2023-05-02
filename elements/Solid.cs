using System.Numerics;
using Raylib_cs;

namespace SharpSand;

abstract class Solid : Element {
    public Solid(Vector2i position) : base(position) {
        Health = 50.0f;
        CorrosionResistance = 0.25f;
        Type = ElementType.Solid;
    }

    public override void Step(Matrix matrix) {
        if (!Inactive)
            ShouldDeactivate(matrix);
    }
}