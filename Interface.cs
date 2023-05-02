using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SharpSand;

// TODO
// - Add indicator for current element selected
// - Add element picker with Q key
// - Add zoom lens by holding some key, maybe shift?

class Interface {
    public Vector2i ScreenSize { get; private set; }
    public Vector2i MatrixSize { get; private set; }
    public int Scale { get; private set; }
    public Theme Theme { get; private set; }

    public Vector2i MenuPos { get; private set; }
    public Vector2i TargetPos { get; private set; }
    public Vector2i MenuSize { get { return new Vector2i(200, ScreenSize.Y); } }

    public bool Active { get; private set; } = false;

    public DrawingTools DrawingTools { get; private set; }

    private List<Texture2D> ElementTextures = new List<Texture2D>();
    private List<ElementListItem> ElementListItems = new List<ElementListItem>();

    private int LastFPS = 0;

    public Interface(Vector2i screen_size, Vector2i matrix_size, int scale, Theme theme) {
        ScreenSize = screen_size;
        MatrixSize = matrix_size;
        Scale = scale;
        Theme = theme;

        MenuPos = new Vector2i(ScreenSize.X, 0);
        TargetPos = MenuPos;

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

        DrawingTools = new DrawingTools(ScreenSize, MatrixSize, Theme, ElementTextures);
    }

    public void HandleInput(Matrix matrix) {
        // Update mouse position
        DrawingTools.MousePosB = DrawingTools.MousePosA;
        DrawingTools.MousePosA = DrawingTools.GetMousePos();

        // Prevent painting while the cursor is over the menu while it is open
        if (!Active || GetMouseX() < ScreenSize.X - MenuSize.X) {

            // Painting
            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
                DrawingTools.PaintLine(matrix, DrawingTools.MousePosA, DrawingTools.MousePosB, DrawingTools.BrushElement, DrawingTools.BrushSize, DrawingTools.BrushDensity);
        }

        // Erasing
        if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
            DrawingTools.PaintLine(matrix, DrawingTools.MousePosA, DrawingTools.MousePosB, "SharpSand.Air", DrawingTools.BrushSize, erase: true);

        // Brush Size (Hold LSHIFT for faster scrolling)
        int MouseWheelAmt = (int)GetMouseWheelMove();
        if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
            MouseWheelAmt *= Scale;

        DrawingTools.BrushSize -= MouseWheelAmt;
        DrawingTools.BrushSize = Math.Clamp(DrawingTools.BrushSize, DrawingTools.MinBrushSize, DrawingTools.MaxBrushSize);

        // Brush Density
        if (IsKeyPressed(KeyboardKey.KEY_W) && DrawingTools.BrushDensityModifier < 1.0m)
            DrawingTools.BrushDensityModifier += 0.1m;

        if (IsKeyPressed(KeyboardKey.KEY_S) && DrawingTools.BrushDensityModifier > 0.0m)
            DrawingTools.BrushDensityModifier -= 0.1m;

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

        // If interface is not open, break now
        if (!Active && MenuPos == TargetPos)
            return;

        // Move interface if it's not fully open or closed
        if (MenuPos.X > TargetPos.X)
            MenuPos = new Vector2i(MenuPos.X - (MenuSize.X / 10), MenuPos.Y);
        else if (MenuPos.X < TargetPos.X)
            MenuPos = new Vector2i(MenuPos.X + (MenuSize.X / 10), MenuPos.Y);

        // Update all element list items    
        foreach (ElementListItem ListItem in ElementListItems) {
            ListItem.Update(DrawingTools);
        }
    }

    public void Draw(Matrix matrix) {
        if (Active || MenuPos != TargetPos) {
            // Background
            DrawRectangle(MenuPos.X, MenuPos.Y, MenuSize.X, MenuSize.Y, Theme.WindowColor);

            // Element List
            foreach (ElementListItem ListItem in ElementListItems) {
                bool selected = ListItem.Index == DrawingTools.ElementIndex;
                ListItem.Draw(Theme, DrawingTools, MenuPos, selected);
            }
        }

        // HUD Elements
        if (GetMousePosition().X < MenuPos.X)
            DrawingTools.DrawBrushIndicator();
        else
            DrawCursor();

        DrawHUD();
        if (!Active) DrawFPS();

        if (!matrix.Active) {
            string PauseText = "[ PAUSED ]";
            var CenterPos = new Vector2i((ScreenSize.X / 2) - (MeasureText(PauseText, 20) / 2), 5);
            DrawingTools.DrawTextShadow(PauseText, CenterPos);
        }
    }

    public void Toggle() {
        if (Active) {
            Active = false;
            TargetPos = new Vector2i(ScreenSize.X, 0);
        } else {
            Active = true;
            TargetPos = new Vector2i(ScreenSize.X - MenuSize.X, 0);
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

    public void Draw(Theme theme, DrawingTools drawing_tools, Vector2i origin, bool selected) {
        ItemRec = new Rectangle(origin.X + ClickBox.x, origin.Y + ClickBox.y, ClickBox.width, ClickBox.height);

        if (selected)
            DrawRectangle(origin.X, origin.Y + (int)ClickBox.y, (int)ClickBox.width, (int)ClickBox.height, theme.SelectHighlight);
        else if (CheckCollisionPointRec(GetMousePosition(), ItemRec))
            DrawRectangle(origin.X, origin.Y + (int)ClickBox.y, (int)ClickBox.width, (int)ClickBox.height, theme.HoverHighlight);

        DrawTexture(PreviewTexture, origin.X + Position.X, origin.Y + Position.Y, Color.WHITE);
        drawing_tools.DrawTextShadow(DisplayName, new Vector2i(origin.X + Position.X + 50, origin.Y + Position.Y + 1));
    }

    public void Toggle(Vector2i origin) {
        Active = !Active;
    }
}