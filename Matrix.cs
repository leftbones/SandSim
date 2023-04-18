using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Matrix {
    public Vector2 Size { get; private set; }
    public Element[,] Elements { get; private set; }

    public int Tick { get; private set; }
    public bool DestroyOutOfBounds { get; set; } = true;

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
                    if (e is not Air && !e.AlreadyStepped) {
                        e.LastPosition = e.Position;
                        e.Step(this);
                        e.Tick(this);
                    }
                }
            }
        } else {
            for (int y = (int)Size.Y - 1; y >= 0; y--) {
                for (int x = (int)Size.X - 1; x >= 0; x--) {
                    Element e = Get(new Vector2(x, y));
                    if (e is not Air && !e.AlreadyStepped) {
                        e.LastPosition = e.Position;
                        e.Step(this);
                        e.Tick(this);
                    }
                }
            }
        }
        Tick++;
    }

    // Get an element from the matrix
    public Element Get(Vector2 position) {
        return Elements[(int)position.X, (int)position.Y];
    }

    // Set an element in the matrix (and update the element's position)
    public bool Set(Vector2 position, Element element) {
        if (InBounds(position)) {
            Elements[(int)position.X, (int)position.Y] = element;
            element.Position = position;
            element.LastDirection = Direction.GetMovementDirection(element.LastPosition, element.Position);
            return true;
        }
        return false;
    }

    // Set an element in the matrix without checking if the position is in bounds
    public bool FastSet(Vector2 position, Element element) {
        Elements[(int)position.X, (int)position.Y] = element;
        return true;
    }

    // Swap the position of two elements in the matrix if the destination is in bounds
    public bool Swap(Vector2 pos1, Vector2 pos2) {
        if (InBounds(pos2)) {
            Element e1 = Get(pos1);
            Element e2 = Get(pos2);
            Set(pos2, e1);
            Set(pos1, e2);
            e1.AlreadyStepped = true;
            return true;
        } else if (DestroyOutOfBounds) {
            Set(pos1, new Air(pos1));
            return true;
        }
        return false;
    }

    // Swap the position of two elements in the matrix if the second is empty
    public bool SwapIfEmpty(Vector2 pos1, Vector2 pos2) {
        if (!InBounds(pos2)) {
            if (DestroyOutOfBounds) {
                Set(pos1, new Air(pos1));
                return true;
            }
            return false;
        }

        if (IsEmpty(pos2))
            return Swap(pos1, pos2);
        return false;
    }

    // Swap the position of two elements if the element at pos2 matches the specified type
    public bool SwapIfType(Vector2 pos1, Vector2 pos2, ElementType type) {
        if (InBounds(pos2)) {
            Element e = Get(pos2);
            if (e.Type == type)
                return Swap(pos1, pos2);
            return false;
        } else if (DestroyOutOfBounds) {
            Set(pos1, new Air(pos1));
            return true;
        }
        return false;
    }

    // Swap the position of two elements if the element at pos2 matches the specified type or pos2 is empty (contains air)
    public bool SwapIfTypeOrEmpty(Vector2 pos1, Vector2 pos2, ElementType type) {
        if (InBounds(pos2)) {
            Element e = Get(pos2);
            if (e.Type == type || e is Air)
                return Swap(pos1, pos2);
            return false;
        } else if (DestroyOutOfBounds) {
            Set(pos1, new Air(pos1));
            return true;
        }
        return false;
    }

    // Swap the position of two elements if the element at pos1 is less dense than the element at pos2 (Liquid/Gas only)
    public bool SwapIfLessDense(Vector2 pos1, Vector2 pos2) {
        List<ElementType> ValidTypes = new List<ElementType>() { ElementType.Liquid, ElementType.Gas };
        if (InBounds(pos2)) {
            Element e1 = Get(pos1);
            Element e2 = Get(pos2);

            if (!ValidTypes.Contains(e1.Type) || !ValidTypes.Contains(e2.Type))
                return false;

            if (e1.Density < e2.Density)
                return Swap(pos1, pos2);
        } else if (DestroyOutOfBounds) {
            Set(pos1, new Air(pos1));
            return true;
        }
        return false;
    }

    // Swap the position of two elements if the element at pos1 is more dense than the element at pos2 (Liquid/Gas only)
    public bool SwapIfMoreDense(Vector2 pos1, Vector2 pos2) {
        List<ElementType> ValidTypes = new List<ElementType>() { ElementType.Liquid, ElementType.Gas };
        if (InBounds(pos2)) {
            Element e1 = Get(pos1);
            Element e2 = Get(pos2);

            if (e2 is Air)
                return false;

            if (!ValidTypes.Contains(e1.Type) || !ValidTypes.Contains(e2.Type))
                return false;

            if (e1.Density > e2.Density)
                return Swap(pos1, pos2);
        } else if (DestroyOutOfBounds) {
            Set(pos1, new Air(pos1));
            return true;
        }
        return false;
    }

    // Check if a position is in bounds
    public bool InBounds(Vector2 position) {
        return (int)position.X >= 0 && (int)position.X < (int)Size.X && (int)position.Y >= 0 && (int)position.Y < (int)Size.Y;
    }

    // Check if a position is empty (contains air) and in bounds
    public bool IsEmpty(Vector2 position) {
        if (InBounds(position))
            return Get(position) is Air;
        return false;
    }

    // Check if a position is in bounds and empty
    public bool InBoundsAndEmpty(Vector2 position) {
        if (InBounds(position)) {
            if (Get(position) is Air)
                return true;
            return false;
        }
        return false;
    }

    // Check if a position contains a type or is empty
    public bool IsTypeOrEmpty(Vector2 position, ElementType type) {
        if (InBounds(position)) {
            Element e = Get(position);
            return e is Air || e.Type == type;
        }
        return false;
    }

    // Check if a position contains a solid
    public bool IsSolid(Vector2 position) {
        if (InBounds(position))
            return Get(position).Type == ElementType.Solid;
        return false;
    }

    // Check if a position contains a solid
    public bool IsLiquid(Vector2 position) {
        if (InBounds(position))
            return Get(position).Type == ElementType.Liquid;
        return false;
    }

    // Check if a position contains a solid
    public bool IsGas(Vector2 position) {
        if (InBounds(position))
            return Get(position).Type == ElementType.Gas;
        return false;
    }

    // Check if a position contains a solid
    public bool IsPowder(Vector2 position) {
        if (InBounds(position))
            return Get(position).Type == ElementType.Powder;
        return false;
    }

    // Check if an element is surrounded (has no valid moves)
    public bool Surrounded(Vector2 position) {
        foreach (Vector2 Dir in Direction.Full) {
            if (IsEmpty(position + Dir))
                return false;
        }
        return true;
    }

    // Return a list of the elements surrounding a position
    public List<Element> GetNeighbors(Vector2 position) {
        List<Element> Neighbors = new List<Element>();
        foreach (Vector2 Dir in Direction.Full) {
            if (InBounds(position + Dir)) {
                if (!IsEmpty(position + Dir))
                    Neighbors.Add(Get(position + Dir));
            }
        }
        return Neighbors;
    }
}