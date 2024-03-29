using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Nanobots : Powder {
    private bool Dormant = false;
    private float Chance = 0.005f;

    public Nanobots(Vector2i position) : base(position) {
        Friction = 0.05f;
        ForceAct = true;
        BaseColor = Effect.GetNanobotsColor();
        Color = BaseColor;
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        if (RNG.Roll(Chance))
            Dormant = true;

        if (!Dormant && RNG.Chance(2)) {
            Color = Effect.GetNanobotsColor();
        }

        base.Step(matrix);
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        if (!Dormant && RNG.Roll(Chance) && other.GetType() != typeof(Nanobots)) {
            matrix.Set(other.Position, new Nanobots(other.Position));
        }
    }
}