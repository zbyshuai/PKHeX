namespace PKHeX.Core;

/// <summary>
/// Logic for determining the PCJP Fifth Anniversary event gift received.
/// </summary>
public static class PCJPFifthAnniversary
{
    private const uint MaxTableWeight = 1000;
    private const uint EntryWeight = 125;

    // Table:
    // Pichu 125 Teeter Dance (100-124 shiny)
    // Pichu 125 Wish (100-124 shiny)
    // Bagon 125 Iron Defense
    // Bagon 125 Wish
    // Absol 125 Spite
    // Absol 125 Wish
    // Ralts 125 Charm
    // Ralts 125 Wish

    // As we can see from the above table, there are 4 different species, with Wish being the second set of moves.
    // From a rand(1000) result, we can determine the species and the moveset with the following branch-less operations:
    // Species index: (result / 250)
    // Moveset index: (result / 125) % 2
    // Shiny: if Pichu, (result % 125) >= 100
    public static (uint Index, bool Wish, bool Shiny) GetResultPCJP(uint rand)
    {
        var result = WeightedTable3.GetPeriodicWeight(rand, MaxTableWeight);
        var eighth = result / EntryWeight;
        var wish = (eighth & 1) == 1;
        var index = eighth >> 1;
        var shiny = index == 0 && (result % EntryWeight) >= 100;
        return (index, wish, shiny);
    }

    /// <summary>
    /// Gets the Species index within the PCJP table.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <returns>0-3</returns>
    public static uint GetIndexPCJP(ushort species)
    {
        // Pichu: 172 = 0_10_10_11_00
        // Bagon: 371 = 1_01_11_00_11
        // Absol: 359 = 1_01_10_01_11
        // Ralts: 280 = 1_00_01_10_00
        // To get the index of the species (0-3) from the above table, we can do some bitwise magic:
        // Bits 2 & 3 are different across species, and conveniently in sequential order! We can just shift right by 2 add 1, then clamp to 0-3.
        return (((uint)species >> 2) + 1) & 3u;
    }

    /// <summary>
    /// Check if the given species, shiny, and wish moveset status match the given rand result.
    /// </summary>
    public static bool IsMatch(ushort species, bool shiny, bool wish, uint rand)
    {
        var index = GetIndexPCJP(species);
        var result = GetResultPCJP(rand);
        return index == result.Index && wish == result.Wish && shiny == result.Shiny;
    }

    /// <summary>
    /// Gets a random 16-bit seed that will return the desired table result.
    /// </summary>
    public static uint GetSeedForResult(ushort species, bool shiny, bool wish, uint seed)
    {
        while (true)
        {
            var u16 = seed & 0xFFFF; // restricted
            var u32 = WeightedTable3.GetRandom32(u16);
            if (IsMatch(species, shiny, wish, u32))
                return u16;
            seed = LCRNG.Next(seed);
        }
    }
}
