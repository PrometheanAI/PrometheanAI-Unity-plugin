using System;
using System.Collections.Generic;
using PrometheanAI.Modules.TCPServer;
using PrometheanAI.Modules.TCPServer.Handlers;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.Core.Services
{   /// <summary>
    /// Class used to initialize and store handlers,and also dispatching needed handlers to Unities Main thread
    /// </summary>
    public class CommandsService
    {
        private readonly Dictionary<string, ICommand> m_commands = new Dictionary<string, ICommand>();
        /// <summary>
        /// public constructor
        /// </summary>
        public CommandsService() {
            RegisterHandlers();
        }

        /// <summary>
        /// Initializes handlers and registers them in m_commands list
        /// </summary>
        private void RegisterHandlers() {
            AddCommandHandler(new GetSelectedAndVisibleStaticMeshActors());
            AddCommandHandler(new GetTransformsForStaticMeshActors());
            AddCommandHandler(new ClearSelection());
            AddCommandHandler(new Focus());
            AddCommandHandler(new GetParents());
            AddCommandHandler(new Parent());
            AddCommandHandler(new Remove());
            AddCommandHandler(new RemoveDescendents());
            AddCommandHandler(new RemoveSelected());
            AddCommandHandler(new SaveCurrentScene());
            AddCommandHandler(new SceneName());
            AddCommandHandler(new Select());
            AddCommandHandler(new SetHidden());
            AddCommandHandler(new SetVisible());
            AddCommandHandler(new UnParent());
            AddCommandHandler(new ReportDone());
            AddCommandHandler(new StartSimulation());
            AddCommandHandler(new EndSimulation());
            AddCommandHandler(new GetsAllValidSceneActorsAndPaths());
            AddCommandHandler(new LoadLevel());
            AddCommandHandler(new OpenLevel());
            AddCommandHandler(new GetVisibleActors());
            AddCommandHandler(new GetAllValidSceneActors());
            AddCommandHandler(new GetVisibleStaticMeshActors());
            AddCommandHandler(new GetSelectedStaticMeshActors());
            AddCommandHandler(new GetCameraInfo());
            AddCommandHandler(new GetCurrentLevelPath());
            AddCommandHandler(new SetLevelCurrent());
            AddCommandHandler(new SetLevelInvisible());
            AddCommandHandler(new UnloadLevel());
            AddCommandHandler(new Rename());
            AddCommandHandler(new Kill());
            AddCommandHandler(new Rotate());
            AddCommandHandler(new RotateRelative());
            AddCommandHandler(new Scale());
            AddCommandHandler(new ScaleRelative());
            AddCommandHandler(new Translate());
            AddCommandHandler(new TranslateRelative());
            AddCommandHandler(new Fix());
            AddCommandHandler(new SaveCameraInfo());
            AddCommandHandler(new GetStaticMeshActorsByPath());
            AddCommandHandler(new SelectStaticMeshActorsByPath());
            AddCommandHandler(new GetLocationsForStaticMeshActors());
            AddCommandHandler(new GetPivotsForStaticMeshActors());
            AddCommandHandler(new GetStaticMeshActorsByMaterialPath());
            AddCommandHandler(new SelectStaticMeshActorsByMaterialPath());
            AddCommandHandler(new GetMaterialsFromMeshActorsByName());
            AddCommandHandler(new GetMaterialsFromSelectedMeshActor());
            AddCommandHandler(new TransformHandler());
            AddCommandHandler(new SetLevelVisible());
            AddCommandHandler(new GetAllExistingAssetsByType());
            AddCommandHandler(new GetAssetBrowserSelection());
            AddCommandHandler(new AddObjects());
            AddCommandHandler(new Add());
            AddCommandHandler(new GetTransformDataFromSimulatingActorsByName());
            AddCommandHandler(new RemoveMaterialOverrides());
            AddCommandHandler(new CreateMaterialInstance());
            AddCommandHandler(new SetMaterialsForSelectedMeshActor());
            AddCommandHandler(new Screenshot());
            AddCommandHandler(new SetMeshAssetMaterial());
            AddCommandHandler(new SetMesh());
            AddCommandHandler(new SetMeshOnSelection());
            AddCommandHandler(new AddMeshOnSelection());
            AddCommandHandler(new AddGroup());
            AddCommandHandler(new TranslateAndSnap());
            AddCommandHandler(new TranslateAndRaytrace());
            AddCommandHandler(new SetMaterialsForActor());
            AddCommandHandler(new Raytrace());
            AddCommandHandler(new RaytraceBidirectional());
            AddCommandHandler(new RaytracePoints());
            AddCommandHandler(new AddObjectsFromTriangles());
            AddCommandHandler(new GetVertexDataFromSceneObjects());
            AddCommandHandler(new GetVertexDataFromSceneObject());
            AddCommandHandler(new DeleteSelectedAssetsInBrowser());
            AddCommandHandler(new Edit());
            AddCommandHandler(new Find());
            AddCommandHandler(new FindAssets());
            AddCommandHandler(new LoadAssets());
            AddCommandHandler(new ImportAsset());
            AddCommandHandler(new ImportTexture());
            AddCommandHandler(new GetAssetDataFromPaths());
            AddCommandHandler(new GetMatAttrsFromMeshActorsByName());
            AddCommandHandler(new GetMatAttrsFromSelectedMeshActors());
            AddCommandHandler(new ImportAssetToAssetBrowser());
            AddCommandHandler(new ImportAssetToAssetBrowserAtPath());
            AddCommandHandler(new GetMatInstanceScalarAttrValForMeshActorsByName());
            AddCommandHandler(new GetMatInstanceVecAttrValForMeshActorsByName());
            AddCommandHandler(new SetMatInstanceScalarAttrForMeshActorsByName());
            AddCommandHandler(new SetMatInstanceVecAttrForMeshActorsByName());
            AddCommandHandler(new SetMatInstanceVecAttrForMeshActorsByNameAndIndex());
            AddCommandHandler(new EnableSimulationOnActorsByName());
            AddCommandHandler(new DisableSimulationOnActorsByName());
            AddCommandHandler(new GetSimulationOnActorsByName());
            AddCommandHandler(new Learn());
            AddCommandHandler(new RaytraceFromCamera());
            AddCommandHandler(new GetAssetDataFromBrowserSelection());
            AddCommandHandler(new UndoHandler());
        }

        /// <summary>
        /// adds new CommandHandler
        /// </summary>
        /// <param name="handler"></param>
        private void AddCommandHandler(ICommand handler) {
            m_commands.Add(handler.GetToken, handler);
        }

        /// <summary>
        /// Checks if CommandService has a handle for incoming command from PrometheanAI
        /// and enqueues it via the MainThreadDispatcher
        /// </summary>
        /// <param name="rawCommandData"></param>
        /// <param name="commandToken"></param>
        /// <param name="commandParametersString"></param>
        /// <param name="callback"></param>
        public void Parse(string rawCommandData, string commandToken, List<string> commandParametersString, Action<CommandHandleProcessState, string> callback) {
            if (!m_commands.ContainsKey(commandToken)) {
                Debug.LogError($"Unknown token encountered: {commandToken}!");
                callback.Invoke(CommandHandleProcessState.Failed, string.Empty);
                return;
            }

            var handler = m_commands[commandToken];
            MainThreadDispatcher.Enqueue(() => {
                handler.Handle(rawCommandData, commandParametersString, callback);
            });
        }
    }
}