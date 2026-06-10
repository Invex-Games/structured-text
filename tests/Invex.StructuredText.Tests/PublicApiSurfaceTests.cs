using Invex.RepoUtils.TestUtils;

namespace Invex.StructuredText.Tests;

[TestFixture]
public class PublicApiTests
{
    [Test]
    public async Task VerifyPublicApiSurface()
    {
        await VerifyJson(PublicApiSurfaceTestUtil.GetPublicApiSurface(typeof(StructuredTextWriter).Assembly));
    }
}
