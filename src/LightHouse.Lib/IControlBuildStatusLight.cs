namespace LightHouse.Lib
{
    public interface IControlBuildStatusLight
    {
        void SetSignalLight(LastBuildsStatus buildsStatus, bool? enableFlashing, byte brightness = 5);
        bool IsConnected { get; }
    }
}