using Raylib_cs;

namespace SharpSand;

class Wick : Solid {
    public Wick(Vector2i position) : base(position) {
        Health = 10;
        Flammability = 0.1f;
        ForceAct = true;
        ColorOffset = 5;
        BaseColor = new Color(48, 17, 17, 255);
        ModifyColor();
    }

    public override void HeatReaction(Matrix matrix) {
        OnFire = true;
    }

    public override void Expire(Matrix matrix) {
        if (OnFire) {
            // One last attempt to spread fire
            ActOnNeighbors(matrix);

            // Create short-lived fire
            var Fire = new Fire(Position);
            Fire.Lifetime /= 3;
            matrix.Set(Position, Fire);

            // Create smoke if space above is empty
            if (matrix.IsEmpty(Position + Direction.Up))
                matrix.Set(Position + Direction.Up, new Smoke(Position + Direction.Up));
        } else {
            matrix.Set(Position, new Air(Position));
        }
    }
}