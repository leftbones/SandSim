using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.TraceLogLevel;

namespace SharpSand;

class Program {
    static unsafe void Main(string[] args) {
        // Init
        Vector2 ScreenSize = new Vector2(800, 600);
        Vector2 MatrixSize = new Vector2(400, 300);

        SetTraceLogLevel(LOG_WARNING | LOG_ERROR | LOG_FATAL);
        InitWindow((int)ScreenSize.X, (int)ScreenSize.Y, "Sand");
        SetTargetFPS(144);

        Matrix Matrix = new Matrix(new Vector2((int)MatrixSize.X, (int)MatrixSize.Y));

        Image BufferImage = GenImageColor((int)MatrixSize.X, (int)MatrixSize.Y, Color.BLACK);
        Texture2D BufferTexture = LoadTextureFromImage(BufferImage);

        // Brush
        int BrushSize = 5;
        int BrushDensity = 5;
        string BrushElement = "Dirt";

        // Spout
        int SpoutSize = 5;
        int SpoutDensity = 1;
        string SpoutElement = "Water";

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
            for (int i = 0; i < SpoutDensity; i++) {
                Vector2 SpoutPos = new Vector2((MatrixSize.X / 2.0f) + RNG.Range(-SpoutSize, SpoutSize), SpoutSize + RNG.Range(-SpoutSize, SpoutSize));

                Type t = Type.GetType("SharpSand." + SpoutElement)!;
                Matrix.Set(SpoutPos, (Element)Activator.CreateInstance(t, SpoutPos)!);
            }

            // Texture
            ImageClearBackground(ref BufferImage, Color.BLACK);

            foreach (Element e in Matrix.Elements) {
                if (e is not Air) {
                    ImageDrawPixel(ref BufferImage, (int)e.Position.X, (int)e.Position.Y, e.Color);
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
    }
}
