namespace Shared.Networking.Common.Interfaces
{
    /// <summary>
    /// Simple model for wrapping Initialize, Start and Stop operations, while securing thread safety of these methods.
    /// </summary>
    public interface IStartStopModel
    {
        void Initialize();
        void Start();
        void Stop();
    }
}