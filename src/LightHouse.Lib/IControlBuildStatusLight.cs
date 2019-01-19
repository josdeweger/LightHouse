namespace LightHouse.Lib
{
    public interface IControlBuildStatusLight
    {
        void SetSignalLight(LastBuildsStatus buildsStatus, byte brightness = 5);
        bool IsConnected { get; }
    }
}