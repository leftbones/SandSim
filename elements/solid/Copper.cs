using Raylib_cs;

namespace SharpSand;

class Copper : Solid {
    public Copper(Vector2i position) : base(position) {
        CorrosionResistance = 1.0f;
        ConductElectricity = 750;
        ColorOffset = 5;
        BaseColor = new Color(185, 115, 50, 255);
        ModifyColor();
    }
}