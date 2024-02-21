using PrometheanAI.Modules.TCPServer;
using UnityEditor;
using UnityEngine;

namespace PrometheanAI.Modules.Editor
{
    /// <summary>
    /// Class which initializes PrometheanTcpWrapperClient in EditMode
    /// </summary>
    [InitializeOnLoad]
    static class PrometheanBridge
    {
        static PrometheanBridge() {
            Debug.Log("Initialize Promethean AI Bridge");
            new PrometheanTcpWrapperClient();
        }
    }
}