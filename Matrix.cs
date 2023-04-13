using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Matrix {
    public Vector2 Size { get; private set; }
    public Element[,] Elements { get; private set; }

    public int Tick { get; private set; }

    public Matrix(Vector2 size) {
        Size = size;
        Tick = 0;

        // Create elements array and fill with air
        Elements = new Element[(int)Size.X, (int)Size.Y];
        for (int x = 0; x < Size.X; x++) {
            for (int y = 0; y < Size.Y; y++) {
                Elements[x, y] = new Air(new Vector2(x, y));
            }
        }
    }

    // Update all non-air elements in the matrix
    public void Update() {
        if (Tick % 2 == 0) {
            for (int y = (int)Size.Y - 1; y >= 0; y--) {
                for (int x = 0; x < (int)Size.X; x++) {
                    Element e = Get(new Vector2(x, y));
                    if (e is not Air)
                        e.Update(this);
                }
            }
        } else {
            for (int y = (int)Size.Y - 1; y >= 0; y--) {
                for (int x = (int)Size.X - 1; x >= 0; x--) {
                    Element e = Get(new Vector2(x, y));
                    if (e is not Air)
                        e.Update(this);
                }
            }
        }
        Tick++;
    }

    // Set an element in the matrix (and update the element's position)
    public void Set(Vector2 position, Element element) {
        if (InBounds(position)) {
            Elements[(int)position.X, (int)position.Y] = element;
            element.Position = position;
        }
    }

    // Get an element from the matrix
    public Element Get(Vector2 position) {
        return Elements[(int)position.X, (int)position.Y];
    }

    // Swap the position of two elements in the matrix
    public bool Swap(Vector2 pos1, Vector2 pos2) {
        if (IsValid(pos2)) {
            Element e1 = Get(pos1);
            Element e2 = Get(pos2);
            Set(pos2, e1);
            Set(pos1, e2);
            return true;
        }
        return false;
    }

    // Check if a position is valid (in bounds and empty)
    public bool IsValid(Vector2 position) {
        if (InBounds(position))
            return IsEmpty(position);
        return false;
    }

    // Check if a position is in bounds
    public bool InBounds(Vector2 position) {
        return (int)position.X >= 0 && (int)position.X < (int)Size.X && (int)position.Y >= 0 && (int)position.Y < (int)Size.Y;
    }

    // Check if a position is empty (contains air)
    public bool IsEmpty(Vector2 position) {
        return Get(position) is Air;
    }
}