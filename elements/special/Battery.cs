using Raylib_cs;

namespace SharpSand;

class Battery : Solid {
    public Battery(Vector2i position) : base(position) {
        ColorOffset = 0;
        BaseColor = (position.X + position.Y) % 2 == 0 ? new Color(100, 100, 100, 255) : new Color(255, 255, 0, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        var Neighbors = matrix.GetNeighbors(Position);
        foreach (Element Neighbor in Neighbors) {
            if (!Neighbor.Electrified && Neighbor.ElectrifiedCooldown == 0 && RNG.Roll(Neighbor.ConductElectricity))
                Neighbor.Electrified = true;
        }
        base.Step(matrix);
    }
}