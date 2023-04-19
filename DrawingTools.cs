using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpSand;


class DrawingTools {
    private string[] ElementNames = new string[] { "Sand", "Dirt", "Snow", "Water", "Lava", "Fire", "Ice", "Plant", "Concrete", "Ember" };
    private List<Texture2D> ElementTextures = new List<Texture2D>();
    private int ElementIndex = 0;

    private Vector2 ScreenSize;
    private Vector2 MatrixSize;
    public int Scale { get; private set; }

    public int BrushSize { get; private set; } = 5;
    public int BrushDensity { get { return Math.Max(BrushSize / 2, 1); } }
    public string BrushElement { get { return "SharpSand." + ElementNames[ElementIndex]; } }

    public int MinBrushSize { get; } = 1;
    public int MaxBrushSize { get { return 400 / Scale; } }

    public Theme Theme { get; private set; }

    public bool PaintOver { get; private set; } = false;


    public DrawingTools(Vector2 screen_size, Vector2 matrix_size) {
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
    public void Update() {
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
            Vector2 Pos = GetMousePos();
            int Size = BrushSize * (Scale / 2);
            int BX = ((int)Pos.X * Scale) - (Size / Scale) * Scale;
            int BY = ((int)Pos.Y * Scale) - (Size / Scale) * Scale;

            Vector2 Position = new Vector2(
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
        Vector2 Pos = GetMousePos();
        int Size = BrushSize * (Scale / 2);
        int BX = ((int)Pos.X * Scale) - (Size / Scale) * Scale;
        int BY = ((int)Pos.Y * Scale) - (Size / Scale) * Scale;

        for (int x = BX; x < BX + (Size * Scale / 2); x++) {
            for (int y = BY; y < BY + (Size * Scale / 2); y++) {
                Vector2 Position = new Vector2(x / Scale, y / Scale);

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
        Vector2 Pos = GetMousePos();
        int Size = BrushSize * (Scale / 2);
        int BX = ((int)Pos.X * Scale) - (Size / Scale) * Scale;
        int BY = ((int)Pos.Y * Scale) - (Size / Scale) * Scale;

        for (int x = BX; x < BX + (Size * Scale / 2); x++) {
            for (int y = BY; y < BY + (Size * Scale / 2); y++) {
                Vector2 Position = new Vector2(x / Scale, y / Scale);
                matrix.Set(Position, new Air(Position));
            }
        }
    }

    // Paint a box of an element to the matrix 
    public void PaintBox(Matrix matrix, Vector2 origin, Vector2 size, string element_name) {
        for (int x = (int)origin.X; x < (int)origin.X + size.X; x++) {
            for (int y = (int)origin.Y; y < (int)origin.Y + size.Y; y++) {
                Vector2 Pos = new Vector2(x, y);
                Type t = Type.GetType("SharpSand." + element_name)!;
                matrix.Set(new Vector2(x, y), (Element)Activator.CreateInstance(t, Pos)!);
            }
        }
    }

    // Paint a line of the given thickness from point a to point b (only straight lines for now)
    public void PaintLine(Matrix matrix, Vector2 a, Vector2 b, string element_name) {

    }

    // Draw a rectangle indicator for the brush position and size
    public void DrawBrushIndicator() {
        Vector2 Pos = GetMousePos();
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
        DrawTextShadow(Text, new Vector2(50, 6), Vector2.One, 20, Theme.ForegroundColor, Theme.ShadowColor);
    }

    // Draw text with a drop shadow
    public void DrawTextShadow(string text, Vector2 position, Vector2? shadow_offset=null, int? font_size=null, Color? text_color=null, Color? shadow_color=null) {
        var so = shadow_offset ?? Vector2.One;
        var fs = font_size ?? 20;
        var tc = text_color ?? Theme.ForegroundColor;
        var sc = shadow_color ?? Theme.ShadowColor;
        DrawText(text, (int)position.X + (int)so.X, (int)position.Y + (int)so.Y, fs, sc);
        DrawText(text, (int)position.X, (int)position.Y, fs, tc);
    }

    // Draw the current FPS to the upper right corner of the screen
    public void DrawFPS() {
        string FPS = String.Format("{0} FPS", GetFPS());
        DrawTextShadow(FPS, new Vector2(ScreenSize.X - (MeasureText(FPS, 20) + 5), 5), Vector2.One, 20, Theme.ForegroundColor, Theme.ShadowColor);
    }

    // Draw all of the HUD elements
    public void DrawHUD() {
        DrawBrushElement();
        DrawTextShadow("Brush: " + BrushSize, new Vector2(5, 30));
        DrawFPS();
    }

    // Get the mouse position in the matrix (mouse position on screen adjusted for scale)
    public Vector2 GetMousePos() {
        return Utility.GridSnap(GetMousePosition() / Scale, 1);
    }
}