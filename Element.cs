using System;
using System.Reflection;
using System.Numerics;
using Raylib_cs;

namespace SharpSand;

enum ElementType { None, Solid, Liquid, Gas, Powder }

abstract class Element {
    public ElementType Type { get; set; }

    public Vector2i Position { get; set; }
    public Vector2i LastPosition { get; set; }
    public Vector2i LastDirection { get; set; }

    ////
    // Tracking
    public int Lifetime { get; set; } = -1;                 // How long the element will live (in ticks), -1 is no limit
    public int TicksLived { get; set; } = 0;                // How many ticks the element has already lived
    public float Health { get; set; } = 1;                  // Current health of the element, hitting 0 triggers the Expire method (Defaults: Solid - 50, Liquid/Powder - 10, Gas - 1)
    public bool AlreadyStepped { get; set; } = false;       // If the element has already run it's step method once in the current update loop
    public bool Inactive { get; set; } = false;             // If the element is able to step and act (on others)
    public int TicksElectrified { get; set; } = 0;          // How many ticks the element has been electrified
    public int ElectrifiedCooldown { get; set; } = 0;       // How many ticks since the element was electrified

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

    public float Flammability { get; set; } = 0.0f;         // How likely the element is to be set on fire
    public float BurnDamageModifier { get; set; } = 1.0f;   // How susceptible the element is to damage from fire (doesn't affect flammability)

    ////
    // Interaction
    public float CorrosionResistance { get; set; } = 0.0f;                              // How resistant the element is to corrosion (rust/acid/etc)
    public int Dissolvable { get; set; } = 0;                                      // If the element will dissolve in water
    public int ConductElectricity { get; set; } = 0;                               // If the element conducts electricity
    public int ConductHeat { get; set; } = 0;                                      // If the element conducts heat
    public List<Vector2i> ActDirections { get; set; } = Direction.ShuffledCardinal;     // The directions in which this element interacts with neighboring elements
    public bool ForceAct { get; set; } = false;                                         // If the element should act on its neighbors (this step) regardless of if it is settled or not

    ////
    // Status
    public bool OnFire { get; set; } = false;                       // If the element is currently on fire or not
    public bool Electrified { get; set; } = false;                  // If the element is currently conducting electricity
    public bool Shimmer { get; set; } = false;                      // If the element is currently shimmering

    ////
    // Coloring
    public Color Color { get; set; } = Color.WHITE;                 // Current color of the element (BaseColor +/- ColorOffset)
    public Color BaseColor { get; set; } = Color.MAGENTA;            // Base color of the element before any offset is applied
    public int ColorOffset { get; set; } = 25;                      // How far the BaseColor should be offset
    public bool NoFireColor { get; set; } = false;                  // If the element should not have it's color affected by the 'OnFire' flag

    ////
    // Other
    public bool DrawWhenSelected { get; set; } = false;         // If true, only draw the element when it is the currently selected element


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
            Inactive = false;
            Settled = false;
            Health -= 1.0f * BurnDamageModifier;
            if (!NoFireColor) ModifyColor();
        }

        if (ElectrifiedCooldown > 0)
            ElectrifiedCooldown--;

        if (Electrified) {
            Inactive = false;
            Settled = false;
            ModifyColor();

            TicksElectrified++;
            if (TicksElectrified == 20) {
                Electrified = false;
                TicksElectrified = 0;
                ElectrifiedCooldown = 20;
                ModifyColor();
            }
        }

        // Act on neighbors if not inactive, not settled (Liquid/Powder), is Solid, or ForceAct is set
        if (!Inactive && (!Settled || Type == ElementType.Solid) || ForceAct) {
            matrix.WakeChunk(this);
            ActOnNeighbors(matrix);
        }
    }

    // Check surroundings and move accordingly
    public abstract void Step(Matrix matrix);

    // Interact with neighboring elements (called by ActOnNeighbors)
    public virtual void ActOnOther(Matrix matrix, Element other) { }

    // Attempt to unsettle a neighboring element (called by ActOnNeighbors)
    public virtual void UnsettleOther(Element other) {
        // If this element is not a solid, the other element is a liquid, or the other element is a powder and fails a friction roll for the other element, unsettle it
        if (Type != ElementType.Solid && (other.Type == ElementType.Liquid || (other.Type == ElementType.Powder && !RNG.Roll(other.Friction))))
            other.Settled = false;
    }

    // Call ActOnOther and UnsettleOther for all neighbors in the directions set on ActDirections, also re-activate all inactive neighbors
    public virtual void ActOnNeighbors(Matrix matrix) {
        Type this_type = this.GetType();
        foreach (Vector2i Dir in ActDirections) {
            if (matrix.InBounds(Position + Dir)) {
                Element e = matrix.Get(Position + Dir);
                Type e_type = e.GetType();

                if (e_type == typeof(Air))
                    continue;

                UnsettleOther(e);
                ActOnOther(matrix, e);
                e.Inactive = false;

                // Electricity
                if (Electrified) {
                    // Spread to conductive neighbors
                    if (!e.Electrified && e.ElectrifiedCooldown == 0 && RNG.Roll(e.ConductElectricity))
                        e.Electrified = true;

                    // Ignite flammable neighbors
                    if (RNG.Roll(e.Flammability))
                        e.OnFire = true;
                }

                // Temperature transference
                if (HeatFactor > 0.0 || OnFire) {
                    // Attempt to spread fire
                    if (OnFire && RNG.Roll(e.Flammability))
                        e.OnFire = true;

                    float heat_power = HeatFactor * (RNG.Range(0, 15) * 1.0f);
                    heat_power = OnFire ? Math.Max(heat_power, 1.0f) : heat_power;
                    if (e_type == this_type) heat_power /= 2;
                    e.ChangeTemp(matrix, heat_power);
                } else if (CoolFactor > 0.0) {
                    float cool_power = -CoolFactor * (RNG.Range(0, 15) * 0.1f);
                    if (e_type == this_type) cool_power /= 2;
                    e.ChangeTemp(matrix, cool_power);
                }
            }
        }
    }

    // Check if the element is surrounded by neighbors of its same type, and become inactive if true
    public void ShouldDeactivate(Matrix matrix) {
        Type this_type = this.GetType();
        var Neighbors = matrix.GetNeighbors(Position);
        if (Neighbors.All(e => e.GetType() == this_type))
            Inactive = true;
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

    // Triggered when the element is dissolved
    public virtual void Dissolve(Matrix matrix) {
        matrix.Set(Position, new Air(Position));
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

        if (Electrified) {
            Color = new Color(255, 255, 0, 255);
            return;
        }

        if (Shimmer) {
            Color = Effect.LightenColor(BaseColor, 100);
            return;
        }

        Color = Utility.OffsetColor(BaseColor, ColorOffset);
    }
}