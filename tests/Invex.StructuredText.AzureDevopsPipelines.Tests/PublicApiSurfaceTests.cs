using Invex.RepoUtils.TestUtils;

namespace Invex.StructuredText.AzureDevopsPipelines.Tests;

[TestFixture]
public class PublicApiTests
{
    [Test]
    public async Task VerifyPublicApiSurface()
    {
        await VerifyJson(PublicApiSurfaceTestUtil.GetPublicApiSurface(typeof(DevopsExpressionFormatter).Assembly));
    }
}
