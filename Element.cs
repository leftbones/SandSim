using System.Numerics;
using Raylib_cs;

namespace SharpSand;

enum ElementType { Solid, Liquid, Gas, Powder }

public static class Direction {
    public static readonly Vector2 Up = new Vector2(0, -1);
    public static readonly Vector2 Down = new Vector2(0, 1);
    public static readonly Vector2 Left = new Vector2(-1, 0);
    public static readonly Vector2 Right = new Vector2(1, 0);
    public static readonly Vector2 UpLeft = new Vector2(-1, -1);
    public static readonly Vector2 UpRight = new Vector2(-1, 1);
    public static readonly Vector2 DownLeft = new Vector2(-1, 1);
    public static readonly Vector2 DownRight = new Vector2(1, 1);

    public static readonly Vector2[] Horizontal = new Vector2[] { Direction.Left, Direction.Right };
}

abstract class Element {
    public ElementType Type { get; set; }
    public Vector2 Position { get; set; }
    public bool Settled { get; set; } = false;
    public int Friction { get; set; } = 0;
    public int ColorOffset { get; set; } = 25;
    public Color Color { get; set; } = Color.WHITE;

    public Element(Vector2 position) {
        Position = position;
    }

    public abstract void Update(Matrix matrix);

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