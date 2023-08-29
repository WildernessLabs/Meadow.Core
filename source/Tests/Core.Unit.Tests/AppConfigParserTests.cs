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
            Assert.Equal("mqtt.meadowcloud.co", s.UpdateSettings.UpdateServer);
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

        [Fact]
        public void ParseFileWithQuotes()
        {
            var yml = File.ReadAllText("app.config.quotes.yaml");

            var s = AppSettingsParser.Parse(yml);

            Assert.Equal("NoQuotes", s.Settings["MyApp.NetName0"]);
            Assert.Equal("SingleQuotes", s.Settings["MyApp.NetName1"]);
            Assert.Equal("Quotes with spaces", s.Settings["MyApp.NetName3"]);
            Assert.Equal("value \"contains\" quotes", s.Settings["MyApp.NetName4"]);
        }

        [Fact]
        public void ParseEnvizorFile()
        {
            var yml = File.ReadAllText("app.config.envizor.yaml");

            var s = AppSettingsParser.Parse(yml);

            // no error means pass
        }
    }
}
