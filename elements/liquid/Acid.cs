using Raylib_cs;

namespace SharpSand;

class Acid : Liquid {
    private int CorrodeCount = 0;
    private int CorrodeLimit = 10;

    public Acid(Vector2i position) : base(position) {
        Spread = 10.0f;
        ColorOffset = 0;
        BaseColor = new Color(0, 255, 0, 255);
        ModifyColor();
    }

    public override void Step(Matrix matrix) {
        // Die after hitting the corrode limit
        if (CorrodeCount == CorrodeLimit)
            Expire(matrix);

        base.Step(matrix);
    }

    public override void ActOnOther(Matrix matrix, Element other) {
        if (CorrodeCount < CorrodeLimit && other.GetType() != typeof(Acid) && RNG.Roll(1.0f - other.CorrosionResistance)) {
            CorrodeCount++;

            if (RNG.CoinFlip())
                matrix.Set(other.Position, new Smoke(other.Position));
            else
                matrix.Set(other.Position, new Air(other.Position));
        }
    }

    public override void Expire(Matrix matrix) {
        if (RNG.CoinFlip())
            matrix.Set(Position, new Smoke(Position));
        else
            matrix.Set(Position, new Air(Position));
    }
}