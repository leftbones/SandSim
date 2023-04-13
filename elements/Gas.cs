using System.Numerics;
using Raylib_cs;

namespace SharpSand;

abstract class Gas : Element {
    public Gas(Vector2 position) : base(position) {
        Type = ElementType.Gas;
    }

    public override void Update(Matrix matrix) { }
}

class Air : Gas {
    public Air(Vector2 position) : base(position) {
        Color = Color.BLACK;
    }

    public override void Update(Matrix matrix) { }
}