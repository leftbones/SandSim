using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpSand;


class DrawingTools {
    private int ElementIndex = 0;
    private List<Texture2D> ElementTextures = new List<Texture2D>();
    private Texture2D RandomTexture;

    private Vector2i ScreenSize;
    private Vector2i MatrixSize;
    public int Scale { get; private set; }

    public int BrushSize { get; private set; } = 5;
    public float BrushDensity { get { return 1.0f / BrushSize; } }
    public string BrushElement { get { if (ElementIndex == -1) return "Chaos"; else return "SharpSand." + Atlas.Entries.ElementAt(ElementIndex).Key; } }
    public bool BrushSolid { get { if (ElementIndex == -1) return RNG.CoinFlip(); else return Atlas.Entries.ElementAt(ElementIndex).Value.ElementType == ElementType.Solid; } }

    public int MinBrushSize { get; } = 1;
    public int MaxBrushSize { get { return 200 / Scale; } }

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
        for (int i = 0; i < Atlas.Entries.Count; i++) {
            try {
                string Name = Atlas.Entries.ElementAt(i).Key;
                ElementTextures.Add(Utility.GetElementTexture("SharpSand." + Name, 40, 20, Scale));
            } catch {
                Console.WriteLine("[ERROR] Failed to generate preview texture for element '" + Atlas.Entries.ElementAt(i).Key + "'");
            }
        }

        RandomTexture = Utility.GetRandomTexture(40, 20, Scale);

        HideCursor();
    }

    // Read inputs for resizing the brush and changing elements
    public void Update(Matrix matrix) {
        // Update mouse position
        MousePosB = MousePosA;
        MousePosA = GetMousePos();

        // Painting
        if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
            PaintLine(matrix, MousePosA, MousePosB, BrushElement, BrushSize, BrushDensity);

        // Erasing
        if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
            PaintLine(matrix, MousePosA, MousePosB, "SharpSand.Air", BrushSize, erase: true);

        // Brush Size (Hold LSHIFT for faster scrolling)
        int MouseWheelAmt = (int)GetMouseWheelMove();
        if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
            MouseWheelAmt *= Scale;

        BrushSize -= MouseWheelAmt;
        BrushSize = Math.Clamp(BrushSize, MinBrushSize, MaxBrushSize);

        // Change Brush Element
        if (IsKeyPressed(KeyboardKey.KEY_W) && ElementIndex < Atlas.Entries.Count - 1)
            ElementIndex++;

        if (IsKeyPressed(KeyboardKey.KEY_S) && ElementIndex > -1)
            ElementIndex--;

        // Toggle PaintOver
        if (IsKeyPressed(KeyboardKey.KEY_O))
            PaintOver = !PaintOver;
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

    // Paint a circle using the line drawing algorithm
    // public void PaintCircle(Matrix matrix, string element_name, Vector2i center, int radius, float density=1.0f, bool erase=false) {
    //     List<Vector2i> Points = new List<Vector2i>();
    //     List<Vector2i> Cache = new List<Vector2i>();

    //     Type t;
    //     if (ElementIndex == -1)
    //         t = Type.GetType("SharpSand." + Atlas.Entries.ElementAt(RNG.Range(0, Atlas.Entries.Count - 1)).Key)!;
    //     else
    //         t = Type.GetType(element_name)!;

    //     Points = Utility.GetCirclePoints(center, radius / 2);
    //     foreach (Vector2i Point in Points) {
    //         int steps = 0;
    //         int limit = RNG.Range(1, radius / 2);
    //         var LinePoints = Utility.GetLine(center, Point);
    //         foreach (Vector2i LP in LinePoints) {
    //             steps++;
    //             if (steps > limit)
    //                 break;

    //             if (Cache.Contains(Point))
    //                 continue;

    //             var NE = (Element)Activator.CreateInstance(t, LP)!;
    //             matrix.Set(LP, NE);
    //             Cache.Add(LP);
    //         }
    //     }
    // }

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
        string Text;
        if (ElementIndex == -1) {
            DrawTexture(RandomTexture, 5, 5, Color.WHITE);
            Text = BrushElement;
        } else {
            DrawTexture(ElementTextures[ElementIndex], 5, 5, Color.WHITE);
            Text = BrushElement.Split(".")[1];
        }

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