using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpSand;


public static unsafe class Utility {
    public static List<Vector2i> CircleSymmetry(Vector2i center, int x, int y) {
        return new List<Vector2i>() {
            new Vector2i(center.X + x, center.Y + y),
            new Vector2i(center.X - x, center.Y + y),
            new Vector2i(center.X + x, center.Y - y),
            new Vector2i(center.X - x, center.Y - y),
            new Vector2i(center.X + y, center.Y + x),
            new Vector2i(center.X - y, center.Y + x),
            new Vector2i(center.X + y, center.Y - x),
            new Vector2i(center.X - y, center.Y - x)
        };
    }

    public static List<Vector2i> GetCirclePoints(Vector2i center, int radius) {
        int x = 0;
        int y = radius;
        int d = 3 - 2 * radius;
        var Points = CircleSymmetry(center, x, y);

        while (y >= x) {
            x++;
            if (d > 0) {
                y--;
                d = d + 4 * (x - y) + 10;
            } else {
                d = d + 4 * x + 6;
            }

            Points.AddRange(CircleSymmetry(center, x, y));
        }

        return Points;
    }

    // Get the points along a single cell line
    public static List<Vector2i> GetLine(Vector2i a, Vector2i b) {
        List<Vector2i> Points = new List<Vector2i>();

        int w = b.X - a.X;
        int h = b.Y - a.Y;
        Vector2i d1 = Vector2i.Zero;
        Vector2i d2 = Vector2i.Zero;

        if (w < 0) d1.X = -1; else if (w > 0) d1.X = 1;
        if (h < 0) d1.Y = -1; else if (h > 0) d1.Y = 1;
        if (w < 0) d2.X = -1; else if (w > 0) d2.X = 1;

        int longest  = Math.Abs(w);
        int shortest = Math.Abs(h);
        if (!(longest > shortest)) {
            longest = Math.Abs(h);
            shortest = Math.Abs(w);
            if (h < 0) d2.Y = -1; else if (h > 0) d2.Y = 1;
            d2.X = 0;
        }

        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++) {
            Points.Add(new Vector2i(a.X, a.Y));
            numerator += shortest;
            if (!(numerator < longest)) {
                numerator -= longest;
                a.X += d1.X;
                a.Y += d1.Y;
            } else {
                a.X += d2.X;
                a.Y += d2.Y;
            }
        }

        return Points;
    }

    // Get the points along a line between two points
    public static List<Vector2i> GetLinePoints(Vector2i start, Vector2i end, int size) {
        List<Vector2i> Points = new List<Vector2i>();

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                Vector2i a = new Vector2i((start.X - size / 2) + x, (start.Y - size / 2) + y);
                Vector2i b = new Vector2i((end.X - size / 2) + x, (end.Y - size / 2) + y);

                int w = b.X - a.X;
                int h = b.Y - a.Y;
                Vector2i d1 = Vector2i.Zero;
                Vector2i d2 = Vector2i.Zero;

                if (w < 0) d1.X = -1; else if (w > 0) d1.X = 1;
                if (h < 0) d1.Y = -1; else if (h > 0) d1.Y = 1;
                if (w < 0) d2.X = -1; else if (w > 0) d2.X = 1;

                int longest  = Math.Abs(w);
                int shortest = Math.Abs(h);
                if (!(longest > shortest)) {
                    longest = Math.Abs(h);
                    shortest = Math.Abs(w);
                    if (h < 0) d2.Y = -1; else if (h > 0) d2.Y = 1;
                    d2.X = 0;
                }

                int numerator = longest >> 1;
                for (int i = 0; i <= longest; i++) {
                    Points.Add(new Vector2i(a.X, a.Y));
                    numerator += shortest;
                    if (!(numerator < longest)) {
                        numerator -= longest;
                        a.X += d1.X;
                        a.Y += d1.Y;
                    } else {
                        a.X += d2.X;
                        a.Y += d2.Y;
                    }
                }
            }
        }

        return Points;
    }

    // Snap coordinates to the nearest integer grid location
    public static Vector2i GridSnap(Vector2i position, int grid_size) {
        return new Vector2i(
            (int)Math.Round((float)position.X / grid_size) * grid_size,
            (int)Math.Round((float)position.Y / grid_size) * grid_size
        );
    }

    // Return a color offset by a specified amount
    public static Color OffsetColor(Color color, int offset) {
        int Offset = RNG.Range(-offset, offset);
        int R = Math.Clamp(color.r + Offset, 10, 255);
        int G = Math.Clamp(color.g + Offset, 10, 255);
        int B = Math.Clamp(color.b + Offset, 10, 255);

        return new Color(R, G, B, color.a);
    }

    // Gets the base color from an element by instancing it (this is not good)
    public static Color GetElementBaseColor(string element_name) {
        Type t = Type.GetType(element_name)!;
        Element e = (Element)Activator.CreateInstance(t, Vector2i.Zero)!;
        return e.BaseColor;
    }

    // Gets an offset color from an element by instancing it (this is also not good)
    public static Color GetElementOffsetColor(string element_name) {
        Type t = Type.GetType(element_name)!;
        Element e = (Element)Activator.CreateInstance(t, Vector2i.Zero)!;
        return OffsetColor(e.BaseColor, e.ColorOffset);
    }

    // Gets a preview texture of an element (this is horrible but it only runs once)
    public static Texture2D GetElementTexture(string element_name, int width, int height, int scale) {
        Image Buffer = GenImageColor(width, height, Color.MAGENTA);
        Texture2D Texture = LoadTextureFromImage(Buffer);

        for (int x = 0; x < width / scale; x++) {
            for (int y = 0; y < height / scale; y++) {
                Color c = GetElementOffsetColor(element_name);
                int ix = x * scale;
                int iy = y * scale;
                
                for (int i = ix; i < ix + scale; i++) {
                    for (int j = iy; j < iy + scale; j++) {
                        ImageDrawPixel(ref Buffer, i, j, c);
                    }
                }
            }
        }

        UpdateTexture(Texture, Buffer.data);

        return Texture;
    }

    public static Texture2D GetRandomTexture(int width, int height, int scale) {
        Image Buffer = GenImageColor(width, height, Color.MAGENTA);
        Texture2D Texture = LoadTextureFromImage(Buffer);

        for (int x = 0; x < width / scale; x++) {
            for (int y = 0; y < height / scale; y++) {
                Color c = new Color(
                    RNG.Range(0, 255),
                    RNG.Range(0, 255),
                    RNG.Range(0, 255),
                    255
                );
                int ix = x * scale;
                int iy = y * scale;
                
                for (int i = ix; i < ix + scale; i++) {
                    for (int j = iy; j < iy + scale; j++) {
                        ImageDrawPixel(ref Buffer, i, j, c);
                    }
                }
            }
        }

        UpdateTexture(Texture, Buffer.data);

        return Texture;
    }
}