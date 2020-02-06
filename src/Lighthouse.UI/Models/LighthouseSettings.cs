using LightHouse.Lib;
using LightHouse.UI.Persistence;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LightHouse.UI.Models
{
    public class LighthouseSettings : ReactiveObject, IModelBase
    {
        public int Id => 1;

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

        public static LighthouseSettings Default()
        {
            return new LighthouseSettings
            {
                RefreshInterval = 30,
                Brightness = 5,
                EnableFlashing = true
            };
        }
    }
}