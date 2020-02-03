using System;
using LightHouse.Lib;
using ReactiveUI;

namespace Lighthouse.UI.Models
{
    public class LighthouseSettings : ReactiveObject
    {
        private string _instance = "http://rhm-p-tfs01.kantoor.tld:8080/tfs";
        public string Instance
        {
            get => _instance;
            set => this.RaiseAndSetIfChanged(ref _instance, value);
        }

        private BuildService _service = BuildService.Tfs;
        public BuildService Service
        {
            get => _service;
            set => this.RaiseAndSetIfChanged(ref _service, value);
        }

        private string _collection = "RedHotMinute";
        public string Collection
        {
            get => _collection;
            set => this.RaiseAndSetIfChanged(ref _collection, value);
        }

        private string _projects = "Landal Evolution Git";
        public string Projects
        {
            get => _projects;
            set => this.RaiseAndSetIfChanged(ref _projects, value);
        }

        private string _excludeBuildDefinitionIds = "562,564";
        public string ExcludeBuildDefinitionIds
        {
            get => _excludeBuildDefinitionIds;
            set => this.RaiseAndSetIfChanged(ref _excludeBuildDefinitionIds, value);
        }

        private string _token = "dwbufiaabjmezu4myxocdn5ukf7e3azyz3qhir5ydi5mq56mvjxq";
        public string Token
        {
            get => _token;
            set => this.RaiseAndSetIfChanged(ref _token, value);
        }

        private int _refreshInterval = 30;
        public int RefreshInterval
        {
            get => _refreshInterval;
            set => this.RaiseAndSetIfChanged(ref _refreshInterval, value);
        }

        private int _brightness = 5;
        public int Brightness
        {
            get => _brightness;
            set => this.RaiseAndSetIfChanged(ref _brightness, value);
        }

        private bool _enableFlashing = false;
        public bool EnableFlashing
        {
            get => _enableFlashing;
            set => this.RaiseAndSetIfChanged(ref _enableFlashing, value);
        }
    }
}