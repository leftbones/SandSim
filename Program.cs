using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.TraceLogLevel;

namespace SharpSand;


class Program {
    static unsafe void Main(string[] args) {
        // Init
        Console.WriteLine("[SYSTEM] Simulation initialized");
        Vector2 ScreenSize = new Vector2(800, 600);
        Vector2 MatrixSize = new Vector2(400, 300);

        SetTraceLogLevel(LOG_WARNING | LOG_ERROR | LOG_FATAL);
        InitWindow((int)ScreenSize.X, (int)ScreenSize.Y, "Sand");
        SetTargetFPS(144);

        Console.WriteLine("[SYSTEM] Matrix initialized");
        Matrix Matrix = new Matrix(new Vector2((int)MatrixSize.X, (int)MatrixSize.Y));

        Image BufferImage = GenImageColor((int)MatrixSize.X, (int)MatrixSize.Y, Color.BLACK);
        Texture2D BufferTexture = LoadTextureFromImage(BufferImage);

        // Brush
        int BrushSize = 5;
        int BrushDensity = 5;
        string BrushElement = "Fire";

        // Spout
        bool SpoutEnabled = true;
        int SpoutSize = 5;
        int SpoutDensity = 10;
        string SpoutElement = "Water";

        // Weather
        bool WeatherEnabled = false;
        int WeatherStrength = 1;
        string WeatherElement = "Water";

        // Plant Base
        for (int x = 0; x < (int)MatrixSize.X - 1; x++) {
            Vector2 Pos1 = new Vector2(x, MatrixSize.Y - 1);
            Vector2 Pos2 = new Vector2(x, MatrixSize.Y - 2);
            Vector2 Pos3 = new Vector2(x, MatrixSize.Y - 3);
            Matrix.Set(Pos1, new Plant(Pos1));
            Matrix.Set(Pos2, new Plant(Pos2));
            Matrix.Set(Pos3, new Plant(Pos3));
        }

        // Concrete Platform
        for (int x = (int)(MatrixSize.X / 2.0f) - 60; x < (int)(MatrixSize.X / 2.0f) + 60; x++) {
            for (int y = (int)(MatrixSize.Y / 2.0f) - 3; y < (int)(MatrixSize.Y / 2.0f) + 3; y++) {
                Vector2 Pos = new Vector2(x, y);
                Matrix.Set(Pos, new Concrete(Pos));
            }
        }

        // Concrete Walls
        for (int x = (int)(MatrixSize.X / 2.0f) - 60; x < (int)(MatrixSize.X / 2.0f) - 54; x++) {
            for (int y = (int)(MatrixSize.Y / 2.0f) - 30; y < (int)(MatrixSize.Y / 2.0f); y++) {
                Vector2 Pos = new Vector2(x, y);
                Matrix.Set(Pos, new Concrete(Pos));
            }
        }

        for (int x = (int)(MatrixSize.X / 2.0f) + 54; x < (int)(MatrixSize.X / 2.0f) + 60; x++) {
            for (int y = (int)(MatrixSize.Y / 2.0f) - 30; y < (int)(MatrixSize.Y / 2.0f); y++) {
                Vector2 Pos = new Vector2(x, y);
                Matrix.Set(Pos, new Concrete(Pos));
            }
        }

        Console.WriteLine("[SYSTEM] Init complete");

        // Main Loop
        while (!WindowShouldClose()) {
            // Update
            Matrix.Update();

            // Brush
            if (IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON)) {
                for (int i = 0; i < BrushDensity; i++) {
                    Vector2 MousePos = GetMousePosition() / 2.0f;
                    Vector2 Position = MousePos + new Vector2(RNG.Range(-BrushSize, BrushSize), RNG.Range(-BrushSize, BrushSize));

                    Type t = Type.GetType("SharpSand." + BrushElement)!;
                    Matrix.Set(Position, (Element)Activator.CreateInstance(t, Position)!);
                }
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
            ImageClearBackground(ref BufferImage, Color.BLACK);

            foreach (Element e in Matrix.Elements) {
                if (e is not Air) {
                    Color c = e.Color;
                    // if (e.Settled) c = Color.MAGENTA;
                    ImageDrawPixel(ref BufferImage, (int)e.Position.X, (int)e.Position.Y, c);
                }
            }

            UpdateTexture(BufferTexture, BufferImage.data);

            // Draw
            BeginDrawing();
            ClearBackground(Color.BLACK);

            DrawTexturePro(BufferTexture, new Rectangle(0, 0, MatrixSize.X, MatrixSize.Y), new Rectangle(0, 0, ScreenSize.X, ScreenSize.Y), Vector2.Zero, 0, Color.WHITE);

            DrawFPS(10, 10);

            EndDrawing();
        }

        // Exit
        CloseWindow();
        Console.WriteLine("[SYSTEM] Simulation exited successfully");
    }
}
