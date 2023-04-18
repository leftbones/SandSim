using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.TraceLogLevel;

namespace SharpSand;

class Program {
    static unsafe void Main(string[] args) {
        // Init
        Console.WriteLine("[SYSTEM] Simulation initialized");
        Vector2 ScreenSize = new Vector2(1280, 720);
        Vector2 MatrixSize = new Vector2(320, 180);

        SetTraceLogLevel(LOG_WARNING | LOG_ERROR | LOG_FATAL);
        InitWindow((int)ScreenSize.X, (int)ScreenSize.Y, "Sand");
        SetTargetFPS(200);

        Console.WriteLine("[SYSTEM] Matrix initialized");
        Matrix Matrix = new Matrix(new Vector2((int)MatrixSize.X, (int)MatrixSize.Y));

        Image BufferImage = GenImageColor((int)MatrixSize.X, (int)MatrixSize.Y, Color.BLACK);
        Texture2D BufferTexture = LoadTextureFromImage(BufferImage);

        var DrawingTools = new DrawingTools(ScreenSize, MatrixSize);

        // Function Keys
        bool ShowHelpText = false;
        bool ShowElementName = false;
        bool LimitFPS = true;

        List<string> HelpText = new List<string>() {
            "Controls:",
            "<Mouse 1> Paint",
            "<Mouse 2> Erase",
            "<Scroll> Change brush size",
            "<LShift> Faster scrolling",
            "<W> Next element",
            "<S> Previous element",
            "<O> Toggle 'Paint Over'",
            "",
            "Hotkeys:",
            "<F2> Toggle FPS cap",
            "<F3> Toggle water spout",
            "<F4> Toggle world borders",
            "<F5> Reset world"
        };


        // Spout
        bool SpoutEnabled = true;
        int SpoutSize = 2;
        int SpoutDensity = 2;
        string SpoutElement = "Water";

        // Weather
        bool WeatherEnabled = false;
        int WeatherStrength = 1;
        string WeatherElement = "Snow";

        // Platforms
        DrawingTools.PaintBox(Matrix, new Vector2(((int)MatrixSize.X / 2.0f) - 60, 60), new Vector2(120, 5), "Concrete");
        DrawingTools.PaintBox(Matrix, new Vector2(((int)MatrixSize.X / 2.0f) - 90, 120), new Vector2(60, 5), "Concrete");
        DrawingTools.PaintBox(Matrix, new Vector2(((int)MatrixSize.X / 2.0f) + 30, 120), new Vector2(60, 5), "Concrete");

        Console.WriteLine("[SYSTEM] Init complete");

        // Main Loop
        while (!WindowShouldClose()) {
            // Hotkeys
            if (IsKeyDown(KeyboardKey.KEY_F1)) ShowHelpText = true;
            else ShowHelpText = false;

            if (IsKeyPressed(KeyboardKey.KEY_F2)) {
                if (LimitFPS) SetTargetFPS(1000);
                else SetTargetFPS(200);
                LimitFPS = !LimitFPS;
            }

            if (IsKeyPressed(KeyboardKey.KEY_F3))
                SpoutEnabled = !SpoutEnabled;

            if (IsKeyPressed(KeyboardKey.KEY_F4)) {
                Matrix.DestroyOutOfBounds = !Matrix.DestroyOutOfBounds;
                Matrix.UnsettleAll();
            }

            if (IsKeyPressed(KeyboardKey.KEY_F5))
                Matrix.Reset();

            // Update
            Matrix.Update();
            DrawingTools.Update();

            // Brush
            if (IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON)) {
                DrawingTools.Paint(Matrix);
            }

            // Eraser
            if (IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON)) {
                DrawingTools.Erase(Matrix);
            }

            // Spout
            if (SpoutEnabled) {
                for (int i = 0; i < SpoutDensity; i++) {
                    Vector2 SpoutPos = new Vector2((MatrixSize.X / 2.0f) + RNG.Range(-SpoutSize, SpoutSize), SpoutSize + RNG.Range(-SpoutSize, SpoutSize));

                    Type t = Type.GetType("SharpSand." + SpoutElement)!;
                    if (Matrix.IsEmpty(SpoutPos))
                        Matrix.Set(SpoutPos, (Element)Activator.CreateInstance(t, SpoutPos)!);
                }
            }

            // Weather
            if (WeatherEnabled) {
                for (int i = 0; i < WeatherStrength; i++) {
                    Vector2 Pos = new Vector2(RNG.Range(0, (int)MatrixSize.X - 1), 0);

                    Type t = Type.GetType("SharpSand." + WeatherElement)!;
                    if (Matrix.IsEmpty(Pos))
                        Matrix.Set(Pos, (Element)Activator.CreateInstance(t, Pos)!);
                }
            }

            // Texture
            ImageClearBackground(ref BufferImage, DrawingTools.Theme.BackgroundColor);

            foreach (Element e in Matrix.Elements) {
                if (e is not Air) {
                    Color c = e.Color;
                    // if (e.Settled) c = Color.MAGENTA;
                    ImageDrawPixel(ref BufferImage, (int)e.Position.X, (int)e.Position.Y, c);
                    e.AlreadyStepped = false;
                }
            }

            UpdateTexture(BufferTexture, BufferImage.data);

            // Draw
            BeginDrawing();
            ClearBackground(DrawingTools.Theme.BackgroundColor);

            DrawTexturePro(BufferTexture, new Rectangle(0, 0, MatrixSize.X, MatrixSize.Y), new Rectangle(0, 0, ScreenSize.X, ScreenSize.Y), Vector2.Zero, 0, Color.WHITE);

            DrawingTools.DrawHUD();
            DrawingTools.DrawBrushIndicator();

            // Show help text
            if (ShowHelpText) {
                for (int i = 0; i < HelpText.Count; i++) { 
                    DrawingTools.DrawTextShadow(HelpText[i], new Vector2(5, 80 + (25 * i)));
                }
            } else {
                DrawingTools.DrawTextShadow("<F1> Help", new Vector2(5, 80));
            }

            // Draw name of element under cursor
            if (ShowElementName) {
                Vector2 MousePos = DrawingTools.GetMousePos();
                if (Matrix.InBounds(MousePos)) {
                    Element e = Matrix.Get(MousePos);
                    string ElementName = e.ToString()!.Split(".")[1];
                    if (e.OnFire)
                        ElementName += " (on fire)";
                    DrawingTools.DrawTextShadow(ElementName, (MousePos * DrawingTools.Scale) + new Vector2(5, 5));
                }
            }

            EndDrawing();
        }

        // Exit
        CloseWindow();
        Console.WriteLine("[SYSTEM] Simulation exited successfully");
    }
}
