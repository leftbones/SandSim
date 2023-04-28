using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpSand;


class DrawingTools {
    private string[] ElementNames = new string[] { "Sand", "Dirt", "Salt", "Snow", "Nanobots", "Water", "Oil", "Lava", "Fire", "Ice", "Plant", "Wood", "Concrete", "Wick", "Ember", "Soot", "Smoke", "Steam" };
    private List<Texture2D> ElementTextures = new List<Texture2D>();
    private int ElementIndex = 0;

    private Vector2i ScreenSize;
    private Vector2i MatrixSize;
    public int Scale { get; private set; }

    public int BrushSize { get; private set; } = 5;
    public int BrushDensity { get { return Math.Max(BrushSize / 2, 1); } }
    public string BrushElement { get { return "SharpSand." + ElementNames[ElementIndex]; } }

    public int MinBrushSize { get; } = 1;
    public int MaxBrushSize { get { return 400 / Scale; } }

    public Theme Theme { get; private set; }

    public bool PaintOver { get; private set; } = false;

    public Vector2i MousePosA { get; set; }
    public Vector2i MousePosB { get; set; }


    public DrawingTools(Vector2i screen_size, Vector2i matrix_size) {
        ScreenSize = screen_size;
        MatrixSize = matrix_size;
        Scale = (int)(ScreenSize.X / MatrixSize.X);
        Theme = new Theme();

        // Generate element preview textures
        for (int i = 0; i < ElementNames.Length; i++) {
            try {
                ElementTextures.Add(Utility.GetElementTexture("SharpSand." + ElementNames[i], 40, 20, Scale));
            } catch {
                Console.WriteLine("[ERROR] Failed to generate preview texture for element '" + ElementNames[i] + "'");
            }
        }

        HideCursor();
    }

    // Read inputs for resizing the brush and changing elements
    public void Update(Matrix matrix) {
        // Update mouse position
        MousePosB = MousePosA;
        MousePosA = GetMousePos();

        // Painting
        if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT)) {
            for (int x = 0; x < BrushSize; x++) {
                for (int y = 0; y < BrushSize; y++) {
                    Vector2i A = new Vector2i((MousePosA.X - BrushSize / 2) + x, (MousePosA.Y - BrushSize / 2) + y);
                    Vector2i B = new Vector2i((MousePosB.X - BrushSize / 2) + x, (MousePosB.Y - BrushSize / 2) + y);
                    PaintLine(matrix, A, B);
                }
            }
        }

        // Erasing
        if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT)) {
            for (int x = 0; x < BrushSize; x++) {
                for (int y = 0; y < BrushSize; y++) {
                    Vector2i A = new Vector2i((MousePosA.X - BrushSize / 2) + x, (MousePosA.Y - BrushSize / 2) + y);
                    Vector2i B = new Vector2i((MousePosB.X - BrushSize / 2) + x, (MousePosB.Y - BrushSize / 2) + y);
                    PaintLine(matrix, A, B, "SharpSand.Air");
                }
            }
        }

        // Brush Size (Hold LSHIFT for faster scrolling)
        int MouseWheelAmt = (int)GetMouseWheelMove();
        if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
            MouseWheelAmt *= Scale;

        BrushSize -= MouseWheelAmt;
        BrushSize = Math.Clamp(BrushSize, MinBrushSize, MaxBrushSize);

        // Change Brush Element
        if (IsKeyPressed(KeyboardKey.KEY_W) && ElementIndex < ElementNames.Length - 1)
            ElementIndex++;

        if (IsKeyPressed(KeyboardKey.KEY_S) && ElementIndex > 0)
            ElementIndex--;

        // Toggle PaintOver
        if (IsKeyPressed(KeyboardKey.KEY_O))
            PaintOver = !PaintOver;
    }

    // Paint elements in the brush area, scattered based on density
    public void Paint(Matrix matrix) {
        for (int i = 0; i < BrushDensity; i++) {
            Vector2i Pos = GetMousePos();
            int Size = BrushSize * (Scale / 2);
            int BX = ((int)Pos.X * Scale) - (Size / Scale) * Scale;
            int BY = ((int)Pos.Y * Scale) - (Size / Scale) * Scale;

            Vector2i Position = new Vector2i(
                RNG.Range(BX, BX + (Size * Scale / 2) - 1) / Scale,
                RNG.Range(BY, BY + (Size * Scale / 2) - 1) / Scale
            );

            if (PaintOver || matrix.IsEmpty(Position)) {
                Type t = Type.GetType(BrushElement)!;
                Element e = (Element)Activator.CreateInstance(t, Position)!;
                if (e.Type == ElementType.Solid) {
                    PaintSolid(matrix);
                    return;
                }
                matrix.Set(Position, (Element)Activator.CreateInstance(t, Position)!);
            }
        }
    }

    // Paint every element within the brush area
    public void PaintSolid(Matrix matrix) {
        Vector2i Pos = GetMousePos();
        int Size = BrushSize * (Scale / 2);
        int BX = ((int)Pos.X * Scale) - (Size / Scale) * Scale;
        int BY = ((int)Pos.Y * Scale) - (Size / Scale) * Scale;

        for (int x = BX; x < BX + (Size * Scale / 2); x++) {
            for (int y = BY; y < BY + (Size * Scale / 2); y++) {
                Vector2i Position = new Vector2i(x / Scale, y / Scale);

                if (PaintOver || matrix.IsEmpty(Position)) {
                    Type t = Type.GetType(BrushElement)!;
                    Element e = (Element)Activator.CreateInstance(t, Position)!;
                    matrix.Set(Position, e);
                }
            }
        }
    }

    // Erase elements (paints air)
    public void Erase(Matrix matrix) {
        Vector2i Pos = GetMousePos();
        int Size = BrushSize * (Scale / 2);
        int BX = ((int)Pos.X * Scale) - (Size / Scale) * Scale;
        int BY = ((int)Pos.Y * Scale) - (Size / Scale) * Scale;

        for (int x = BX; x < BX + (Size * Scale / 2); x++) {
            for (int y = BY; y < BY + (Size * Scale / 2); y++) {
                Vector2i Position = new Vector2i(x / Scale, y / Scale);
                matrix.Set(Position, new Air(Position));
            }
        }
    }

    // Paint a box of an element to the matrix 
    public void PaintBox(Matrix matrix, Vector2i origin, Vector2i size, string element_name) {
        for (int x = (int)origin.X; x < (int)origin.X + size.X; x++) {
            for (int y = (int)origin.Y; y < (int)origin.Y + size.Y; y++) {
                Vector2i Pos = new Vector2i(x, y);
                Type t = Type.GetType("SharpSand." + element_name)!;
                matrix.Set(new Vector2i(x, y), (Element)Activator.CreateInstance(t, Pos)!);
            }
        }
    }

    // Paint a line from point A to point B
    public void PaintLine(Matrix matrix, Vector2i a, Vector2i b, string? element_name=null) {
        string element = element_name ?? BrushElement;

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

        Type t = Type.GetType(element)!;

        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++) {
            Vector2i pos = new Vector2i(a.X, a.Y);
            if (PaintOver || matrix.IsEmpty(pos) || element == "SharpSand.Air" && !matrix.IsEmpty(pos))
                matrix.Set(pos, (Element)Activator.CreateInstance(t, pos)!);
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
        DrawTextShadow(Text, new Vector2i(50, 6), Vector2i.One, 20, Theme.ForegroundColor, Theme.ShadowColor);
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

    // Draw the current FPS to the upper right corner of the screen
    public void DrawFPS(Color? color=null) {
        color = color ?? Theme.ForegroundColor;
        string FPS = String.Format("{0} FPS", GetFPS());
        DrawTextShadow(FPS, new Vector2i(ScreenSize.X - (MeasureText(FPS, 20) + 5), 5), Vector2i.One, 20, color, Theme.ShadowColor);
    }

    // Draw all of the HUD elements
    public void DrawHUD() {
        DrawBrushElement();
        DrawTextShadow("Brush: " + BrushSize, new Vector2i(5, 30));
    }

    // Get the mouse position in the matrix (mouse position on screen adjusted for scale)
    public Vector2i GetMousePos() {
        return Utility.GridSnap(new Vector2i(GetMousePosition()) / Scale, 1);
    }
}