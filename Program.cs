using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.TraceLogLevel;

namespace SharpSand;

class Program {
    static unsafe void Main(string[] args) {
        // Init + Setup
        Console.WriteLine("[SYSTEM] Simulation initialized");
        Vector2i ScreenSize = new Vector2i(1280, 720);
        int Scale = 4;

        SetTraceLogLevel(LOG_WARNING | LOG_ERROR | LOG_FATAL);
        InitWindow(ScreenSize.X, ScreenSize.Y, "Sand");
        SetTargetFPS(200);

        Console.WriteLine("[SYSTEM] Matrix initialized");
        var Matrix = new Matrix(ScreenSize, Scale);

        Image BufferImage = GenImageColor(Matrix.Size.X, Matrix.Size.Y, Color.BLACK);
        Texture2D BufferTexture = LoadTextureFromImage(BufferImage);

        var DrawingTools = new DrawingTools(ScreenSize, Matrix.Size);

        var Settings = new Settings();

        List<string> HelpText = new List<string>() {
            "Controls:",
            "<Mouse 1> Paint",
            "<Mouse 2> Erase",
            "<Scroll> Change brush size",
            "<LShift> Faster scrolling",
            "<W> Next element",
            "<S> Previous element",
            "<O> Toggle 'Paint Over'",
            "<Space> Pause/Play simulation",
            "<T> Advance one tick",
            "",
            "Hotkeys:",
            "<F2> Toggle FPS cap",
            // "<F3> Toggle water spout", // Unused
            "<F4> Toggle world borders",
            "<F5> Reset world",
            "<F6> Toggle weather",
            "<F7> Cycle weather elements",
            "<F8> Toggle element name"
        };

        Console.WriteLine("[SYSTEM] Init complete");

        // Main Loop
        while (!WindowShouldClose()) {
            // Hotkeys
            if (IsKeyDown(KeyboardKey.KEY_F1)) Settings.DisplayHelpText = true;
            else Settings.DisplayHelpText = false;

            if (IsKeyPressed(KeyboardKey.KEY_F2)) {
                Settings.CycleSimulationSpeed();
            }

            if (IsKeyPressed(KeyboardKey.KEY_F4)) {
                Matrix.DestroyOutOfBounds = !Matrix.DestroyOutOfBounds;
                Matrix.UnsettleAll();
            }

            if (IsKeyPressed(KeyboardKey.KEY_F5))
                Matrix.Reset();

            if (IsKeyPressed(KeyboardKey.KEY_F6))
                Settings.WeatherEnabled = !Settings.WeatherEnabled;

            if (IsKeyPressed(KeyboardKey.KEY_F7)) {
                Settings.CycleWeatherElement();
            }

            if (IsKeyPressed(KeyboardKey.KEY_F8))
                Settings.ShowElementName = !Settings.ShowElementName;

            if (IsKeyPressed(KeyboardKey.KEY_F9))
                Settings.DrawChunkBorders = !Settings.DrawChunkBorders;

            if (IsKeyPressed(KeyboardKey.KEY_SPACE)) {
                Matrix.Active = !Matrix.Active;
            }

            if (IsKeyPressed(KeyboardKey.KEY_T)) {
                if (!Matrix.Active) {
                    Matrix.Active = true;
                    Matrix.StepTick = true;
                }
            }

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

            // Weather
            if (Settings.WeatherEnabled) {
                for (int i = 0; i < Settings.WeatherStrength; i++) {
                    Vector2i Pos = new Vector2i(RNG.Range(0, Matrix.Size.X - 1), 0);

                    Type t = Type.GetType("SharpSand." + Settings.WeatherElements[Settings.WeatherSelected])!;
                    if (Matrix.IsEmpty(Pos))
                        Matrix.Set(Pos, (Element)Activator.CreateInstance(t, Pos)!);
                }
            }

            // Texture
            ImageClearBackground(ref BufferImage, DrawingTools.Theme.BackgroundColor);

            foreach (Element e in Matrix.Elements) {
                if (e.GetType() != typeof(Air)) {
                    Color c = e.Color;
                    ImageDrawPixel(ref BufferImage, e.Position.X, e.Position.Y, c);
                    e.AlreadyStepped = false;
                }
            }

            UpdateTexture(BufferTexture, BufferImage.data);

            // Draw
            BeginDrawing();
            ClearBackground(DrawingTools.Theme.BackgroundColor);

            DrawTexturePro(BufferTexture, new Rectangle(0, 0, Matrix.Size.X, Matrix.Size.Y), new Rectangle(0, 0, ScreenSize.X, ScreenSize.Y), Vector2.Zero, 0, Color.WHITE);

            // Draw Chunk Borders
            if (Settings.DrawChunkBorders) {
                foreach (Chunk Chunk in Matrix.Chunks) {
                    Chunk.DebugDraw();
                }
            }

            // Draw Interface
            DrawingTools.DrawHUD();
            DrawingTools.DrawFPS();
            DrawingTools.DrawBrushIndicator();

            // Show help text
            if (Settings.DisplayHelpText) {
                for (int i = 0; i < HelpText.Count; i++) { 
                    DrawingTools.DrawTextShadow(HelpText[i], new Vector2i(5, 80 + (25 * i)));
                }
            } else {
                DrawingTools.DrawTextShadow("<F1> Help", new Vector2i(5, 80));
            }

            // Draw name of element under cursor
            if (Settings.ShowElementName) {
                Vector2i MousePos = DrawingTools.GetMousePos();
                if (Matrix.InBounds(MousePos)) {
                    Element e = Matrix.Get(MousePos);
                    string ElementName = e.ToString()!.Split(".")[1];
                    if (e.OnFire) ElementName += " (on fire)";
                    ElementName += String.Format(" (Temp: {0})", e.ActiveTemp);

                    DrawingTools.DrawTextShadow(ElementName, (MousePos * DrawingTools.Scale) + new Vector2i(5, 5));
                }
            }

            EndDrawing();
        }

        // Exit
        CloseWindow();
        Console.WriteLine("[SYSTEM] Simulation exited successfully");
    }
}
