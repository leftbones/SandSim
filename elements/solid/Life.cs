using Raylib_cs;

namespace SharpSand;

class Life : Solid {
    public bool Alive = true;
    private Color LiveColor = new Color(255, 255, 255, 255);
    private Color DeadColor = new Color(25, 25, 25, 255);

    public Life(Vector2i position) : base(position) {
        Flammability = 0.02f;
        BurnDamageModifier = 0.5f;
        ColorOffset = 0;
        BaseColor = new Color(255, 255, 255, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        // Count Neighbors
        var Neighbors = matrix.GetNeighbors(Position).FindAll(n => n.GetType() == typeof(Life));
        int LiveCount = 0;
        int DeadCount = 0;
        foreach (Life L in Neighbors) {
            if (L.Alive) LiveCount++;
            else DeadCount++;
        }

        // Determine Status
        if (Alive) {
            if (LiveCount < 2 || LiveCount > 3)
                Alive = false;
        } else {
            if (LiveCount == 3)
                Alive = true;
        }

        // Update Color
        if (Alive)
            Color = LiveColor;
        else
            Color = DeadColor;
    }
}