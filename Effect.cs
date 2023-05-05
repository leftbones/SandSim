using Raylib_cs;

namespace SharpSand;


public static class Effect {
    public static readonly Color[] FireColors = new Color[] {
        new Color(239, 71, 111, 255),
        new Color(255, 117, 56, 255),
        new Color(255, 206, 92, 255)
    };
    
    public static readonly Color[] BlueFireColors = new Color[] {
        new Color(1, 151, 244, 255),
        new Color(4, 174, 173, 255),
        new Color(240, 246, 240, 255)
    };

    public static Color GetFireColor() {
        return FireColors[RNG.Random.Next(0, FireColors.Length)];
    }

    public static Color GetBlueFireColor() {
        return BlueFireColors[RNG.Random.Next(0, BlueFireColors.Length)];
    }

    public static Color GetNanobotsColor() {
        int n = RNG.Range(20, 220);
        return new Color(n, n, n, 255);
    }

    public static Color GetRandomColor() {
        return new Color(
            RNG.Range(25, 255),
            RNG.Range(25, 255),
            RNG.Range(25, 255),
            255
        );
    }

    public static Color DarkenColor(Color color, int amount) {
        return new Color(
            Math.Clamp(color.r - amount, 0, 255),
            Math.Clamp(color.g - amount, 0, 255),
            Math.Clamp(color.b - amount, 0, 255),
            color.a
        );
    }

    public static Color LightenColor(Color color, int amount) {
        return new Color(
            Math.Clamp(color.r + amount, 0, 255),
            Math.Clamp(color.g + amount, 0, 255),
            Math.Clamp(color.b + amount, 0, 255),
            color.a
        );
    }
}