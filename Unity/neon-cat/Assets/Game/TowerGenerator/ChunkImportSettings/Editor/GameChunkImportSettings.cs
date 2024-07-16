using TowerGenerator;
using UnityEditor;

namespace Game
{
    [InitializeOnLoad]
    public static class GameChunkImportSettings
    {
        static GameChunkImportSettings()
        {
            //ChunkImportSettingsManager.RegisterChunkImportSettings("Ships", new ChunkImportSettings{ ChunksOutputPath = "Assets/Game/Prefabs", Scale = 1.65f});
            //ChunkImportSettingsManager.RegisterChunkImportSettings("HomeIsland", new ChunkImportSettings { ChunksOutputPath = "Assets/Game/HomeIsland/!Prefabs", AddColliders = true});
            // todo: import triangles from towergenerator pack : ChunkImportSettingsManager.RegisterChunkImportSettings("TriangleTilesPack", new ChunkImportSettings { ChunksOutputPath = "Assets/Game/Level/!Prefabs/TriangleTiles", AddColliders = true });
        }
    }
}