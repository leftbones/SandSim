# TODO

## Urgent
- Clean up the file structure! Holy hell, it's awful.
- Have elements store their string name to avoid having to call `GetType()` and `typeof()` all over the place (probably a massive drain on performance)
- Refactor the base `Step` method for gases, it's messy and could probably be optimized much better.

## Less Urgent
- Rather than calling "Utility.OffsetColor" every time an offset color is needed, it could be called by the Atlas during the setup a few times and stored for each element, that way each element could have a number of preset offset colors to pick from.
- Refactor the temperature system (again) for more realistic temperature transference. Example: Water is warmer than Ice, so while Ice cools water, Water should also heat Ice slightly. Then I need to find a good balance so that a little water would be frozen, but a lot of water might melt the ice.

## Eventually
- Implement Electricity and Electricity Conductivity!
- Implement Heat Conductivity after refactoring the temperature system.
- Refactor the base `Step` method for liquids with density in mind, it works fine but I'm sure it could be better.

## Maybe
- Right now, `Special` type elements can be of any type, but I'm finding that they're mostly solids. Should special be made into it's own type?


# Known Bugs
* The drawing tools and brush indicator don't line up properly if the scale is set to anything other than 4
* The brush indicator sometimes fails to align to the grid properly and is off by half a cell or so, first noticed after changing from Vector2 to Vector2i
* Liquid settling is still kind of buggy, good example is lava dropped onto a pool of acid just settles on top of the acid instead of being corroded


# Simplified Element Properties
- Temperature (Default internal temperature of the element when created)
- Hardness (Replaces CorrosionResistance, could maybe be used for explosion resistance?)
- Diffusion (How much horizontal "wiggle" the element will have while moving)
- Weight (Replaces Density, just a much better name)
- Gravity (How quickly the element moves up or down, positive or negative, solids would have 0)
- Flammable (0 is inflammable, anything higher increases flammability)
- Dissolvable (0 will not dissolve, anything higher increases rate of dissolving)
- Explosive (Boolean, if the element should explode when set on fire)
- HeatConductivity (0 is none, anything higher increases conductivity)
- ElectricConductivity (0 is none, anything higher increases conductivity)

Note: Properties like `Flammable` and `HeatConductivity` will probably be checked with `RNG.Roll()`, which takes a float, but maybe I should set the properties to be integers for easier understanding and comparison to other elements. Example: Wood's `Flammable` is set to 5, but when rolled, could be multiplied by 0.001 to become 0.005. Just a thought, because 5 is easier to look at and remember than 0.005 is.


# New Elements
- Solid
  * Wire - If connected with two ends of Copper, electricity can safely pass from one to the other
  * Vine - Grows randomly when painted, unlike Plant which requires Water to grow

- Liquid
  * Gas - Flammable, burns for a long time, creates mini-explosions when burning out
  * Mud - Made when Dirt contacts Water, dries into Dirt
  * Molten Plastic - Created when Plastic is heated, becomes Plastic when cooled

- Gas
  * Hydrogen - Extremely flammable, creates large explosions
  * Blue Fire - Behaves exactly like Fire, but applies cooling instead of heating

- Powder
  * Dust - Lightweight powder created by various reactions, mildly flammable and burns extremely fast
  * Concrete Powder - Becomes Concrete when mixed with Water

- Special
  * Separator - Elements that touch it can be split into their component elements, if possible (Example: Water -> Hydrogen + Air)
  * Converter - Converts any element that touches it into the first element applied to it
  * Katamari - Absorbs any element that touches it and grows larger


# New Objects
* Switch - Clickable toggle switch, when activated, it will allow electricity to pass through until deactivated
* Button - Clickable momentary switch, when activated, it will allow electricity to pass through for a few ticks
* Bouncy Ball - A simple ball that bounces on solids and floats on liquids


# Weird Things
This code made the brush work like an actual paint brush for some reason (see: https://streamable.com/soqdsg)

public void PaintLine(Matrix matrix, Vector2i a, Vector2i b, string element_name) {
    float Density = BrushDensity;

    bool Force = false;
    if (PaintOver || element_name == "SharpSand.Air")
        Force = true;

    List<Vector2i> Points = Utility.GetLinePoints(a, b);
    List<Vector2i> Cache = new List<Vector2i>();

    Type t = Type.GetType(element_name)!;

    foreach (Vector2i Point in Points) {
        if (Cache.Contains(Point))
            continue;

        if (Force || (RNG.Roll(Density) && matrix.IsEmpty(Point))) {
            var NewElement = (Element)Activator.CreateInstance(t, Point)!;
            matrix.Set(Point, NewElement);
            Cache.Add(Point);
        }
    }
}