using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpSand;


class DrawingTools {
    public int ElementIndex { get; set; } = 0;
    public List<Texture2D> ElementTextures { get; private set; }

    private Vector2i ScreenSize;
    private Vector2i MatrixSize;
    public int Scale { get; private set; }

    public int BrushSize { get; set; } = 5;
    public decimal BrushDensityModifier { get; set; } = 0.5m;
    public float BrushDensity { get { return (float)(BrushDensityModifier / BrushSize); } }
    public string BrushElement { get { return "SharpSand." + Atlas.Entries.ElementAt(ElementIndex).Key; } }
    public bool BrushSolid { get { return Atlas.Entries.ElementAt(ElementIndex).Value.ElementType == ElementType.Solid; } }

    public int MinBrushSize { get; } = 1;
    public int MaxBrushSize { get { return 300 / Scale; } }

    public Theme Theme { get; private set; }

    public bool PaintOver { get; set; } = false;

    public Vector2i MousePosA { get; set; }
    public Vector2i MousePosB { get; set; }


    public DrawingTools(Vector2i screen_size, Vector2i matrix_size, Theme theme, List<Texture2D> element_textures) {
        ScreenSize = screen_size;
        MatrixSize = matrix_size;
        Theme = theme;
        ElementTextures = element_textures;
        Scale = (int)(ScreenSize.X / MatrixSize.X);

        // Generate element preview textures
        for (int i = 0; i < Atlas.Entries.Count; i++) {
            try {
                string Name = Atlas.Entries.ElementAt(i).Key;
                ElementTextures.Add(Utility.GetElementTexture("SharpSand." + Name, 40, 20, Scale));
            } catch {
                Console.WriteLine("[ERROR] Failed to generate preview texture for element '" + Atlas.Entries.ElementAt(i).Key + "'");
            }
        }

        HideCursor();
    }

    // Draw text with a drop shadow
    public void DrawTextShadow(string text, Vector2i position, Vector2i? shadow_offset=null, int? font_size=null, Color? text_color=null, Color? shadow_color=null) {
        var so = shadow_offset ?? Vector2i.One;
        var fs = font_size ?? 20;
        var tc = text_color ?? Theme.ForegroundColor;
        var sc = shadow_color ?? Theme.ShadowColor;
        DrawText(text, (int)position.X + (int)so.X, (int)position.Y + (int)so.Y, fs, sc);
        DrawText(text, (int)position.X, (int)position.Y, fs, tc);
    }

    // Paint a line from point A to point B
    public void PaintLine(Matrix matrix, Vector2i a, Vector2i b, string element_name, int size, float density=1.0f, bool erase=false) {
        bool Force = false;
        if (PaintOver || erase)
            Force = true;

        List<Vector2i> Points = Utility.GetLinePoints(a, b, size).Distinct().ToList();
        List<Vector2i> Cache = new List<Vector2i>();

        Type t;
        if (ElementIndex == -1)
            t = Type.GetType("SharpSand." + Atlas.Entries.ElementAt(RNG.Range(0, Atlas.Entries.Count - 1)).Key)!;
        else
            t = Type.GetType(element_name)!;

        foreach (Vector2i Point in Points) {
            // Point has already been visited
            if (Cache.Contains(Point))
                continue;

            // Brush element is not solid and RNG roll fails
            if (element_name != "SharpSand.Air" && !BrushSolid && !RNG.Roll(density))
                continue;

            // Brush is erasing and point is already empty
            if (erase && matrix.IsEmpty(Point)) {
                Cache.Add(Point);
                continue;
            }

            // Brush is not forced and point is not empty
            if (!Force && !matrix.IsEmpty(Point)) {
                Cache.Add(Point);
                continue;
            }

            var NewElement = (Element)Activator.CreateInstance(t, Point)!;
            matrix.Set(Point, NewElement);
            Cache.Add(Point);
        }
    }

    // Draw a rectangle indicator for the brush position and size
    public void DrawBrushIndicator() {
        Vector2i Pos = GetMousePos();
        int Size = BrushSize * (Scale / 2);
        int BX = ((int)Pos.X * Scale) - (Size / Scale) * Scale;
        int BY = ((int)Pos.Y * Scale) - (Size / Scale) * Scale;
        DrawRectangleLines(BX, BY, Size * (Scale / 2), Size * (Scale / 2), Theme.ForegroundColor);

        Size = (BrushSize - 1) * (Scale / 2);
        DrawRectangleLines(BX + Size, BY + Size, (Scale / 2) * (Scale / 2), (Scale / 2) * (Scale / 2), Theme.ForegroundColor);
    }

    // Draw the generated preview texture for the current element
    public void DrawBrushElement() {
        DrawTexture(ElementTextures[ElementIndex], 5, 5, Color.WHITE);
        string Text = BrushElement.Split(".")[1];

        if (PaintOver)
            Text += " [+]";

        DrawTextShadow(Text, new Vector2i(50, 6));
    }

    // Get the mouse position in the matrix (mouse position on screen adjusted for scale)
    public Vector2i GetMousePos() {
        return Utility.GridSnap(new Vector2i(GetMousePosition()) / Scale, 1);
    }
}