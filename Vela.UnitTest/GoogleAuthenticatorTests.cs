using VelaWeb.Server;
using Xunit;

namespace Vela.UnitTest;

public class GoogleAuthenticatorTests
{
    private const string Secret = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ";

    // RFC 6238 SHA1 vectors, truncated to the Google six-digit format.
    [Theory]
    [InlineData(59L, "287082")]
    [InlineData(1111111109L, "081804")]
    [InlineData(1111111111L, "050471")]
    [InlineData(1234567890L, "005924")]
    [InlineData(2000000000L, "279037")]
    [InlineData(20000000000L, "353130")]
    public void AcceptsPublishedVectors(long timestamp, string code)
    {
        Assert.True(GoogleAuthenticator.Verify(Secret, code, DateTimeOffset.FromUnixTimeSeconds(timestamp)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("abcdef")]
    [InlineData("000000")]
    public void RejectsInvalidCodes(string? code)
    {
        Assert.False(GoogleAuthenticator.Verify(Secret, code, DateTimeOffset.FromUnixTimeSeconds(59)));
    }

    [Fact]
    public void AllowsOnlyOneStepOfClockDrift()
    {
        Assert.True(GoogleAuthenticator.Verify(Secret, "287082", DateTimeOffset.FromUnixTimeSeconds(89)));
        Assert.False(GoogleAuthenticator.Verify(Secret, "287082", DateTimeOffset.FromUnixTimeSeconds(119)));
    }

    [Fact]
    public void CreatesRandomBase32Secrets()
    {
        var secret = GoogleAuthenticator.CreateSecret();
        Assert.Matches("^[A-Z2-7]{32}$", secret);
        Assert.NotEqual(secret, GoogleAuthenticator.CreateSecret());
    }
}
