using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpSand;


class DrawingTools {
    private string[] ElementNames = new string[] { "Sand", "Dirt", "Snow", "Soot", "Ember", "Water", "Fire", "Smoke", "Steam", "Concrete", "Ice", "Plant" };
    private List<Texture2D> ElementTextures = new List<Texture2D>();
    private int ElementIndex = 0;

    private Vector2 ScreenSize;
    private Vector2 MatrixSize;

    public int Scale { get; set; } = 2;

    public int BrushSize { get; set; } = 5;
    public int BrushDensity { get { return Math.Max(BrushSize / Scale, 1); } }
    public string BrushElement { get { return "SharpSand." + ElementNames[ElementIndex]; } }

    public Vector2 BrushPos { get { return Utility.GridSnap(new Vector2((int)GetMouseX() / Scale, (int)GetMouseY() / Scale), 1); } }

    public int MinBrushSize { get; } = 1;
    public int MaxBrushSize { get { return 200 / Scale; } }

    public Theme Theme { get; private set; }


    public DrawingTools(Vector2 screen_size, Vector2 matrix_size) {
        ScreenSize = screen_size;
        MatrixSize = matrix_size;
        Theme = new Theme();

        // Generate element preview textures
        for (int i = 0; i < ElementNames.Length; i++) {
            ElementTextures.Add(Utility.GetElementTexture("SharpSand." + ElementNames[i], 40, 20, Scale));
        }

        HideCursor();
    }

    public void Update() {
        // Brush Size (Hold LSHIFT for faster scrolling)
        int MouseWheel = (int)GetMouseWheelMove();
        if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
            MouseWheel *= 10;

        BrushSize -= (int)MouseWheel;
        BrushSize = Math.Clamp(BrushSize, MinBrushSize, MaxBrushSize);

        // Change Brush Element
        if (IsKeyPressed(KeyboardKey.KEY_W) && ElementIndex < ElementNames.Length - 1)
            ElementIndex++;

        if (IsKeyPressed(KeyboardKey.KEY_S) && ElementIndex > 0)
            ElementIndex--;
    }

    public void Paint(Matrix matrix) {
        for (int i = 0; i < BrushDensity; i++) {
            Vector2 MousePos = GetMousePosition() / 2.0f;
            Vector2 Position = MousePos + new Vector2(RNG.Range(-BrushSize / Scale, BrushSize / Scale), RNG.Range(-BrushSize / Scale, BrushSize / Scale));
            Type t = Type.GetType(BrushElement)!;
            Element e = (Element)Activator.CreateInstance(t, Position)!;
            if (e.Type == ElementType.Solid) {
                PaintSolid(matrix);
                return;
            }
            matrix.Set(Position, (Element)Activator.CreateInstance(t, Position)!);
        }
    }

    public void PaintSolid(Matrix matrix) {
        for (int x = 0; x < BrushSize; x++) {
            for (int y = 0; y < BrushSize; y++) {
                Vector2 MousePos = GetMousePosition() / 2.0f;
                Vector2 Position = (MousePos + new Vector2(x, y)) - new Vector2(BrushSize / 2, BrushSize / 2);
                Type t = Type.GetType(BrushElement)!;
                Element e = (Element)Activator.CreateInstance(t, Position)!;
                matrix.Set(Position, e);
            }
        }
    }

    public void Erase(Matrix matrix) {
        for (int x = 0; x < BrushSize; x++) {
            for (int y = 0; y < BrushSize; y++) {
                Vector2 MousePos = GetMousePosition() / 2.0f;
                Vector2 Position = (MousePos + new Vector2(x, y)) - new Vector2(BrushSize / 2, BrushSize / 2);
                matrix.Set(Position, new Air(Position));
            }
        }
    }

    public void DrawBox(Matrix matrix, Vector2 origin, Vector2 size, string element_name) {
        for (int x = (int)origin.X; x < (int)origin.X + size.X; x++) {
            for (int y = (int)origin.Y; y < (int)origin.Y + size.Y; y++) {
                Vector2 Pos = new Vector2(x, y);
                Type t = Type.GetType("SharpSand." + element_name)!;
                matrix.Set(new Vector2(x, y), (Element)Activator.CreateInstance(t, Pos)!);
            }
        }
    }

    public void DrawBrushIndicator() {
        DrawRectangleLines((((int)BrushPos.X * Scale) - (BrushSize / 2) * Scale) + 1, (((int)BrushPos.Y * Scale) - (BrushSize / Scale) * Scale) + 1, BrushSize * Scale, BrushSize * Scale, Theme.ShadowColor);
        DrawRectangleLines((((int)BrushPos.X * Scale) - (BrushSize / 2) * Scale), (((int)BrushPos.Y * Scale) - (BrushSize / Scale) * Scale), BrushSize * Scale, BrushSize * Scale, Theme.ForegroundColor);
    }

    public void DrawBrushElement() {
        DrawTexture(ElementTextures[ElementIndex], 5, 5, Color.WHITE);
        DrawTextShadow(BrushElement.Split(".")[1], new Vector2(50, 6), Vector2.One, 20, Theme.ForegroundColor, Theme.ShadowColor);
    }

    public void DrawTextShadow(string text, Vector2 position, Vector2 shadow_offset, int font_size, Color text_color, Color shadow_color) {
        DrawText(text, (int)position.X + (int)shadow_offset.X, (int)position.Y + (int)shadow_offset.Y, font_size, shadow_color);
        DrawText(text, (int)position.X, (int)position.Y, font_size, text_color);
    }

    public void DrawFPS() {
        string FPS = String.Format("{0} FPS", GetFPS());
        DrawTextShadow(FPS, new Vector2(ScreenSize.X - (MeasureText(FPS, 20) + 5), 5), Vector2.One, 20, Theme.ForegroundColor, Theme.ShadowColor);
    }
}