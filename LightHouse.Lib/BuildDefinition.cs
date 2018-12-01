using System;

namespace LightHouse.Lib
{
    public class Build
    {
        public string BuildNumber { get; set; }
        public int DefinitionId { get; set; }
        public string DefinitionName { get; set; }
        public BuildStatus Status { get; set; }
        public BuildResult Result { get; set; }
        public string RequestedBy { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        public string StartAndFinishTime =>
            $"Started: {StartTime.ToShortDateString()} at {StartTime.ToShortTimeString()} \n" +
            $"Ended: {FinishTime.ToShortDateString()} at {FinishTime.ToShortTimeString()}";
    }
}