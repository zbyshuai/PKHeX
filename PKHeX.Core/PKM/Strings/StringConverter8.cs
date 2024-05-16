using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Logic for converting a <see cref="string"/> for Generation 8 games.
/// </summary>
/// <remarks>Also used by LGP/E; this encoding is essentially the same as Generation 6's.</remarks>
public static class StringConverter8
{
    private const ushort TerminatorNull = 0;

    /// <summary>Converts Generation 7-Beluga encoded data to decoded string.</summary>
    /// <param name="data">Encoded data</param>
    /// <returns>Decoded string.</returns>
    public static string GetString(ReadOnlySpan<byte> data)
    {
        Span<char> result = stackalloc char[data.Length];
        int length = LoadString(data, result);
        return new string(result[..length]);
    }

    /// <inheritdoc cref="GetString(ReadOnlySpan{byte})"/>
    /// <param name="data">Encoded data</param>
    /// <param name="result">Decoded character result buffer</param>
    /// <returns>Character count loaded.</returns>
    public static int LoadString(ReadOnlySpan<byte> data, Span<char> result)
    {
        int i = 0;
        for (; i < data.Length; i += 2)
        {
            var value = ReadUInt16LittleEndian(data[i..]);
            if (value == TerminatorNull)
                break;
            result[i/2] = (char)value;
        }
        return i/2;
    }

    /// <summary>Gets the bytes for a Generation 7-Beluga string.</summary>
    /// <param name="destBuffer">Span of bytes to write encoded string data</param>
    /// <param name="value">Decoded string.</param>
    /// <param name="maxLength">Maximum length of the input <see cref="value"/></param>
    /// <param name="option">Buffer pre-formatting option</param>
    /// <returns>Encoded data.</returns>
    public static int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength,
        StringConverterOption option = StringConverterOption.ClearZero)
    {
        if (value.Length > maxLength)
            value = value[..maxLength]; // Hard cap

        if (option is StringConverterOption.ClearZero)
            destBuffer.Clear();

        WriteCharacters(destBuffer, value);

        int count = value.Length * 2;
        if (count == destBuffer.Length)
            return count;
        WriteUInt16LittleEndian(destBuffer[count..], TerminatorNull);
        return count + 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteCharacters(Span<byte> destBuffer, ReadOnlySpan<char> value)
    {
        if (BitConverter.IsLittleEndian)
        {
            var u16 = MemoryMarshal.Cast<char, byte>(value);
            u16.CopyTo(destBuffer);
            return;
        }
        for (int i = 0; i < value.Length; i++)
            WriteUInt16LittleEndian(destBuffer[(i * 2)..], value[i]);
    }

    /// <summary>
    /// Applies the under string to the top string, if possible.
    /// </summary>
    /// <param name="top">Displayed string</param>
    /// <param name="under">Previous string</param>
    /// <returns>Indication of the under string's presence.</returns>
    public static TrashMatch ApplyTrashBytes(Span<byte> top, ReadOnlySpan<char> under)
    {
        var index = TrashBytes.GetStringLength(top);
        if (index == -1)
            return TrashMatch.Skipped;
        index++; // hop over the terminator
        if (index >= under.Length) // Overlapping
            return TrashMatch.TooLongToTell;

        var src = under[index..];
        var dest = top[(index * 2)..];
        SetString(dest, src, src.Length, StringConverterOption.None);
        return TrashMatch.Present;
    }

    /// <summary>
    /// Checks the displayed top string against the under string to see if the under is present.
    /// </summary>
    /// <param name="top">Displayed string</param>
    /// <param name="under">Previous string</param>
    /// <returns>Indication of the under string's presence.</returns>
    public static TrashMatch GetTrashState(ReadOnlySpan<byte> top, ReadOnlySpan<char> under)
    {
        if (under.Length == 0)
            return TrashMatch.Skipped;

        var index = TrashBytes.GetStringLength(top);
        if ((uint)index >= under.Length)
            return TrashMatch.TooLongToTell;
        index++; // hop over the terminator

        return GetTrashState(top, under, index);
    }

    private static TrashMatch GetTrashState(ReadOnlySpan<byte> top, ReadOnlySpan<char> under, int index)
    {
        // Adjust our spans to the relevant sections
        under = under[index..];
        var relevantSection = top[(index * 2)..];
        bool check = IsEqualsEncoded(relevantSection, under);
        return check ? TrashMatch.Present : TrashMatch.NotPresent;
    }

    private static bool IsEqualsEncoded(ReadOnlySpan<byte> relevantSection, ReadOnlySpan<char> under)
    {
        if (BitConverter.IsLittleEndian)
        {
            var u16 = MemoryMarshal.Cast<char, byte>(under);
            return relevantSection.SequenceEqual(u16);
        }
        Span<byte> expect = stackalloc byte[relevantSection.Length];
        WriteCharacters(expect, under);
        return relevantSection.SequenceEqual(expect);
    }

    /// <summary>
    /// Used when importing a 3DS string into HOME.
    /// </summary>
    public static void NormalizeHalfWidth(Span<byte> str)
    {
        if (BitConverter.IsLittleEndian)
        {
            var u16 = MemoryMarshal.Cast<byte, char>(str);
            foreach (ref var c in u16)
            {
                if (c == TerminatorNull)
                    return;
                c = NormalizeHalfWidth(c);
            }
        }

        // Slower path for Big-Endian runtimes.
        for (int i = 0; i < str.Length; i += 2)
        {
            var data = str[i..];
            var c = ReadUInt16LittleEndian(data);
            if (c == TerminatorNull)
                return;
            WriteUInt16LittleEndian(data, NormalizeHalfWidth((char)c));
        }
    }

    private static char NormalizeHalfWidth(char str) => StringConverter.NormalizeGenderSymbol(str);
}
