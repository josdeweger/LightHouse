namespace LightHouse.Lib
{
    public interface IControlBuildStatusLight
    {
        void SetSignalLight(LastBuildsStatus buildsStatus);
        bool IsConnected { get; }
    }
}