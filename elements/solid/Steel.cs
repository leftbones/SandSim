using Raylib_cs;

namespace SharpSand;

////
// Notes
// - Corrosion resistance should lower when heated (IRL: Hydrochloric acid will dissolve the iron in steel at high temperatures to create hydrogen gas)

class Steel : Solid {
    public Steel(Vector2i position) : base(position) {
        CorrosionResistance = 1.0f;
        ConductElectricity = 825;
        ColorOffset = 5;
        BaseColor = new Color(155, 175, 185, 255);
        ModifyColor();
    }
}