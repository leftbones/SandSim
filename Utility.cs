using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpSand;


public static unsafe class Utility {
    // Return a color offset by a specified amount
    public static Color OffsetColor(Color color, int offset) {
        int Offset = RNG.Range(-offset, offset);
        int R = Math.Clamp(color.r + Offset, 0, 255);
        int G = Math.Clamp(color.g + Offset, 0, 255);
        int B = Math.Clamp(color.b + Offset, 0, 255);

        return new Color(R, G, B, color.a);
    }

    // Gets the base color from an element by instancing it (this is not good)
    public static Color GetElementBaseColor(string element_name) {
        Type t = Type.GetType(element_name)!;
        Element e = (Element)Activator.CreateInstance(t, Vector2.Zero)!;
        return e.BaseColor;
    }

    // Gets an offset color from an element by instancing it (this is also not good)
    public static Color GetElementOffsetColor(string element_name) {
        Type t = Type.GetType(element_name)!;
        Element e = (Element)Activator.CreateInstance(t, Vector2.Zero)!;
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
                ImageDrawPixel(ref Buffer, ix, iy, c);
                ImageDrawPixel(ref Buffer, ix+1, iy, c);
                ImageDrawPixel(ref Buffer, ix, iy+1, c);
                ImageDrawPixel(ref Buffer, ix+1, iy+1, c);
            }
        }

        UpdateTexture(Texture, Buffer.data);

        return Texture;
    }
}