using Game;
using TowerGenerator.ChunkImporter;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    public class FbxCommandFractured : FbxCommandBase
    {
        // parameters
        public string FracturesGameObjectName;

        public FbxCommandFractured(string fbxCommandName, int executionPriority) : base(fbxCommandName, executionPriority)
        {
        }

        public override void ParseParameters(string parametersString, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");

            // set default values for parameters
            FracturesGameObjectName = gameObject.transform.GetChild(0).name;

            if (string.IsNullOrWhiteSpace(parametersString))
                return; // keep default values if there is no parameters

            FracturesGameObjectName = parametersString;
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportState importInformation)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importInformation);
            Assert.IsNull(gameObject.GetComponent<FractureController>());
            var fractureController = gameObject.AddComponent<FractureController>();
            var blockVisual = gameObject.AddComponent<BlockVisual>();

            blockVisual.BlockRenderer = blockVisual.GetComponent<Renderer>();
            gameObject.AddComponent<MeshCollider>().convex = true;


            fractureController.Fractures = gameObject.transform.Find(FracturesGameObjectName).gameObject;
            foreach (Transform fracture in fractureController.Fractures.transform)
            {
                fracture.gameObject.AddComponent<MeshCollider>().convex = true;
            }

            fractureController.BlockVisual = blockVisual;
            fractureController.Parent = blockVisual.gameObject;
            fractureController.InitOnAwake = true;
        }
    }

    [InitializeOnLoad]
    public static class CustomCommandRegistrator
    {
        static CustomCommandRegistrator()
        {
            FbxCommandExecutor.RegisterFbxCommand(new FbxCommandExecutor.CommandRegistrationEntry { Name = "Fractured", Creator = () => new FbxCommandFractured("Fractured", 5) });
        }

    }
}