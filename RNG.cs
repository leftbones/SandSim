namespace SharpSand;

static class RNG {
    public static Random Random = new Random(Guid.NewGuid().GetHashCode());

    // Return true or false based on a roll using floating point numbers
    public static bool Roll(float n) {
        return Random.NextDouble() <= n;
    }

    // Return true or false based on 1:n odds
    public static bool Odds(int n) {
        return Range(1, n) == 1;
    }

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