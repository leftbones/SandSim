using System.Numerics;
using Raylib_cs;

namespace SharpSand;

abstract class Liquid : Element {
    public Liquid(Vector2 position) : base(position) {
        Type = ElementType.Liquid;
    }

    public override void Update(Matrix matrix) { }
}