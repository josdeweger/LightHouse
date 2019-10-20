using System.Collections.Generic;
using CommandLine;
using LightHouse.Lib;

namespace LightHouse
{
    public class Options
    {
        [Option('s', "service", Required = true, HelpText = "Type of service used. Only one option at the moment: 'devops'.")]
        public BuildService Service { get; set; }

        [Option('r', "refresh-interval", Required = false, HelpText = "Refresh interval in seconds. Defaults to 30 seconds, value starting from 5", Default = 30)]
        public int RefreshInterval { get; set; }

        [Option('b', "brightness", Required = false, HelpText = "Set the brightness of the LED's, value between 1 and 100", Default = 5)]
        public int Brightness { get; set; }

        [Option('i', "instance", Required = true,
            HelpText = "DevOps instance (url contains following parts: http(s)://{instance}[/{collection}[/{team-project}]).")]
        public string Instance { get; set; }

        [Option('c', "collection", Required = true,
            HelpText = "DevOps collection (url contains following parts: http(s)://{instance}[/{collection}[/{team-project}]).")]
        public string Collection { get; set; }

        [Option('p', "team-projects", Separator = ',', Required = false,
            HelpText = "Team Projects, comma seperated (url contains following parts: http(s)://{instance}[/{collection}[/{team-project}]).")]
        public IEnumerable<string> TeamProjects { get; set; }

        [Option('u', "username", Required = false, HelpText = "User name used to authenticate.")]
        public string Username { get; set; }

        [Option('t', "token", Required = true, HelpText = "Token used to authenticate.")]
        public string Token { get; set; }
    }
}