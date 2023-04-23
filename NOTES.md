# SharpSand Notes

## Priority List
* Work out a way to allow settled elements to still process and react to temp changes without attempting movement
* Change "Flammable" bool to "Flammability" float modifier to be used when rolling to spread fire between elements
* Clean up directory structure! Holy shit this is a MESS.
* Add some sort of "Game" or "Simulation" class to control Matrix, DrawingTools, Theme, etc. all in one place.

## Eventually List
* Gasses are currently unable to become settled because there are too many factors that determine if they should unsettle, this might not be a problem at all
* Work out a way to transfer temperature between elements
  * HEAT SOURCE elements give off heat without losing heat (Fire applies heat to Plant)
  * HEATED elements transfer their heat to others (Iron heated by Fire transfers heat to Water)
  * Same applies to cooling (Water touching Iron heated by Fire cools the Iron)
* The RNG methods I'm using are all over the place, I should probably commit to just using RNG.Roll and remove all instances of RNG.Chance (RNG.Odds is okay for specific odds)
* Might be a good idea to replace all the instances where I create a new Vector2 list for movement directions with a List<Vector2> property on element that I can just overwrite, to save from having to create new lists every step
* Separate Density into different properties for Liquid and Gas so they can't interact in any way

## Density Explained
"Density" is a scientific term that I'm using in a completely wrong way to determine how liquids and gasses interact with other liquids and gasses.
If a falling liquid meets another liquid with a lower density, it will displace that liquid. The same principle applies to gasses.

Air is the baseline for gasses with a density of zero. Water is the baseline for liquids with a density of 100.

### Liquid Density (Ascending)
* Oil
* Water
* Salt Water
* Lava
* Molten Glass
* Slime

### Gas Density (Ascending)
* Fire
* Methane
* Air
* Steam
* Smoke


## Temperature System
Elements have an ActiveTemp and an IdleTemp (default is 50.0)
* ActiveTemp is the current temperature of the element. When no outside forces are influencing temperature, this will attempt to regulate back to IdleTemp.
* IdleTemp is the base temperature of the element when no other elements are applying heating or cooling to it.

Example: Water comes in contact with Lava

* Water is processed
  * Water has a CoolFactor of 0.5, Lava's temperature is reduced from 50.0 to 49.5
  * Lava has a HeatFactor of 10.0, Water's temperature is increased to 60.0 from 50.0
  * Water attempts to reach IdleTemp, decreases from 60.0 to 59.5
* Lava is processed
  * Lava has a HeatFactor of 10.0, Water's temperature is increased to 69.0 from 59.0
  * Water has a CoolFactor of 0.5, Lava's temperature is reduced from 49.5 to 49.0
  * Lava attempts to reach IdleTemp, increases from 49.0 to 49.5

Water's idle temperature regulation can't compete with Lava's HeatFactor, once Water's ActiveTemp reaches 100.0, Water becomes Steam
If Water were continuously applied to Lava, dropping it's ActiveTemp to 0.0, Lava would become Stone