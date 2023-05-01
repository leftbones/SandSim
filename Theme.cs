using Raylib_cs;

namespace SharpSand;


public struct Theme {
    public Color BackgroundColor;
    public Color ForegroundColor;
    public Color ShadowColor;
    public Color WindowColor = new Color(50, 50, 50, 225);
    public Color HoverHighlight = new Color(239, 71, 111, 150);

    public Theme() : this(Color.BLACK, Color.WHITE, Color.DARKGRAY) { }
    public Theme(Color background, Color foreground, Color shadow) {
        BackgroundColor = background;
        ForegroundColor = foreground;
        ShadowColor = shadow;
    }
}