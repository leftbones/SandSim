using System.Numerics;
using Raylib_cs;

namespace SharpSand;

enum ElementType { Solid, Liquid, Gas, Powder }

abstract class Element {
    public ElementType Type { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 LastPosition { get; set; }
    public bool Settled { get; set; } = false;
    public bool Asleep { get; set; } = false;
    public int TicksLived { get; set; } = 0;
    public int Lifetime { get; set; } = -1;
    public int Health { get; set; } = 500;
    public int Friction { get; set; } = 0;
    public int DispersionRate { get; set; } = 1;
    // public bool CanBeHeated { get; set; } = false;
    // public bool CanBeCooled { get; set; } = false;
    public bool OnFire { get; set; } = false;
    public int FireDamage { get; set; } = 3;
    public int FireResistance { get; set; } = 100;
    public int ColorOffset { get; set; } = 25;
    public Color Color { get; set; } = Color.WHITE;
    public Color BaseColor = Color.MAGENTA;


    public Element(Vector2 position) {
        Position = position;
        LastPosition = position;
    }


    // Increment the element's tick age and apply any effects (like fire damage)
    public void Tick(Matrix matrix) {
        TicksLived++;
        if (TicksLived == Lifetime || Health <= 0)
            Expire(matrix);

        if (OnFire) {
            Health -= FireDamage;
            ModifyColor();
        }
    }

    public abstract void Update(Matrix matrix);

    public virtual bool ActOnOther(Matrix matrix, Element other) { return false; }

    public virtual void ApplyHeating(Matrix matrix) { }
    public virtual void ApplyCooling(Matrix matrix) { }

    public virtual void Expire(Matrix matrix) { }

    public int GetIgniteChance() {
        return 100 - Math.Clamp(((FireResistance + RNG.Range(-FireResistance / 4, FireResistance / 4)) + RNG.Range(-5, 5)), 0, 100);
    }

    public void ModifyColor() {
        if (OnFire) {
            Color = Effect.GetFireColor();
            return;
        }

        Color = Utility.OffsetColor(BaseColor, ColorOffset);
    }
}