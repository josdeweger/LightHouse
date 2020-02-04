using LightHouse.Lib;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Lighthouse.UI.Models
{
    public class LighthouseSettings : ReactiveObject
    {
        [Reactive]
        public string Instance { get; set; }

        [Reactive]
        public BuildService Service { get; set; }

        [Reactive]
        public string Collection { get; set; }

        [Reactive]
        public string Projects { get; set; }

        [Reactive]
        public string ExcludeBuildDefinitionIds { get; set; }

        [Reactive]
        public string Token { get; set; }

        [Reactive]
        public int RefreshInterval { get; set; }

        [Reactive]
        public int Brightness { get; set; }

        [Reactive]
        public bool EnableFlashing { get; set; }

        public LighthouseSettings()
        {
            Instance = "http://rhm-p-tfs01.kantoor.tld:8080/tfs";
            Service = BuildService.Tfs;
            Collection = "RedHotMinute";
            Projects = "Landal Evolution Git";
            ExcludeBuildDefinitionIds = "562,564";
            Token = "dwbufiaabjmezu4myxocdn5ukf7e3azyz3qhir5ydi5mq56mvjxq";
            RefreshInterval = 30;
            Brightness = 5;
            EnableFlashing = false;
        }
    }
}