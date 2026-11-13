using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace VelaWeb.Server;

// RFC 6238: Google Authenticator defaults (SHA1, six digits, 30 seconds).
public static class GoogleAuthenticator
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    public static string CreateSecret()
    {
        var bytes = RandomNumberGenerator.GetBytes(20);
        var result = new StringBuilder(32);
        int buffer = 0, bits = 0;
        foreach (var b in bytes)
        {
            buffer = (buffer << 8) | b;
            bits += 8;
            while (bits >= 5)
            {
                bits -= 5;
                result.Append(Alphabet[(buffer >> bits) & 31]);
            }
        }
        return result.ToString();
    }

    public static bool Verify(string secret, string? code, DateTimeOffset? now = null)
    {
        if (code == null || code.Length != 6 || code.Any(c => c < '0' || c > '9'))
            return false;
        var key = new List<byte>();
        int buffer = 0, bits = 0;
        foreach (var c in secret.Trim().TrimEnd('=').ToUpperInvariant())
        {
            var value = Alphabet.IndexOf(c);
            if (value < 0) return false;
            buffer = (buffer << 5) | value;
            bits += 5;
            if (bits >= 8)
            {
                bits -= 8;
                key.Add((byte)(buffer >> bits));
            }
        }
        if (key.Count == 0) return false;
        var step = (now ?? DateTimeOffset.UtcNow).ToUnixTimeSeconds() / 30;
        using var hmac = new HMACSHA1(key.ToArray());
        // Allow one interval of clock drift in either direction.
        for (var delta = -1; delta <= 1; delta++)
        {
            var counter = new byte[8];
            BinaryPrimitives.WriteInt64BigEndian(counter, step + delta);
            var hash = hmac.ComputeHash(counter);
            var offset = hash[^1] & 15;
            var number = BinaryPrimitives.ReadInt32BigEndian(hash.AsSpan(offset, 4)) & 0x7fffffff;
            var expected = (number % 1000000).ToString("D6");
            if (CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(expected), Encoding.ASCII.GetBytes(code)))
                return true;
        }
        return false;
    }
}
