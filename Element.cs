using System;
using System.Reflection;
using System.Numerics;
using Raylib_cs;

namespace SharpSand;

enum ElementType { Solid, Liquid, Gas, Powder }

abstract class Element {
    public ElementType Type { get; set; }

    public Vector2i Position { get; set; }
    public Vector2i LastPosition { get; set; }
    public Vector2i LastDirection { get; set; }

    ////
    // Tracking
    public int Lifetime { get; set; } = -1;                 // How long the element will live (in ticks), -1 is no limit
    public int TicksLived { get; set; } = 0;                // How many ticks the element has already lived
    public float Health { get; set; } = 500;                // Current health of the element, hitting 0 triggers the Expire method
    public bool AlreadyStepped { get; set; } = false;       // If the element has already run it's step method once in the current update loop

    ////
    // Moveability
    public bool Settled { get; set; } = false;              // If the element has reached a stable position, has to become unsettled to move again
    public float Friction { get; set; } = 1.0f;             // Affects how likely an element is to become settled (Powder)
    public float Spread { get; set; } = 0.0f;               // Affects how quickly an element will disperse horizontally (Liquid/Gas)
    public float Drift { get; set; } = 0.0f;                // Affects how much an element drifts horizontally while falling (Powder)
    public float Density { get; set; } = 0.0f;              // Affects how liquids/gases move through eachother (Liquid/Gas) or how quickly powders dissolve in liquid (Powder)

    ////
    // Temperature
    public float IdleTemp { get; set; } = 50.0f;            // The resting temperature of the element
    public float ActiveTemp { get; set; } = 50.0f;          // The current temperature of the element
    public float HeatFactor { get; set; } = 0.0f;           // How much heating the element gives off
    public float CoolFactor { get; set; } = 0.0f;           // How much cooling the element gives off

    public bool Flammable { get; set; } = false;            // If the element can be set on fire
    public float BurnDamageModifier { get; set; } = 1.0f;   // How resistant the element is to damage from fire (doesn't affect flammability)

    ////
    // Interaction
    public bool ForceAct { get; set; } = false;                             // If the element should act on its neighbors (this step) regardless of if it is settled or not
    public List<Vector2i> ActDirections { get; set; } = Direction.Full;      // The directions in which this element interacts with neighboring elements

    ////
    // Status
    public bool OnFire { get; set; } = false;                       // If the element is currently on fire or not
    public bool Shimmer { get; set; } = false;                      // If the element is currently shimmering

    ////
    // Coloring
    public Color Color { get; set; } = Color.WHITE;                 // Current color of the element (BaseColor +/- ColorOffset)
    public Color BaseColor { get; set; }= Color.MAGENTA;            // Base color of the element before any offset is applied
    public int ColorOffset { get; set; } = 25;                      // How far the BaseColor should be offset


    public Element(Vector2i position) {
        Position = position;
        LastPosition = position;
    }


    // Increment the element's tick age, apply any time-based effects, update position tracking
    public void Tick(Matrix matrix) {
        TicksLived++;
        if (TicksLived == Lifetime || Health <= 0)
            Expire(matrix);

        if (OnFire) {
            Health -= 1.0f * BurnDamageModifier;
            ModifyColor();
        }

        if ((!Settled && Type != ElementType.Solid ) || ForceAct) {
            matrix.WakeChunk(this);
            ActOnNeighbors(matrix);
        }

        if (Position == LastPosition && Type != ElementType.Gas) // TODO: Revisit if this gas exclusion is still necessary
            Settled = true;
    }

    // Check surroundings and move accordingly
    public abstract void Step(Matrix matrix);

    // Interact with neighboring elements (called by ActOnNeighbors)
    public virtual void ActOnOther(Matrix matrix, Element other) { }

    // Attempt to unsettle a neighboring element (called by ActOnNeighbors)
    public virtual void UnsettleOther(Element other) {
        if (RNG.Roll(Friction)) {
            Settled = false;
            other.Settled = false;
        }
    }

    // Call ActOnOther and UnsettleOther for all neighbors in the directions set on ActDirections
    // [Note] This used to check if the element was air before acting on it, but that actually hurt performance instead of helping it
    public virtual void ActOnNeighbors(Matrix matrix) {
        foreach (Vector2i Dir in ActDirections) {
            if (matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);

                UnsettleOther(e);
                ActOnOther(matrix, e);

                // Temperature transference
                if (HeatFactor > 0.0 || OnFire) {
                    if (OnFire && e.Flammable && RNG.Chance(1)) e.OnFire = true;
                    float heat_power = HeatFactor * (RNG.Range(0, 15) * 1.0f);
                    heat_power = OnFire ? Math.Max(heat_power, 1.0f) : heat_power;
                    e.ChangeTemp(matrix, heat_power);
                } else if (CoolFactor > 0.0) {
                    float cool_power = -CoolFactor * (RNG.Range(0, 15) * 0.1f);
                    e.ChangeTemp(matrix, cool_power);
                }

                // // Receive temp changes
                // if (e.HeatFactor > 0.0) {
                //     if (e.OnFire && Flammable && RNG.Chance(1)) OnFire = true;
                //     float amount = e.OnFire ? Math.Max(HeatFactor, 1.0f) : HeatFactor;
                //     ChangeTemp(matrix, amount);
                // } else if (e.CoolFactor > 0.0)
                //     ChangeTemp(matrix, -e.CoolFactor);
            }
        }
    }

    // Alter the elements active temperature
    public virtual void ChangeTemp(Matrix matrix, float amount) {
        ActiveTemp += amount;

        if (ActiveTemp > IdleTemp) ActiveTemp -= 0.5f;
        if (ActiveTemp < IdleTemp) ActiveTemp += 0.5f;

        ActiveTemp = Math.Clamp(ActiveTemp, 0.0f, 100.0f);

        if (ActiveTemp == 100.0f)
            HeatReaction(matrix);
        else if (ActiveTemp == 0.0f)
            CoolReaction(matrix);
    }

    // Called when temperature reaches 100.0
    public virtual void HeatReaction(Matrix matrix) { }

    // Called when temperature reaches 0.0
    public virtual void CoolReaction(Matrix matrix) { }

    // Triggered when TicksLived reaches Lifetime or Health reaches 0
    public virtual void Expire(Matrix matrix) {
        matrix.Set(Position, new Air(Position));
    }

    // Replace this element with a new element
    public virtual void ReplaceWith(Matrix matrix, Element element) {
        matrix.Set(Position, element);
    }

    // Remove OnFire status and return the element to it's base color
    public virtual void ExtinguishFire() {
        OnFire = false;
        ModifyColor();
    }

    // Modify the element's color based on any effects, or apply the color offset if no effects are active
    public void ModifyColor() {
        if (OnFire) {
            Color = Effect.GetFireColor();
            return;
        }

        Color = Utility.OffsetColor(BaseColor, ColorOffset);
    }
}