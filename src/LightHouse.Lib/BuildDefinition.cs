using System;

namespace LightHouse.Lib
{
    public class Build
    {
        public int? DefinitionId { get; set; }
        public string DefinitionName { get; set; }
        public BuildStatus? Status { get; set; }
        public BuildResult? Result { get; set; }
    }
}