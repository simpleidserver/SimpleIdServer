namespace SimpleIdServer.Mobile.Extensions;

public static class StringExtensions
{
    public static byte[] ConvertToBase32(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));
        input = input.TrimEnd('=');
        int byteCount = input.Length * 5 / 8;
        byte[] returnArray = new byte[byteCount];

        byte curByte = 0, bitsRemaining = 8;
        int mask = 0, arrayIndex = 0;

        foreach (char c in input)
        {
            int cValue = CharToValue(c);

            if (bitsRemaining > 5)
            {
                mask = cValue << (bitsRemaining - 5);
                curByte = (byte)(curByte | mask);
                bitsRemaining -= 5;
            }
            else
            {
                mask = cValue >> (5 - bitsRemaining);
                curByte = (byte)(curByte | mask);
                returnArray[arrayIndex++] = curByte;
                curByte = (byte)(cValue << (3 + bitsRemaining));
                bitsRemaining += 3;
            }
        }

        if (arrayIndex != byteCount)
        {
            returnArray[arrayIndex] = curByte;
        }

        return returnArray;
    }

    private static int CharToValue(char c)
    {
        int value = (int)c;
        if (value < 91 && value > 64)
        {
            return value - 65;
        }

        if (value < 56 && value > 49)
        {
            return value - 24;
        }

        if (value < 123 && value > 96)
        {
            return value - 97;
        }

        throw new ArgumentException("Character is not a Base32 character.", "c");
    }
}
