using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Plant : Solid {
    public int SpreadChance = 10;

    public Plant(Vector2 position) : base(position) {
        FireResistance = 20;
        FireDamage = 30;
        BaseColor = new Color(7, 197, 102, 255);
        ModifyColor();
    }

    public override void Update(Matrix matrix) {
        foreach (Vector2 Dir in Direction.Cardinal) {
            if (RNG.Chance(10) && matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                ActOnOther(matrix, e);
            }
        }
    }

    public override bool ActOnOther(Matrix matrix, Element other) {
        if (other is Water) {
            if (OnFire)
                OnFire = false;
            else
                matrix.Set(other.Position, new Plant(other.Position));

            other.Settled = false;
            Settled = false;
            return true;
        }
        return false;
    }

    public override void ApplyHeating(Matrix matrix) {
        if (!OnFire)
            OnFire = true;
    }

    public override void Expire(Matrix matrix) {
        if (RNG.Chance(2))
            matrix.Set(Position, new Ember(Position));
        else {
            matrix.Set(Position, new Fire(Position));
            foreach (Vector2 Dir in Direction.Full) {
                if (matrix.InBounds(Position + Dir)) {
                    Element e = matrix.Get(Position + Dir);
                    if (e is not Air && RNG.Chance(e.GetIgniteChance())) {
                        e.ApplyHeating(matrix);
                    }
                }
            }
        }

        if (matrix.IsEmpty(Position + Direction.Up))
            matrix.Set(Position + Direction.Up, new Smoke(Position + Direction.Up));
    }
}