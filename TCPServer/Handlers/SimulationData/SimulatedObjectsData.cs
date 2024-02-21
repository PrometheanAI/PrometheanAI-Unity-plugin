using System.Collections.Generic;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer.Handlers
{
    /// <summary>
    /// Data container for GetTransformDataFromSimulatingActorsByName Handler
    /// </summary>
    public static class SimulatedObjectsData
    {
        public static Dictionary<string, Vector3[]> SimulatedData = new Dictionary<string, Vector3[]>();
    }
}