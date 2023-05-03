using Raylib_cs;

namespace SharpSand;

class Spawner : Solid {
    private Element ElementContained;

    public Spawner(Vector2i position) : base(position) {
        ColorOffset = 0;
        BaseColor = new Color(255, 0, 255, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        if (ElementContained is null)
            return; 

        foreach (Vector2i Dir in Direction.ShuffledFull) {

        }
    }
}