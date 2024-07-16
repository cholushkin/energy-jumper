using UnityEngine;

namespace Game
{
    public class HomeIslandControllerDev : MonoBehaviour
    {
        public HomeIslandController HomeIslandController;
        public bool ListenKeyboard;

        void Update()
        {
            if (ListenKeyboard)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.E))
                {
                    DoEnableNext();
                }

                if (UnityEngine.Input.GetKeyDown(KeyCode.C))
                {
                    var prevUpgradeIndex = HomeIslandController.GetPrevUpgradeIndex();
                    if (prevUpgradeIndex != -1)
                    {
                        var upgrade = StateHome.Instance.HomeIslandControllerPrefab.AllUpgrades[prevUpgradeIndex];

                        upgrade.Pan = HomeCameraController.Instance.ControllerPan.TargetObject.transform.localPosition
                            .y;
                        upgrade.Zoom = HomeCameraController.Instance.ControllerZoom.TargetObject.transform.localPosition
                            .z;
                        upgrade.Rotation = HomeCameraController.Instance.ControllerRotate.TargetObject.transform
                            .localRotation.eulerAngles.y;

                        Debug.Log(
                            $"SET CAM to prefab. Upgrade {upgrade.ObjectName}; pan:{upgrade.Pan} zoom:{upgrade.Zoom} rot:{upgrade.Rotation}");
                    }
                }

                if (UnityEngine.Input.GetKeyDown(KeyCode.D))
                {
                    Debug.Log(
                        $"SET DEFAULT CAM to prefab. Upgrade {HomeIslandController.GetCurrentUpgrade().ObjectName}");
                }
            }
        }


        public void DoUpgrade()
        {
            HomeIslandController.DoUpgrade();
        }

        public void DoEnableNext()
        {
            HomeIslandController.EnableNextUpgrade();
        }


        //[ContextMenu("DbgSpawnIslandFull")]
        //public void DbgSpawnIslandFull()
        //{
        //    HomeSettings.Apply();
        //    Instantiate(HomeIslandPrefab, transform);
        //    _chunkController = GetComponentInChildren<ChunkControllerBase>();
        //    Assert.IsNotNull(_chunkController);
        //    _chunkController.Init();
        //    PrepareChunk();


        //    var impactTree = _chunkController.GetImpactTree();
        //    foreach (var treeNode in impactTree.TraverseDepthFirstPostOrder())
        //    {
        //        Group group = treeNode.Data;
        //        foreach (Transform child in group.transform)
        //        {
        //            var upgradeItem = AllUpgrades.FirstOrDefault(x => x.ObjectName == child.name);

        //            if (upgradeItem == null)
        //                continue;
        //            upgradeItem._enableItemindex = child.GetSiblingIndex();
        //            upgradeItem._group = group;
        //        }
        //    }

        //    SyncStateTo(55);
        //}

        public void PrintInfo()
        {
            Debug.Log($"All upgrades number: {HomeIslandController.AllUpgrades.Count}");
        }
    }
}
