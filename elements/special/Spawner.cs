using Raylib_cs;

namespace SharpSand;

class Spawner : Solid {
    private Type? ContainedType = null;
    public float SpawnChance = 0.01f;

    public Spawner(Vector2i position) : base(position) {
        Health = 99999;
        CorrosionResistance = 1.0f;
        DrawWhenSelected = true;
        ColorOffset = 0;
        BaseColor = (position.X + position.Y) % 2 == 0 ? new Color(100, 100, 100, 255) : new Color(150, 150, 150, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        if (ContainedType is null) {
            foreach (Vector2i Dir in Direction.ShuffledFull) {
                if (matrix.InBounds(Position + Dir)) {
                    Element e = matrix.Get(Position + Dir);
                    if (e.GetType() != typeof(Air) && e.GetType() != typeof(Spawner)) {
                        ContainedType = e.GetType();
                    }
                }
            }
        } else {
            foreach (Vector2i Dir in Direction.ShuffledFull) {
                if (RNG.Roll(SpawnChance) && matrix.IsEmpty(Position + Dir)) {
                    var NewElement = (Element)Activator.CreateInstance(ContainedType, Position + Dir)!;
                    matrix.Set(Position + Dir, NewElement);
                }
            }
        }

        base.Step(matrix);
    }
}