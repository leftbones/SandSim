using Raylib_cs;

namespace SharpSand;

class Acid : Liquid {
    public Acid(Vector2i position) : base(position) {
        Spread = 10.0f;
        ColorOffset = 0;
        BaseColor = new Color(0, 255, 0, 255);
        ModifyColor();
    }
}