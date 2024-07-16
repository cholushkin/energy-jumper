using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Level.Entities
{
    [SelectionBase]
    public class PortalOut : PortalControllerBase
    {

        public class SpawningQueue
        {
            public class SpawningItem
            {
                public GameObject ObjectToSpawn;
                public float Delay;
                public Vector3 Speed;
            }

            public class SpawningItemPrefab : SpawningItem
            {
            }

            private const float DefaultDelay = 2f;
            private Queue<SpawningItem> _queue = new Queue<SpawningItem>();


            public void AddItem(GameObject obj, Vector3 speed, float delay = DefaultDelay)
            {
                Assert.IsNotNull(obj);
                var item = new SpawningItem
                {
                    ObjectToSpawn = obj,
                    Delay = delay,
                    Speed = speed
                };
                _queue.Enqueue(item);
            }

            public void AddItemPrefab(GameObject obj, Vector3 speed, float delay = DefaultDelay)
            {
                Assert.IsNotNull(obj);
                var item = new SpawningItemPrefab
                {
                    ObjectToSpawn = obj,
                    Delay = delay,
                    Speed = speed
                };
                _queue.Enqueue(item);
            }

            public SpawningItem Pop()
            {
                if (_queue.Count == 0)
                    return null;
                return _queue.Dequeue();
            }

            public bool IsEmpty()
            {
                return _queue.Count == 0;
            }
        }

        public class EventSpawnObject // Spawned when portal spawns an object (but not the player)
        {
            public GameObject SpawnedObject;
        }

        public class EventSpawnPlayer // Spawned when portal spawns player (but not the object)
        {
            public GameObject SpawnedPlayer;
        }

        public bool IsCloseOnEmptyQueue;
        public SpawningQueue SpawnQueue = new SpawningQueue();

        private SpawningQueue.SpawningItem _currentItem;
        private float CurrentSpawnDelay;

        public interface IOutingPortalSupport // objects that wants to work with PortalOut should implement this
        {
            void OnStartOut(TweenCallback onFinishCallback);
        }

        public enum OutingState
        {
            Waiting, // for next object added to the queue
            StartSpawning, // playing animation
            WaitingForFinishSpawning,
        }

        private OutingState _outingState;


        void Update()
        {
            if (CurrentState == State.Working)
            {
                if (_outingState == OutingState.Waiting)
                {
                    _currentItem = SpawnQueue.Pop();

                    // get new spawning item from the queue
                    if (_currentItem != null)
                    {
                        CurrentSpawnDelay = _currentItem.Delay;
                        _outingState = OutingState.StartSpawning;
                    }
                }
                if (_outingState == OutingState.StartSpawning)
                {
                    // update spawning delay and spawn new
                    if (CurrentSpawnDelay <= 0f)
                    {
                        _outingState = OutingState.WaitingForFinishSpawning;
                        Spawn(_currentItem);
                    }
                    CurrentSpawnDelay -= Time.deltaTime;
                }
                if (_outingState == OutingState.WaitingForFinishSpawning)
                {

                }
            }
        }

        public void Spawn(SpawningQueue.SpawningItem spawningItem)
        {
            Assert.IsNotNull(spawningItem);
            Assert.IsNotNull(spawningItem.ObjectToSpawn);
            var obj = spawningItem.ObjectToSpawn;

            // prefab
            if (spawningItem is SpawningQueue.SpawningItemPrefab)
            {
                obj = Instantiate(spawningItem.ObjectToSpawn);

                // get my pattern
                var spawnParent = transform.parent;
                //Assert.IsNotNull(patternTransform.GetComponent<Pattern>());

                // set parent to the pattern
                obj.transform.SetParent(spawnParent);
                // obj.transform.localScale = Vector3.one;
            }

            Debug.LogFormat("Spawning item: '{0}'", obj);

            // move object to portal position and enable it
            obj.transform.position = transform.position;
            obj.GetComponent<IOutingPortalSupport>().OnStartOut(() => OnFinishSpawn(obj));

            VisualController.PlaySpawnAnimation();
        }

        private void OnFinishSpawn(GameObject spawnedObject)
        {
            Assert.IsNotNull(_currentItem);
            if (_currentItem.ObjectToSpawn.GetComponent<PlayerController>() != null)
            {

                GlobalEventAggregator.EventAggregator.Publish(new EventSpawnPlayer { SpawnedPlayer = spawnedObject });
            }
            else
            {
                GlobalEventAggregator.EventAggregator.Publish(
                    new EventSpawnObject { SpawnedObject = _currentItem.ObjectToSpawn });
            }
            ProcessWorkingCounter();

            // close portal on empty queue?
            if (IsCloseOnEmptyQueue && SpawnQueue.IsEmpty() && CurrentState != State.Closing)
            {
                CurrentState = State.Closing;
                StartCoroutine(ClosingCoroutine());
            }
            _currentItem = null;
            _outingState = OutingState.Waiting;
        }
    }
}
