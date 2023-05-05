using Raylib_cs;

namespace SharpSand;

class Chaos : Solid {
    public Chaos(Vector2i position) : base(position) { }

    public override void Step(Matrix matrix) {
        Type? NewType = System.Type.GetType("SharpSand." + Atlas.Entries.ElementAt(RNG.Range(0, Atlas.Entries.Count - 6)).Key)!;
        var NewElement = (Element)Activator.CreateInstance(NewType, Position)!;
        matrix.Set(Position, NewElement);
    }
}