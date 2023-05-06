using System.Numerics;
using Newtonsoft.Json;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.TraceLogLevel;

namespace SharpSand;

class Program {
    static unsafe void Main(string[] args) {
        ////
        // Init
        Console.WriteLine("[SYSTEM] Simulation initialized");
        Vector2i ScreenSize = new Vector2i(1280, 720);
        int Scale = 4;

        Console.WriteLine(String.Format("[SYSTEM] Screen size is {0}x{1}, scale is {2}", ScreenSize.X, ScreenSize.Y, Scale));

        SetTraceLogLevel(LOG_WARNING | LOG_ERROR | LOG_FATAL);
        InitWindow(ScreenSize.X, ScreenSize.Y, "SandSim");
        SetTargetFPS(200);
        SetExitKey(KeyboardKey.KEY_NULL);


        ////
        // Setup

        // Load Settings
        var Settings = new Settings();
        try {
            Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"))!;
            Console.WriteLine("[SYSTEM] Settings restored");
        } catch {
            Console.WriteLine("[ERROR] No settings.json found, default settings restored");
        }

        // Autoload Matrix
        var Matrix = new Matrix(ScreenSize, Scale);
        if (Settings.LoadOnStart) {
            try {
                string input = File.ReadAllText("autosave.sav");
                int x = 0, y = 0;
                foreach (var row in input.Split('\n')) {
                    y = 0;
                    foreach (var col in row.Trim().Split(" ")) {
                        Vector2i Pos = new Vector2i(x, y);
                        Element NewElement;

                        if (col.Trim() == "") {
                            continue;
                        } else if (col.Trim() == "-1") {
                            NewElement = new Air(Pos);
                        } else {
                            Type t = Type.GetType("SharpSand." + Atlas.GetNameFromID(int.Parse(col.Trim())))!;
                            NewElement = (Element)Activator.CreateInstance(t, Pos)!;
                        }

                        Matrix.Set(Pos, NewElement);
                        y++;
                    }
                    x++;
                }
                Console.WriteLine("[SYSTEM] Auto load successful");
            } catch {
                Console.WriteLine("[ERROR] No autosave.sav found, new matrix created");
            }
        }

        // Apply matrix settings
        Matrix.DestroyOutOfBounds = Settings.DestroyOutOfBounds;

        Console.WriteLine("[SYSTEM] Matrix initialized");

        // Matrix Buffer + Texture
        Image BufferImage = GenImageColor(Matrix.Size.X, Matrix.Size.Y, Color.BLACK);
        Texture2D BufferTexture = LoadTextureFromImage(BufferImage);

        // Theme + Interface
        var Theme = new Theme();
        var Interface = new Interface(ScreenSize, Matrix.Size, Scale, Theme);

        Console.WriteLine("[SYSTEM] Init complete");


        ////
        // Main Loop
        while (!WindowShouldClose()) {
            ////
            // Input

            // Escape closes all interface windows, or closes the game if no windows are open
            if (IsKeyPressed(KeyboardKey.KEY_ESCAPE)) {
                if (Interface.MenuActive)
                    Interface.ToggleMenu();
                else if (Interface.SettingsActive)
                    Interface.ToggleSettings();
                else
                    break;
            }


            // Open Element Menu            
            if (!Interface.SettingsActive && IsKeyPressed(KeyboardKey.KEY_E))
                Interface.ToggleMenu();

            // Open Settings Menu
            if (IsKeyPressed(KeyboardKey.KEY_F1))
                Interface.ToggleSettings();

            if (IsKeyPressed(KeyboardKey.KEY_F2))
                Settings.CycleSimulationSpeed();

            if (IsKeyPressed(KeyboardKey.KEY_F3)) {
                Settings.SaveOnClose = !Settings.SaveOnClose;
                Settings.LoadOnStart = !Settings.LoadOnStart;
                Console.WriteLine("[SYSTEM] Auto save/load set to " + Settings.SaveOnClose.ToString().ToUpper());
            }

            if (IsKeyPressed(KeyboardKey.KEY_F4)) {
                Settings.DestroyOutOfBounds = !Settings.DestroyOutOfBounds;
                Matrix.DestroyOutOfBounds = !Matrix.DestroyOutOfBounds;
                Matrix.UnsettleAll();
                Console.WriteLine("[SYSTEM] Destroy out of bounds set to " + Settings.DestroyOutOfBounds.ToString().ToUpper());
            }

            if (IsKeyPressed(KeyboardKey.KEY_F5)) {
                Matrix.Reset();
                Console.WriteLine("[SYSTEM] Simulation reset");
            }

            if (IsKeyPressed(KeyboardKey.KEY_F6)) {
                Settings.WeatherEnabled = !Settings.WeatherEnabled;
                Console.WriteLine("[SYSTEM] Weather simulation set to " + Settings.WeatherEnabled.ToString().ToUpper());
            }

            if (IsKeyPressed(KeyboardKey.KEY_F7))
                Settings.CycleWeatherElement();

            if (IsKeyPressed(KeyboardKey.KEY_F8)) {
                Settings.ShowElementName = !Settings.ShowElementName;
                Console.WriteLine("[SYSTEM] Element name display set to " + Settings.ShowElementName.ToString().ToUpper());
            }

            if (IsKeyPressed(KeyboardKey.KEY_F9)) {
                Settings.DrawChunkBorders = !Settings.DrawChunkBorders;
                Console.WriteLine("[SYSTEM] Chunk border drawing set to " + Settings.DrawChunkBorders.ToString().ToUpper());

            }

            if (IsKeyPressed(KeyboardKey.KEY_F10)) {
                Settings.HideSpawnerRemover = !Settings.HideSpawnerRemover;
                Console.WriteLine("[SYSTEM] Spawner/Remover hiding set to " + Settings.HideSpawnerRemover.ToString().ToUpper());
            }

            if (IsKeyPressed(KeyboardKey.KEY_F11)) {

            }

            if (IsKeyPressed(KeyboardKey.KEY_F12)) {

            }

            if (IsKeyPressed(KeyboardKey.KEY_SPACE))
                Matrix.Active = !Matrix.Active;

            if (IsKeyPressed(KeyboardKey.KEY_T)) {
                if (!Matrix.Active) {
                    Matrix.Active = true;
                    Matrix.StepTick = true;
                }
            }

            if (IsKeyPressed(KeyboardKey.KEY_Q)) {
                Interface.PickElement(Matrix);
            }


            ////
            // Update
            Interface.Update(Matrix);
            Matrix.Update();

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
            ImageClearBackground(ref BufferImage, Theme.BackgroundColor);

            foreach (Element e in Matrix.Elements) {
                if (e.GetType() != typeof(Air)) {
                    e.AlreadyStepped = false;

                    // Elements with "DrawWhenSelected" enabled are only drawn if currently selected (duh)
                    if (e.DrawWhenSelected && e.ToString() != Interface.DrawingTools.BrushElement) {
                        Type E_Type = e.GetType();
                        if (Settings.HideSpawnerRemover && (E_Type == typeof(Spawner) || E_Type == typeof(Remover)))
                            continue;
                    }

                    Color c = e.Color;

                    // Inactive Overlay
                    if (Settings.DebugOverlayInactive && e.Inactive) {
                        c = new Color(
                            Math.Min(e.Color.r + 100, 255),
                            Math.Max(e.Color.g - 50, 0),
                            Math.Max(e.Color.b - 50, 0),
                            e.Color.a
                        );
                    }

                    // Settled Overlay
                    if (Settings.DebugOverlaySettled && e.Settled) {
                        c = new Color(
                            Math.Max(e.Color.r - 50, 0),
                            Math.Max(e.Color.g - 50, 0),
                            Math.Min(e.Color.b + 100, 255),
                            e.Color.a
                        );
                    }

                    ImageDrawPixel(ref BufferImage, e.Position.X, e.Position.Y, c);
                }
            }

            UpdateTexture(BufferTexture, BufferImage.data);


            ////
            // Draw
            BeginDrawing();

            ClearBackground(Theme.BackgroundColor);

            DrawTexturePro(BufferTexture, new Rectangle(0, 0, Matrix.Size.X, Matrix.Size.Y), new Rectangle(0, 0, ScreenSize.X, ScreenSize.Y), Vector2.Zero, 0, Color.WHITE);

            // Draw Chunk Borders
            if (Settings.DrawChunkBorders) {
                foreach (Chunk Chunk in Matrix.Chunks) {
                    Chunk.DebugDraw();
                }
            }

            // Draw name of element under cursor
            if (Settings.ShowElementName) {
                Vector2i MousePos = Interface.DrawingTools.GetMousePos();
                if (Matrix.InBounds(MousePos)) {
                    Element e = Matrix.Get(MousePos);
                    string ElementName = e.ToString()!.Split(".")[1];
                    if (e.OnFire) ElementName += " (on fire)";
                    ElementName += String.Format(" (Temp: {0})", e.ActiveTemp);

                    Interface.DrawingTools.DrawTextShadow(ElementName, (MousePos * Scale) + new Vector2i(5, 5));
                }
            }

            // Interface
            Interface.Draw(Matrix);

            EndDrawing();
        }

        ////
        // Exit
        CloseWindow();

        // Save settings
        string SettingsOutput = JsonConvert.SerializeObject(Settings);
        File.WriteAllText("settings.json", SettingsOutput);
        Console.WriteLine("[SYSTEM] Settings saved to settings.json");

        // Autosave Matrix
        if (Settings.SaveOnClose) {
            using (var sw = new StreamWriter("autosave.sav", append: false)) {
                int x, y;
                for (x = 0; x < Matrix.Size.X; x++) {
                    for (y = 0; y < Matrix.Size.Y; y++) {
                        int ID = Atlas.GetIDFromName(Matrix.Elements[x, y].ToString()!.Split(".")[1]);
                        sw.Write(ID + " ");
                    }
                    sw.Write("\n");
                }

                sw.Flush();
                sw.Close();
            }
            Console.WriteLine("[SYSTEM] Game saved to autosave.sav");
        }

        Console.WriteLine("[SYSTEM] Simulation exited successfully");
    }
}
