namespace Odin.Messaging
{
    /// <summary>
    /// Represents a client connection to a messaging service
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Disconnects if already connected.
        /// </summary>
        /// <returns></returns>
        bool EnsureDisconnected();

        /// <summary>
        /// IsConnected
        /// </summary>
        bool IsConnected { get; }
        
        /// <summary>
        /// Status
        /// </summary>
        string Status { get; }
        
        /// <summary>
        /// EnsureConnected
        /// </summary>
        void EnsureConnected();
    }
}
