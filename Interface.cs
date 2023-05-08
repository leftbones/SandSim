using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpSand;

// TODO
// - Add zoom lens by holding some key, maybe shift?

class Interface {
    public Vector2i ScreenSize { get; private set; }
    public Vector2i MatrixSize { get; private set; }
    public int Scale { get; private set; }
    public Theme Theme { get; private set; }

    public bool MenuActive { get; private set; } = false;
    public Vector2i MenuPos { get; private set; }
    public Vector2i MenuTargetPos { get; private set; }
    public Vector2i MenuSize { get { return new Vector2i(300, ScreenSize.Y); } }
    public int MenuScrollOffset { get; set; } = 0;
    public int MaxMenuItems { get { return ScreenSize.Y / 30; } }

    public bool SettingsActive { get; private set; } = false;
    public Vector2i SettingsPos { get; private set; }
    public Vector2i SettingsTargetPos { get; private set; }
    public Vector2i SettingsSize { get; private set; } = new Vector2i(600, 585);

    public DrawingTools DrawingTools { get; private set; }

    private List<string> HelpText = new List<string>() {
        "[Controls]",
        "<Mouse Left/Right> Paint/Erase elements",
        "<Mouse Wheel> Adjust brush size",
        "<W/S> Increase/decrease brush density",
        "<Space> Pause/play simulation",
        "<T> Advance one tick (while paused)",
        "<O> Toggle paint overlap",
        "<Q> Pick element under the cursor",
        "<E> Open the element selection menu",
        "",
        "[Hotkeys]",
        "<F2> Cycle simulation speed",
        "<F3> Toggle auto save/load",
        "<F4> Toggle world borders",
        "<F5> Reset world",
        "<F6> Toggle weather",
        "<F7> Cycle weather element",
        "<F8> Toggle element name display",
        "<F9> Toggle chunk border display",
        "<F10> Toggle Spawner/Remover hiding",
        "<F11> Toggle inactive element overlay (debug)",
        "<F12> Toggle settled element overlay (debug)"
    };

    private List<Texture2D> ElementTextures = new List<Texture2D>();
    private List<ElementListItem> ElementListItems = new List<ElementListItem>();

    private int LastFPS = 0;

    public Interface(Vector2i screen_size, Vector2i matrix_size, int scale, Theme theme) {
        ScreenSize = screen_size;
        MatrixSize = matrix_size;
        Scale = scale;
        Theme = theme;

        MenuPos = new Vector2i(ScreenSize.X, 0);
        MenuTargetPos = MenuPos;

        SettingsPos = new Vector2i((ScreenSize.X / 2) - (SettingsSize.X / 2), ScreenSize.Y);
        SettingsTargetPos = SettingsPos;

        // Generate element preview textures + create element list items
        for (int i = 0; i < Atlas.Entries.Count; i++) {
            string ElementName = "SharpSand." + Atlas.Entries.ElementAt(i).Key;
            string DisplayName = Atlas.Entries.ElementAt(i).Value.DisplayName;

            Texture2D ElementTexture = Utility.GetElementTexture(ElementName, 40, 20, Scale);
            ElementTextures.Add(ElementTexture);

            var Position = new Vector2i(5, 5 + (30 * i));
            var ClickBox = new Rectangle(0, Position.Y - 5, MenuSize.X, 30);
            var NewListItem = new ElementListItem(i, Position, DisplayName, ElementName, ElementTexture, ClickBox);
            ElementListItems.Add(NewListItem);
        }

        DrawingTools = new DrawingTools(ScreenSize, MatrixSize, Scale, Theme, ElementTextures);
    }

    public void HandleInput(Matrix matrix) {
        // Update mouse position
        DrawingTools.MousePosB = DrawingTools.MousePosA;
        DrawingTools.MousePosA = DrawingTools.GetMousePos();

        // Prevent painting while the cursor is over the menu while it is open
        if (!MenuActive || GetMouseX() < ScreenSize.X - MenuSize.X) {

            // Painting
            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
                DrawingTools.PaintLine(matrix, DrawingTools.MousePosA, DrawingTools.MousePosB, DrawingTools.BrushElement, DrawingTools.BrushSize, DrawingTools.BrushDensity);
        }

        // Erasing
        if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
            DrawingTools.PaintLine(matrix, DrawingTools.MousePosA, DrawingTools.MousePosB, "SharpSand.Air", DrawingTools.BrushSize, erase: true);

        // Mouse Wheel (Brush Size + Menu Scrolling)
        int MouseWheelAmt = (int)GetMouseWheelMove();
        if (!MenuActive || GetMouseX() < ScreenSize.X - MenuSize.X) {
            if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
                MouseWheelAmt *= Scale;

            DrawingTools.BrushSize -= MouseWheelAmt;
            DrawingTools.BrushSize = Math.Clamp(DrawingTools.BrushSize, DrawingTools.MinBrushSize, DrawingTools.MaxBrushSize);
        } else {
            MenuScrollOffset -= MouseWheelAmt;
            MenuScrollOffset = Math.Clamp(MenuScrollOffset, 0, 99);
        }

        // Brush Density
        if (IsKeyPressed(KeyboardKey.KEY_W) && DrawingTools.BrushDensityModifier < 1.0m) {
            Decimal Amount = IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) ? 0.1m : 0.01m;
            DrawingTools.BrushDensityModifier += Amount;
        }

        if (IsKeyPressed(KeyboardKey.KEY_S) && DrawingTools.BrushDensityModifier > 0.01m) {
            Decimal Amount = IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) ? 0.1m : 0.01m;
            DrawingTools.BrushDensityModifier -= Amount;
        }

        DrawingTools.BrushDensityModifier = Math.Clamp(DrawingTools.BrushDensityModifier, 0.01m, 1.0m);

        // Toggle PaintOver
        if (IsKeyPressed(KeyboardKey.KEY_O))
            DrawingTools.PaintOver = !DrawingTools.PaintOver;
    }

    public void Update(Matrix matrix) {
        // Handle user input
        HandleInput(matrix);

        // Update FPS tracker if unpaused
        if (matrix.Active)
            LastFPS = GetFPS();

        // If any interface window is not open, break now
        if ((!MenuActive && MenuPos == MenuTargetPos) && (!SettingsActive && SettingsPos == SettingsTargetPos))
            return;

        // Move interface windows if they are not fully open or closed
        if (MenuPos.X > MenuTargetPos.X)
            MenuPos = new Vector2i(MenuPos.X - Math.Abs((MenuTargetPos.X - MenuPos.X) / 10), MenuPos.Y);
        else if (MenuPos.X < MenuTargetPos.X)
            MenuPos = new Vector2i(MenuPos.X + Math.Abs((MenuTargetPos.X - MenuPos.X) / 10), MenuPos.Y);

        if (SettingsPos.Y > SettingsTargetPos.Y)
            SettingsPos = new Vector2i(SettingsPos.X, SettingsPos.Y - Math.Abs((SettingsTargetPos.Y - SettingsPos.Y) / 10));
        else if (SettingsPos.Y < SettingsTargetPos.Y)
            SettingsPos = new Vector2i(SettingsPos.X, SettingsPos.Y + Math.Abs((SettingsTargetPos.Y - SettingsPos.Y) / 10));

        // Update all element list items    
        foreach (ElementListItem ListItem in ElementListItems) {
            ListItem.Update(DrawingTools);
        }
    }

    public void Draw(Matrix matrix) {
        // Help Indicator
        string HelpShortcut = "<F1> Help";
        DrawingTools.DrawTextShadow(HelpShortcut, new Vector2i((ScreenSize.X / 2) - (MeasureText(HelpShortcut, 20) / 2), ScreenSize.Y - 25));

        // Element Menu
        if (MenuActive || MenuPos != MenuTargetPos) {
            // Background
            DrawRectangle(MenuPos.X, MenuPos.Y, MenuSize.X, MenuSize.Y, Theme.WindowColor);

            // Element List
            for (int i = MenuScrollOffset; i < ElementListItems.Count; i++) {
                var ListItem = ElementListItems[i];
                bool selected = ListItem.Index == DrawingTools.ElementIndex;
                ListItem.Draw(Theme, DrawingTools, MenuPos, -MenuScrollOffset, selected);
            }
        }

        // Settings Menu
        if (SettingsActive || SettingsPos != SettingsTargetPos) {
            // Background
            DrawRectangle(SettingsPos.X, SettingsPos.Y, SettingsSize.X, SettingsSize.Y, Theme.WindowColor);

            // Header
            string HeaderText = "[Help]";
            DrawingTools.DrawTextShadow(HeaderText, new Vector2i(SettingsPos.X + (SettingsSize.X / 2) - (MeasureText(HeaderText, 20) / 2), SettingsPos.Y + 10));

            // Settings
            for (int i = 0; i < HelpText.Count; i++) {
                DrawingTools.DrawTextShadow(HelpText[i], new Vector2i(SettingsPos.X + 10, SettingsPos.Y + 30 + (25 * (i + 1))));
            }
        }

        // HUD Elements
        if (GetMousePosition().X < MenuPos.X)
            DrawingTools.DrawBrushIndicator();
        else
            DrawCursor();

        DrawHUD();
        if (!MenuActive) DrawFPS();

        if (!matrix.Active) {
            string PauseText = "[PAUSED]";
            var CenterPos = new Vector2i((ScreenSize.X / 2) - (MeasureText(PauseText, 20) / 2), 5);
            DrawingTools.DrawTextShadow(PauseText, CenterPos);
        }
    }

    public void ToggleMenu() {
        if (MenuActive) {
            MenuActive = false;
            MenuTargetPos = new Vector2i(ScreenSize.X + 10, 0);
        } else {
            MenuActive = true;
            MenuTargetPos = new Vector2i(ScreenSize.X - MenuSize.X, 0);
        }
    }

    public void ToggleSettings() {
        if (MenuActive)
            ToggleMenu();

        if (SettingsActive) {
            SettingsActive = false;
            SettingsTargetPos = new Vector2i(SettingsTargetPos.X, ScreenSize.Y + 10);
        } else {
            SettingsActive = true;
            SettingsTargetPos = new Vector2i(SettingsTargetPos.X, (ScreenSize.Y / 2) - (SettingsSize.Y / 2));
        }
    }

    // Pick the element under the cursor and set it to the brush element
    public void PickElement(Matrix matrix) {
        Vector2i MousePos = DrawingTools.GetMousePos();
        if (matrix.InBounds(MousePos)) {
            Element e = matrix.Get(MousePos);
            if (e.GetType() == typeof(Air))
                return;
            DrawingTools.ElementIndex = Atlas.Entries.Keys.ToList().IndexOf(e.ToString()!.Split(".")[1]);
        }
    }

    // Draw all of the HUD elements
    public void DrawHUD() {
        DrawingTools.DrawBrushElement();
        DrawingTools.DrawTextShadow("Size: " + DrawingTools.BrushSize, new Vector2i(5, 30));
        DrawingTools.DrawTextShadow("Density: " + DrawingTools.BrushDensityModifier, new Vector2i(5, 50));
    }

    // Draw the current FPS to the upper right corner of the screen
    public void DrawFPS(Color? color=null) {
        color = color ?? Theme.ForegroundColor;
        string FPS = String.Format("{0} FPS", LastFPS);
        DrawingTools.DrawTextShadow(FPS, new Vector2i(ScreenSize.X - (MeasureText(FPS, 20) + 5), 5), Vector2i.One, 20, color);
    }

    // Draw the cursor used for the menus
    public void DrawCursor() {
        int MX = GetMouseX();
        int MY = GetMouseY();

        DrawCircleLines(MX + 1, MY + 1, 5.0f, Theme.ShadowColor);
        DrawCircle(MX + 1, MY + 1, 2.0f, Theme.ShadowColor);
        DrawCircleLines(MX, MY, 5.0f, Theme.ForegroundColor);
        DrawCircle(MX, MY, 2.0f, Theme.ForegroundColor);
    }
}


////
// Element List Item
class ElementListItem {
    public int Index { get; private set; }
    public Vector2i Position { get; private set; }
    public string DisplayName { get; private set; }
    public string ElementName { get; private set; }
    public Texture2D PreviewTexture { get; private set; }

    public Rectangle ClickBox { get; private set; }
    public bool Active { get; private set; }

    private Rectangle ItemRec;

    public ElementListItem(int index, Vector2i position, string display_name, string element_name, Texture2D preview_texture, Rectangle click_box) {
        Index = index;
        Position = position;
        DisplayName = display_name;
        ElementName = element_name;
        PreviewTexture = preview_texture;
        ClickBox = click_box;
    }

    public void Update(DrawingTools drawing_tools) {
        if (CheckCollisionPointRec(GetMousePosition(), ItemRec) && IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) {
            drawing_tools.ElementIndex = Index;
        }
    }

    public void Draw(Theme theme, DrawingTools drawing_tools, Vector2i origin, int scroll_offset, bool selected) {
        ItemRec = new Rectangle(origin.X + ClickBox.x, (origin.Y + ClickBox.y) + (30 * scroll_offset), ClickBox.width, ClickBox.height);

        if (selected)
            DrawRectangle(origin.X, origin.Y + (int)ClickBox.y + (30 * scroll_offset), (int)ClickBox.width, (int)ClickBox.height, theme.SelectHighlight);
        else if (CheckCollisionPointRec(GetMousePosition(), ItemRec))
            DrawRectangle(origin.X, origin.Y + (int)ClickBox.y + (30 * scroll_offset), (int)ClickBox.width, (int)ClickBox.height, theme.HoverHighlight);

        DrawTexture(PreviewTexture, origin.X + Position.X, origin.Y + Position.Y + (30 * scroll_offset), Color.WHITE);
        drawing_tools.DrawTextShadow(DisplayName, new Vector2i(origin.X + Position.X + 50, origin.Y + Position.Y + 1 + (30 * scroll_offset)));
    }

    public void Toggle(Vector2i origin) {
        Active = !Active;
    }
}