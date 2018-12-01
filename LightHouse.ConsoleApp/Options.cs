using System.Collections.Generic;
using CommandLine;

namespace LightHouse.ConsoleApp
{
    public class Options
    {
        [Option('s', "service", Required = true, HelpText = "Type of service used. Only one option at the moment: 'vsts'.")]
        public string Service { get; set; }
    }

    public class VstsOptions : Options
    {
        [Option('i', "instance", Required = true, 
            HelpText = "VSTS instance (url contains following parts: https://{instance}[/{collection}[/{team-project}]).")]
        public string Instance { get; set; }

        [Option('c', "collection", Required = true,
            HelpText = "VSTS collection (url contains following parts: https://{instance}[/{collection}[/{team-project}]).")]
        public string Collection { get; set; }

        [Option('p', "team-projects", Separator = ',', Required = false,
            HelpText = "Team Projects, comma seperated (url contains following parts: https://{instance}[/{collection}[/{team-project}]).")]
        public IEnumerable<string> TeamProjects { get; set; }

        [Option('t', "token", Required = true,
            HelpText = "Personal token used to authenticate.")]
        public string PersonalToken { get; set; }
    }
}