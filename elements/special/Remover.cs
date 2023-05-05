using Raylib_cs;

namespace SharpSand;

class Remover : Solid {
    public Remover(Vector2i position) : base(position) {
        Health = 99999;
        CorrosionResistance = 1.0f;
        DrawWhenSelected = true;
        ColorOffset = 0;
        BaseColor = (position.X + position.Y) % 2 == 0 ? new Color(100, 0, 0, 255) : new Color(150, 0, 0, 255);
        ModifyColor();
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        if (other.GetType() != typeof(Remover))
            matrix.Set(other.Position, new Air(other.Position));
    }
}