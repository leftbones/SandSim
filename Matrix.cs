using System.Numerics;
using Raylib_cs;

namespace SharpSand;

class Matrix {
    public Vector2i ScreenSize { get; private set; }
    public int Scale { get; private set; }
    public Vector2i Size { get; private set; }
    public Element[,] Elements { get; private set; }

    public int Tick { get; private set; }
    public bool DestroyOutOfBounds { get; set; } = true;

    public bool Active = true;
    public bool StepTick = false;

    public bool UseChunks { get; set; } = false;
    public Chunk[,] Chunks { get; private set; }
    private int ChunkSize = 80;



    public Matrix(Vector2i screen_size, int scale) {
        ScreenSize = screen_size;
        Scale = scale;
        Tick = 0;

		Size = new Vector2i(ScreenSize.X / Scale, ScreenSize.Y / Scale);

        // Create chunks
        Chunks = new Chunk[ScreenSize.X / ChunkSize, ScreenSize.Y / ChunkSize];
        for (int x = 0; x < ScreenSize.X / ChunkSize; x++) {
            for (int y = 0; y < ScreenSize.Y / ChunkSize; y++) {
                var ChunkPos = new Vector2i(x * ChunkSize, y * ChunkSize);
                Chunks[x, y] = new Chunk(ChunkPos, ChunkSize);
            }
        }

        // Create elements array and fill with air
        Elements = new Element[Size.X, Size.Y];
        for (int x = 0; x < Size.X; x++) {
            for (int y = 0; y < Size.Y; y++) {
                Elements[x, y] = new Air(new Vector2i(x, y));
            }
        }
    }

    // Update all non-air elements in the matrix
    public void Update() {
        bool IsEvenTick = Tick % 2 == 0;
        if (Active) {

            ////
            // Active Chunks Only
            if (UseChunks) {
                for (int cy = ScreenSize.Y / ChunkSize - 1; cy >= 0; cy--) {
                    for (int cx = IsEvenTick ? 0 : ScreenSize.X / ChunkSize - 1; IsEvenTick ? cx < ScreenSize.X / ChunkSize : cx >= 0; cx += IsEvenTick ? 1 : -1) {
                        var Chunk = Chunks[cx, cy];

                        if (Chunk.Awake) {
                            for (int y = Chunk.Position.Y + Chunk.Size - 1; y >= Chunk.Position.Y; y--) {
                                for (int x = IsEvenTick ? 0 : Chunk.Position.X + Chunk.Size - 1; IsEvenTick ? x < Chunk.Position.X + ChunkSize : x >= Chunk.Position.X; x += IsEvenTick ? 1 : -1) {
                                    Element e = Get(new Vector2i(x / Scale, y / Scale));
                                    if (e.GetType() != typeof(Air) && !e.AlreadyStepped) {
                                        e.LastPosition = e.Position;
                                        e.Step(this);
                                        e.AlreadyStepped = true;
                                        e.Tick(this);
                                    }
                                }
                            }
                        }

                        Chunk.Update();
                    }
                }
            }

            ////
            // Entire Matrix
            else {
                for (int y = Size.Y - 1; y >= 0; y--) {
                    for (int x = IsEvenTick ? 0 : Size.X - 1; IsEvenTick ? x < Size.X : x >= 0; x += IsEvenTick ? 1 : -1) {
                        Element e = Get(new Vector2i(x, y));
                        if (e.GetType() != typeof(Air) && !e.AlreadyStepped) {
                            e.LastPosition = e.Position;
                            e.Step(this);
                            e.AlreadyStepped = true;
                            e.Tick(this);
                        }
                    }
                }
            }

            Tick++;

            if (StepTick) {
                Active = false;
                StepTick = false;
            }
        }
    }

    // Wake the chunk containing an element
    public void WakeChunk(Element e) {
        if (!UseChunks)
            return;

        Chunk c = GetChunk(e.Position);
        c.WakeNextStep = true;

        if (InBounds(e.Position + Direction.Left) && e.Position.X == c.Position.X / Scale) GetChunk(e.Position + Direction.Left).WakeNextStep = true;
        if (InBounds(e.Position + Direction.Up) && e.Position.Y == c.Position.Y / Scale) GetChunk(e.Position + Direction.Up).WakeNextStep = true;
        if (InBounds(e.Position + Direction.Right) && e.Position.X >= (c.Position.X / Scale) + (c.Size / Scale) - 1) GetChunk(e.Position + Direction.Right).WakeNextStep = true;
        if (InBounds(e.Position + Direction.Down) && e.Position.Y >= (c.Position.Y / Scale) + (c.Size / Scale) - 1) GetChunk(e.Position + Direction.Down).WakeNextStep = true;

        // if (InBounds(e.Position + (Direction.Left * 2)) && e.Position.X - 1 == c.Position.X / Scale) GetChunk(new Vector2i(e.Position.X - 1, e.Position.Y) + Direction.Left).WakeNextStep = true;
        // if (InBounds(e.Position + (Direction.Up * 2)) && e.Position.Y - 1 == c.Position.Y / Scale) GetChunk(new Vector2i(e.Position.X, e.Position.Y - 1) + Direction.Up).WakeNextStep = true;
        // if (InBounds(e.Position + (Direction.Right * 2)) && e.Position.X + 1 >= (c.Position.X / Scale) + (c.Size / Scale) - 1) GetChunk(new Vector2i(e.Position.X + 1, e.Position.Y) + Direction.Right).WakeNextStep = true;
        // if (InBounds(e.Position + (Direction.Down * 2)) && e.Position.Y + 1 >= (c.Position.Y / Scale) + (c.Size / Scale) - 1) GetChunk(new Vector2i(e.Position.X, e.Position.Y + 1) + Direction.Down).WakeNextStep = true;
    }

    // Get a chunk from matrix coordinates
    public Chunk GetChunk(Vector2i position) {
        return Chunks[
            (int)Math.Floor(((float)position.X / ChunkSize) * Scale),
            (int)Math.Floor(((float)position.Y / ChunkSize) * Scale)
        ];
    }

    // Get an element from the matrix
    public Element Get(Vector2i position) {
        return Elements[position.X, position.Y];
    }

    // Set an element in the matrix (and update the element's position)
    public bool Set(Vector2i position, Element element) {
        if (InBounds(position)) {
            Elements[position.X, position.Y] = element;
            element.Position = position;
            element.LastDirection = Direction.GetMovementDirection(element.LastPosition, element.Position);
            WakeChunk(element);
            return true;
        }
        return false;
    }

    // Set an element in the matrix without checking if the position is in bounds
    // public bool FastSet(Vector2i position, Element element) {
    //     Elements[position.X, position.Y] = element;
    //     return true;
    // }

    // Swap the position of two elements in the matrix if the destination is in bounds
    public bool Swap(Vector2i pos1, Vector2i pos2) {
        if (InBounds(pos2)) {
            Element e1 = Get(pos1);
            Element e2 = Get(pos2);
            Set(pos2, e1);
            Set(pos1, e2);
            return true;
        } else if (DestroyOutOfBounds) {
            Element air = new Air(pos1);
            Set(pos1, air);
            return true;
        }
        return false;
    }

    // Swap the position of two elements in the matrix if the second is empty
    public bool SwapIfEmpty(Vector2i pos1, Vector2i pos2) {
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
    public bool SwapIfType(Vector2i pos1, Vector2i pos2, ElementType type) {
        if (InBounds(pos2)) {
            Element e = Get(pos2);
            if (e.Type == type)
                return Swap(pos1, pos2);
            return false;
        } else if (DestroyOutOfBounds) {
            Element air = new Air(pos1);
            Set(pos1, air);
            return true;
        }
        return false;
    }

    // Swap the position of two elements if the element at pos2 matches the specified type or pos2 is empty (contains air)
    public bool SwapIfTypeOrEmpty(Vector2i pos1, Vector2i pos2, ElementType type) {
        if (InBounds(pos2)) {
            Element e = Get(pos2);
            if (e.Type == type || e.GetType() == typeof(Air))
                return Swap(pos1, pos2);
            return false;
        } else if (DestroyOutOfBounds) {
            Element air = new Air(pos1);
            Set(pos1, air);
            return true;
        }
        return false;
    }

    // Swap the position of two elements if the element at pos1 is less dense than the element at pos2 (Liquid/Gas only)
    public bool SwapIfLessDense(Vector2i pos1, Vector2i pos2) {
        List<ElementType> ValidTypes = new List<ElementType>() { ElementType.Liquid, ElementType.Gas };
        if (InBounds(pos2)) {
            Element e1 = Get(pos1);
            Element e2 = Get(pos2);

            if (!ValidTypes.Contains(e1.Type) || !ValidTypes.Contains(e2.Type))
                return false;

            if (e1.Density < e2.Density)
                return Swap(pos1, pos2);
        } else if (DestroyOutOfBounds) {
            Element air = new Air(pos1);
            Set(pos1, air);
            return true;
        }
        return false;
    }

    // Swap the position of two elements if the element at pos1 is more dense than the element at pos2 (Liquid/Gas only)
    public bool SwapIfMoreDense(Vector2i pos1, Vector2i pos2) {
        List<ElementType> ValidTypes = new List<ElementType>() { ElementType.Liquid, ElementType.Gas };
        if (InBounds(pos2)) {
            Element e1 = Get(pos1);
            Element e2 = Get(pos2);

            if (e2.GetType() == typeof(Air))
                return false;

            if (!ValidTypes.Contains(e1.Type) || !ValidTypes.Contains(e2.Type))
                return false;

            if (e1.Density > e2.Density)
                return Swap(pos1, pos2);
        } else if (DestroyOutOfBounds) {
            Element air = new Air(pos1);
            Set(pos1, air);
            return true;
        }
        return false;
    }

    // Check if a position is in bounds
    public bool InBounds(Vector2i position) {
        return position.X >= 0 && position.X < Size.X && position.Y >= 0 && position.Y < Size.Y;
    }

    // Check if a position is empty (contains air) and in bounds
    public bool IsEmpty(Vector2i position) {
        if (InBounds(position))
            return Get(position).GetType() == typeof(Air);
        return false;
    }

    // Check if a position is in bounds and empty
    public bool InBoundsAndEmpty(Vector2i position) {
        if (InBounds(position)) {
            if (Get(position).GetType() == typeof(Air))
                return true;
            return false;
        }
        return false;
    }

    // Check if a position contains a type or is empty
    public bool IsTypeOrEmpty(Vector2i position, ElementType type) {
        if (InBounds(position)) {
            Element e = Get(position);
            return e.Type == type || e.GetType() == typeof(Air);
        }
        return false;
    }

    // Check if a position contains a less dense element
    public bool IsLessDense(Vector2i pos1, Vector2i pos2) {
        if (InBounds(pos2)) {
            Element e1 = Get(pos1);
            Element e2 = Get(pos2);
            return e1.Density > e2.Density;
        }
        return false;
    }

    // Check if a position contains a solid
    public bool IsSolid(Vector2i position) {
        if (InBounds(position))
            return Get(position).Type == ElementType.Solid;
        return false;
    }

    // Check if a position contains a solid
    public bool IsLiquid(Vector2i position) {
        if (InBounds(position))
            return Get(position).Type == ElementType.Liquid;
        return false;
    }

    // Check if a position contains a solid
    public bool IsGas(Vector2i position) {
        if (InBounds(position))
            return Get(position).Type == ElementType.Gas;
        return false;
    }

    // Check if a position contains a solid
    public bool IsPowder(Vector2i position) {
        if (InBounds(position))
            return Get(position).Type == ElementType.Powder;
        return false;
    }

    // Check if an element is surrounded (has no valid moves)
    public bool Surrounded(Vector2i position) {
        foreach (Vector2i Dir in Direction.Full) {
            if (IsEmpty(position + Dir))
                return false;
        }
        return true;
    }

    // Return a list of the elements surrounding a position
    public List<Element> GetNeighbors(Vector2i position) {
        List<Element> Neighbors = new List<Element>();
        foreach (Vector2i Dir in Direction.Full) {
            if (InBounds(position + Dir)) {
                if (!IsEmpty(position + Dir))
                    Neighbors.Add(Get(position + Dir));
            }
        }
        return Neighbors;
    }

    // Reset the entire matrix to air
    public void Reset() {
        Elements = new Element[Size.X, Size.Y];
        for (int x = 0; x < Size.X; x++) {
            for (int y = 0; y < Size.Y; y++) {
                Elements[x, y] = new Air(new Vector2i(x, y));
            }
        }
    }

    // Unsettle every element in the matrix
    public void UnsettleAll() {
        foreach (Element e in Elements) {
            e.Settled = false;
        }
    }
}