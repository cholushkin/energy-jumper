using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using GameLib.Alg;
using GameLib.Log;
using TowerGenerator;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    // It's assumed that every upgrade item is a child of some Group
    // when we enable any upgrade group takes control of how to enable it's child (usually TowerGenerator use groups to randomize it's state, but here we enable child of the group manually)
    // [upgrade0] - enabled visually parts, GetPrevUpgrade returns this upgrade
    // [upgrade1] - invisible for now, _currentUpgradeIndex==1, next upgrade to visualize and pay, GetCurrentUpgrade return this upgrade
    // [upgrade2] - future upgrades

    public class HomeIslandController : MonoBehaviour, IHandle<ChunkControllerBase.EventNodeActiveStateChanged>
    {
        public class EventIslandUpgrade
        {
            public int IslandLevel;
        }


        [Serializable]
        public class UpgradeItem
        {
            public string ObjectName;
            public int PriceCoins;
            public float Rotation;
            public float Pan;
            public float Zoom;
            public bool IncreaseLevel;
        }

        public GameObject HomeIslandChunkPrefab;
        public HomeCameraController HomeCameraController;
        public HomeIslandEnvironmentSettings HomeSettings;
        public HomeIslandUpgradeVisualizer HomeIslandUpgradeVisualizer;

        public List<UpgradeItem> AllUpgrades;

        public LogChecker Log;
        
        private ChunkControllerBase _chunkController;
        private Dictionary<string, GameObject> _nameToObject;
        private bool _coroutineRunning;
        private int _currentUpgradeIndex;
        private bool _suppressRevealEffect;

        void Awake()
        {
            HomeSettings.Apply();
#if UNITY_EDITOR
            Debug.Log($"instantiating from chunk prefab: {AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(HomeIslandChunkPrefab))}");
#endif
            Instantiate(HomeIslandChunkPrefab, transform);
            _chunkController = GetComponentInChildren<ChunkControllerBase>();
            Assert.IsNotNull(_chunkController);
            _chunkController.Init();
            CacheObjects();
            LoadStateFromUserAccount();

            // print some debug information
            if (Log.Normal())
            {
                Debug.Log($"Upgrades available: {AllUpgrades.Count}");
                Debug.Log($"Upgrades affecting LEVEL: {AllUpgrades.Count(x=> x.IncreaseLevel)}");
                Debug.Log($"Upgrades price sum : {AllUpgrades.Sum(x => x.PriceCoins)}");
            }
        }


        public void SetEnabled(bool flag)
        {
            if(flag)
                GlobalEventAggregator.EventAggregator.Subscribe(this);
            else
                GlobalEventAggregator.EventAggregator.Unsubscribe(this);
            gameObject.SetActive(flag);
        }


        public bool AbleToDoUpgrade()
        {
            if (_coroutineRunning)
            {
                //Debug.LogError("Can't run next animation due to still playing prev animation");
                return false;
            }

            if (_currentUpgradeIndex >= AllUpgrades.Count)
            {
                //Debug.LogError("No more upgrades defined!");
                return false;
            }

            var upgradeState = AllUpgrades[_currentUpgradeIndex];
            
            if (!StateHome.Instance.IsEnoughCoins(upgradeState.PriceCoins))
            {
                //Debug.LogError("Can't do upgrade. Not enough money");
                return false;
            }

            return true;
        }

        public void DoUpgrade()
        {
            var account = UserAccounts.Instance.GetCurrentAccount();
            var curUpgrade = AllUpgrades[_currentUpgradeIndex];

            Assert.IsTrue(AbleToDoUpgrade(), "unable to upgrade");

            if (StateHome.Instance.SpendCoins(curUpgrade.PriceCoins)) // acc data change
            {
                StartUpgradeAnimation();
                if(curUpgrade.IncreaseLevel)
                    account.AccountData.IslandLevel +=  1; // acc data change
                EnableNextUpgrade();
                account.AccountData.IslandUpgradeIndex = _currentUpgradeIndex; // acc data change
                account.Save();
                GlobalEventAggregator.EventAggregator.Publish( new EventIslandUpgrade{IslandLevel = account.AccountData.IslandLevel } );
            }
        }

        public void EnableNextUpgrade()
        {
            var curUpgrade = AllUpgrades[_currentUpgradeIndex++];
            EnableUpgrade(curUpgrade);
        }

        public void EnableUpgrade(UpgradeItem upgrade)
        {
            var objectsToEnable = upgrade.ObjectName.Split(',');

            foreach (var objName in objectsToEnable)
            {
                var item = _nameToObject[objName];
                if (item == null)
                    Debug.LogError($"Can't find {objName}");
                var ownerGroup = item.transform.parent.GetComponent<Group>();
                ownerGroup.EnableItem(item.transform.GetSiblingIndex(), true, true); // enable
            }
        }

        public UpgradeItem GetCurrentUpgrade()
        {
            var index = GetCurrentUpgradeIndex();
            if (index == -1)
                return null;
            return AllUpgrades[index];
        }

        public UpgradeItem GetPrevUpgrade()
        {
            var index = GetPrevUpgradeIndex();
            if (index == -1)
                return null;
            return AllUpgrades[index];
        }

        public int GetCurrentUpgradeIndex()
        {
            if (_currentUpgradeIndex >= AllUpgrades.Count)
                return -1;
            return _currentUpgradeIndex;
        }

        public int GetPrevUpgradeIndex()
        {
            if (_currentUpgradeIndex - 1 < 0)
                return -1;
            if (_currentUpgradeIndex - 1 >= AllUpgrades.Count)
                return -1;
            return _currentUpgradeIndex - 1;
        }

        public UpgradeItem GetUpgrade(int index)
        {
            return AllUpgrades[index];
        }


        private void StartUpgradeAnimation()
        {
            StartCoroutine(CoroutineUpgradeAnimation());
        }

        IEnumerator CoroutineUpgradeAnimation()
        {
            _coroutineRunning = true;
            {
                var curUpgrade = AllUpgrades[_currentUpgradeIndex];
                Debug.Log($"Upgrading : \"{curUpgrade.ObjectName}\", cam pan:{curUpgrade.Pan} zoom:{curUpgrade.Zoom} rot:{curUpgrade.Rotation}");

                if (curUpgrade.Zoom == 0f)
                    Debug.LogError("no camera params saved for upgrade " + curUpgrade.ObjectName);

                HomeCameraController.SetFocusTarget(curUpgrade.Rotation, curUpgrade.Zoom, curUpgrade.Pan);

                yield return new WaitUntil(() => !HomeCameraController.IsAnimatingToValue());
                HomeCameraController.SetRelaxing();
                EnableUpgrade(curUpgrade); // it will cause EventNodeActiveStateChanged handler
            }
            _coroutineRunning = false;
        }

        private void LoadStateFromUserAccount()
        {
            var account = UserAccounts.Instance.GetCurrentAccount();
            _currentUpgradeIndex = account.AccountData.IslandUpgradeIndex;
            Debug.Log($"Loading HomeIsland state = {_currentUpgradeIndex}");
            _suppressRevealEffect = true;
            for (int i = 0; i < _currentUpgradeIndex; ++i)
                EnableUpgrade(AllUpgrades[i]);
            _suppressRevealEffect = false;
        }

        private void CacheObjects()
        {
            var impactTree = _chunkController.GetImpactTree();
            
            // fill _nameToObject
            _nameToObject = new Dictionary<string, GameObject>();
            foreach (var treeNode in impactTree.TraverseDepthFirstPostOrder())
            {
                var group = treeNode.Data;
                foreach (var child in group.transform.Children())
                    _nameToObject[child.name] = child.gameObject;
            }
        }

        public void Handle(ChunkControllerBase.EventNodeActiveStateChanged message)
        {
            if(_suppressRevealEffect)
                return;
            HomeIslandUpgradeVisualizer.VisualizePart(message.Node);
        }
    }

}