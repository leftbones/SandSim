namespace SharpSand;

static class RNG {
    public static Random Random = new Random(Guid.NewGuid().GetHashCode());

    // Return true or false based on a percent chance out of 100
    public static bool Chance(int chance) {
        return Random.Next(1, 100) <= chance;
    }

    // Return the result of a coin flip
    public static bool CoinFlip() {
        return Chance(50);
    }

    // Return a random int from an inclusive range
    public static int Range(int min, int max) {
        return Random.Next(min, max+1);
    }
}