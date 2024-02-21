namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Enum representing the state of a command being processed by a Handler
    /// </summary>
    public enum CommandHandleProcessState
    {
        Success,
        SingleState,
        Failed
    }
}