using Raylib_cs;

namespace SharpSand;

class Rust : Powder {
    public Rust(Vector2i position) : base(position) {
        Friction = 0.8f;
        CorrosionResistance = 0.6f;
        BaseColor = new Color(180, 65, 15, 255);
        ModifyColor();
    }
}
