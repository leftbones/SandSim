# SharpSand Notes

## Eventually List
* Gasses are currently unable to become settled because there are too many factors that determine if they should unsettle, this might not be a problem at all
* Work out a way to transfer temperature between elements
  * HEAT SOURCE elements give off heat without losing heat (Fire applies heat to Plant)
  * HEATED elements transfer their heat to others (Iron heated by Fire transfers heat to Water)
  * Same applies to cooling (Water touching Iron heated by Fire cools the Iron)
* The RNG methods I'm using are all over the place, I should probably commit to just using RNG.Roll and remove all instances of RNG.Chance (RNG.Odds is okay for specific odds)
* Might be a good idea to replace all the instances where I create a new Vector2 list for movement directions with a List<Vector2> property on element that I can just overwrite, to save from having to create new lists every step

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