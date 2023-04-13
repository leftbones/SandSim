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
    public int Friction { get; set; } = 0;
    public int DispersionRate { get; set; } = 1;
    public float Temperature { get; set; } = 0.0f;
    public bool CanBeHeated { get; set; } = true;
    public bool CanBeCooled { get; set; } = true;
    public float HeatLimit { get; set; } = 0.0f;
    public float CoolLimit { get; set; } = 0.0f;
    public float HeatingFactor { get; set; } = 0;
    public float CoolingFactor { get; set; } = 0;
    public int ColorOffset { get; set; } = 25;
    public Color Color { get; set; } = Color.WHITE;

    public Element(Vector2 position) {
        Position = position;
        LastPosition = position;
    }

    public abstract void Update(Matrix matrix);

    public void Tick(Matrix matrix) {
        TicksLived++;
        if (TicksLived == Lifetime)
            LifetimeExpire(matrix);
    }

    public virtual void HeatReaction(Matrix matrix) { }
    public virtual void CoolReaction(Matrix matrix) { }

    public virtual void ActOnOther(Matrix matrix, Element other) { }

    public virtual void ApplyHeating(Matrix matrix, float amount) {
        Temperature += amount;
        if (Temperature >= HeatLimit)
            HeatReaction(matrix);
    }

    public virtual void ApplyCooling(Matrix matrix, float amount) {
        Temperature -= amount;
        if (Temperature <= CoolLimit)
            CoolReaction(matrix);
    }

    public virtual void LifetimeExpire(Matrix matrix) { }

    // Set and offset color
    public void SetColor(Color color) {
        // Method A: Different offsets
        // int R = Math.Clamp(color.r + RNG.Range(-ColorOffset, ColorOffset), 0, 255);
        // int G = Math.Clamp(color.g + RNG.Range(-ColorOffset, ColorOffset), 0, 255);
        // int B = Math.Clamp(color.b + RNG.Range(-ColorOffset, ColorOffset), 0, 255);

        // Method B: Same offsets
        int Offset = RNG.Range(-ColorOffset, ColorOffset);
        int R = Math.Clamp(color.r + Offset, 0, 255);
        int G = Math.Clamp(color.g + Offset, 0, 255);
        int B = Math.Clamp(color.b + Offset, 0, 255);

        Color = new Color(R, G, B, color.a);
    }
}