using Raylib_cs;

namespace SharpSand;


struct Theme {
    public Color BackgroundColor = Color.BLACK;
    public Color ForegroundColor = Color.WHITE;
    public Color ShadowColor = Color.DARKGRAY;

    public Theme() : this(Color.BLACK, Color.WHITE, Color.DARKGRAY) { }
    public Theme(Color background, Color foreground, Color shadow) {
        BackgroundColor = background;
        ForegroundColor = foreground;
        ShadowColor = shadow;
    }
}