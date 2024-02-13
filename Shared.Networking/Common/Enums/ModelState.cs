namespace Shared.Networking.Common.Enums
{
    /// <summary>
    /// For determining state of model that implements Initialize, Start, Stop
    /// </summary>
    public enum ModelState
    {
        /// <summary>
        /// State before initialization
        /// </summary>
        New,
        /// <summary>
        /// State after initialization
        /// </summary>
        Initialized,
        /// <summary>
        /// State after calling start
        /// </summary>
        Started,
        /// <summary>
        /// State after calling stop
        /// </summary>
        Stopped
    }
}