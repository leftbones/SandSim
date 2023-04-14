using Raylib_cs;

namespace SharpSand;


public static class Effect {
    public static readonly Color[] FireColors = new Color[] {
        new Color(239, 71, 111, 255),
        new Color(255, 117, 56, 255),
        new Color(255, 206, 92, 255)
    };

    public static Color GetFireColor() {
        return FireColors[RNG.Random.Next(0, FireColors.Length)];
    }
}