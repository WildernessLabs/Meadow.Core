using Meadow;
using Meadow.Logging;
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

            var s = AppSettingsParser.Parse(yml);

            Assert.True(s.UpdateSettings.Enabled);
            Assert.Equal("https://staging.meadowcloud.dev", s.UpdateSettings.AuthServer);
        }

        [Fact]
        public void Parse4SpaceFile()
        {
            var yml = File.ReadAllText("app.config.4.yaml");

            var s = AppSettingsParser.Parse(yml);

            Assert.Equal(LogLevel.Debug, s.LoggingSettings.LogLevel.Default);
        }

        [Fact]
        public void ParseNoSpacesFile()
        {
            var yml = File.ReadAllText("app.config.nospaces.yaml");

            var s = AppSettingsParser.Parse(yml);
        }

        [Fact]
        public void ParseFileWithComments()
        {
            var yml = File.ReadAllText("app.config.comments.yaml");

            var s = AppSettingsParser.Parse(yml);
        }
    }
}
