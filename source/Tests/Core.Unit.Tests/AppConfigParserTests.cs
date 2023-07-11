using Meadow;
using System.IO;
using Xunit;

namespace Core.Unit.Tests
{
    public class AppConfigParserTests
    {
        [Fact]
        public void ParseFullFile()
        {
            var yml = File.ReadAllText("app.config.yaml");

            var parser = new AppSettingsParser();
            var s = parser.Parse(yml);

            Assert.True(s.UpdateSettings.Enabled);
            Assert.Equal("https://staging.meadowcloud.dev", s.UpdateSettings.AuthServer);
        }

        [Fact]
        public void ParseNoSpacesFile()
        {
            var yml = File.ReadAllText("app.config.nospaces.yaml");

            var parser = new AppSettingsParser();
            var s = parser.Parse(yml);
        }

        [Fact]
        public void ParseFileWithComments()
        {
            var yml = File.ReadAllText("app.config.comments.yaml");

            var parser = new AppSettingsParser();
            var s = parser.Parse(yml);
        }
    }
}
