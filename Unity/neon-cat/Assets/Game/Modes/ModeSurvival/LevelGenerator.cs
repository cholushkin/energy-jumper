using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Level;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;
using Range = GameLib.Random.Range;

public class LevelGenerator : Level
{
    public class GeneratorState
    {
        public int ChunksGenerated;
        public int Generation; // index of the state since beginning of generation
        public Vector3 StartPoint;
        public Vector3 EndPoint;
        public List<GameObject> HorizontallyGenerated = new List<GameObject>(16);
        public List<GameObject> UpGenerated = new List<GameObject>(8);
        public GameObject OriginChunk;
        public bool IsCycleComplete;
    }

    public long Seed;
    public bool StartGenerateOnAwake;
    public GeneratorSettingsProvider SettingsProvider;

    private GeneratorState _state;
    private GeneratorState _prevState;

    private int _pointerPlayer;
    private int _pointerGarbageCollector;
    private int _pointerGenerator; // if _pointerPlayer reach this point generator generate new portion of level and move pointers
    private const int _distanceWaterToGarbageCollector = 20;
    private const int _distancePlayerToGenerator = 200;

    private Range HorizontalPropagate = new Range(1, 4);
    private Range HorizontalPropagateMin = new Range(0, 2);
    private Range UpPropagate = new Range(1, 4);
    private Vector3 TileOffset = new Vector3(37, 60, 1);
    private LevelEnvironmentSettings _levelEnvSettings;
    private GeneratorSettingsProvider.LevelSettingsTable _levelSettings;

    private IPseudoRandomNumberGenerator _rnd;


    void Awake()
    {
        _rnd = RandomHelper.CreateRandomNumberGenerator(Seed);
        Seed = _rnd.GetState().AsNumber();
        Debug.Log(Seed);

        _levelSettings = SettingsProvider.GetLevelSettingsTable(StateGameplay.Instance.LevelID);

        // initialize pointers
        _pointerPlayer = 0;
        _pointerGenerator = _pointerPlayer + _distancePlayerToGenerator;

        // initialize the state
        _state = new GeneratorState();
        _state.EndPoint = _state.StartPoint = new Vector3(0f, 0f, TileOffset.z);

        // apply level settings to chunkprefabs
        _levelEnvSettings = GetComponent<LevelEnvironmentSettings>();

        if (StartGenerateOnAwake)
            StartGenerate();
    }

    public void StartGenerate()
    {
        StartCoroutine(Generate());
    }

    public bool IsFirstGenerationComplete()
    {
        return _state.IsCycleComplete && _state.Generation >= 1;
    }

    IEnumerator Generate()
    {
        while (!StopGeneratingCondition())
        {
            // Generate next cycle
            if (_state.EndPoint.y < _pointerGenerator + _distancePlayerToGenerator)
            {
                // new state
                _prevState = _state;
                _state = new GeneratorState();
                _state.Generation = _prevState.Generation + 1;
                _state.StartPoint = _prevState.EndPoint;

                var sagRight = _prevState.EndPoint.x > 0f;
                var leftAmount = sagRight
                    ? _rnd.FromRangeInt(HorizontalPropagate)
                    : _rnd.FromRangeInt(HorizontalPropagateMin);
                var rightAmount = sagRight
                    ? _rnd.FromRangeInt(HorizontalPropagateMin)
                    : _rnd.FromRangeInt(HorizontalPropagate);
                var upAmount = _rnd.FromRangeInt(UpPropagate);

                yield return GenerateLeft(leftAmount + 1, _state.StartPoint);
                _state.OriginChunk = _state.HorizontallyGenerated.First();

                yield return GenerateRight(rightAmount, _state.StartPoint + new Vector3(TileOffset.x, 0f, 0f));

                yield return GenerateUp(upAmount);
                _state.EndPoint = _state.UpGenerated.Last().transform.position + Vector3.up*TileOffset.y;

                _state.HorizontallyGenerated = _state.HorizontallyGenerated.OrderBy(x => x.transform.position.x).ToList();

                // create start portal
                if (_state.Generation == 1)
                {
                    var chunk0 = _state.OriginChunk;
                    var teleportPosition = chunk0.GetComponent<ChunkRandomization>().StablePositions
                        .FirstOrDefault(x => x.gameObject.activeInHierarchy).transform.position;
                    teleportPosition.z = 0f;

                    StartPoint = Instantiate(GamePrefabs.Instance.GameEntities["PortalStart"], teleportPosition, Quaternion.identity, transform).transform;
                }

                _state.IsCycleComplete = true;
            }

           


            yield return null;

            // Update conditions
            var player = StateGameplay.Instance.Player;
            if (player == null)
                continue;

            _pointerPlayer = (int)player.transform.position.y;
            _pointerGenerator = Mathf.Max(_pointerPlayer + _distancePlayerToGenerator, _pointerGenerator);

            // todo: _pointerGarbageCollector = water level - _distanceWaterToGarbageCollector 
        }

        // create end portal
        {
            var chunkLast = _state.UpGenerated.Last();
            var teleportPosition = chunkLast.GetComponent<ChunkRandomization>().StablePositions
                .FirstOrDefault(x => x.gameObject.activeInHierarchy).transform.position;
            teleportPosition.z = 0f;
            Instantiate(GamePrefabs.Instance.GameEntities["PortalFinish"], teleportPosition, Quaternion.identity, transform);
        }
    }

    private bool StopGeneratingCondition()
    {
        if (_state.ChunksGenerated > _levelSettings.MinChunks && _state.IsCycleComplete)
            return true;
        return false;
    }


    private IEnumerator GenerateLeft(int chunksNumber, Vector3 startPoint)
    {
        var pos = startPoint;
        var level = (int)_state.EndPoint.y;

        for (int i = 0; i < chunksNumber; ++i)
        {
            var chunkPrefab = SettingsProvider.GetRndPrefab(level, _rnd);
            var chunkName = $"[{chunkPrefab.name}]L:{_state.Generation}:{_state.ChunksGenerated}";
            var rot = i % 2 == 0 ? chunkPrefab.transform.rotation : chunkPrefab.transform.rotation * Quaternion.Euler(0, 180, 0);
            var chunk = CreateChunk(chunkPrefab, pos, rot, chunkName);
            var chunkRandomization = chunk.GetComponent<ChunkRandomization>();
            chunkRandomization.IsRotated = i % 2 != 0;


            pos.x -= TileOffset.x;
            _state.ChunksGenerated++;
            _state.HorizontallyGenerated.Add(chunk);
            yield return null;
        }
    }

    private IEnumerator GenerateRight(int chunksNumber, Vector3 startPoint)
    {
        var pos = startPoint;
        var level = (int)_state.EndPoint.y;

        for (int i = 0; i < chunksNumber; ++i)
        {
            var chunkPrefab = SettingsProvider.GetRndPrefab(level, _rnd);
            var chunkName = $"[{chunkPrefab.name}]R:{_state.Generation}:{_state.ChunksGenerated}";
            var rot = i % 2 == 0 ? chunkPrefab.transform.rotation * Quaternion.Euler(0, 180, 0) : chunkPrefab.transform.rotation;
            var chunk = CreateChunk(chunkPrefab, pos, rot, chunkName);
            var chunkRandomization = chunk.GetComponent<ChunkRandomization>();
            chunkRandomization.IsRotated = i % 2 == 0;
            pos.x += TileOffset.x;
            _state.ChunksGenerated++;
            _state.HorizontallyGenerated.Add(chunk);
            yield return null;
        }
    }

    private IEnumerator GenerateUp(int chunksNumber)
    {
        var level = (int)_state.EndPoint.y;
        var startChunk = _rnd.FromList(_state.HorizontallyGenerated);

        if (chunksNumber % 2 != 0)
            ++chunksNumber;

        // if chunk is not rotated then take it's left or right neighbour
        if (!startChunk.GetComponent<ChunkRandomization>().IsRotated)
        {
            var chunkIndex = _state.HorizontallyGenerated.IndexOf(startChunk);
            
            // get random left or right index clamped to list boundaries
            var neighbourIndex = Mathf.Clamp(_rnd.YesNo() ? chunkIndex - 1 : chunkIndex + 1, 0,
                _state.HorizontallyGenerated.Count - 1);

            if (neighbourIndex == chunkIndex)
            {
                if (neighbourIndex == 0)
                    neighbourIndex = 1;
                else if (neighbourIndex == _state.HorizontallyGenerated.Count - 1)
                    neighbourIndex = _state.HorizontallyGenerated.Count - 2;
            }

            startChunk = _state.HorizontallyGenerated[neighbourIndex];
            Assert.IsTrue(startChunk.GetComponent<ChunkRandomization>().IsRotated);
        }

        var pos = startChunk.transform.position + Vector3.up * TileOffset.y;
        for (int i = 0; i < chunksNumber; ++i)
        {
            var chunkPrefab = SettingsProvider.GetRndPrefab(level, _rnd);
            var chunkName = $"[{chunkPrefab.name}]U:{_state.Generation}:{_state.ChunksGenerated}";
            var rot = i % 2 != 0 ? chunkPrefab.transform.rotation * Quaternion.Euler(0, 180, 0) : chunkPrefab.transform.rotation;
            var chunk = CreateChunk(chunkPrefab, pos, rot, chunkName);
            var chunkRandomization = chunk.GetComponent<ChunkRandomization>();
            chunkRandomization.IsRotated = i % 2 != 0;

            var movingUp = i % 2 != 0;
            if (movingUp)
                pos += Vector3.up * TileOffset.y;
            else // random left or right
            {
                if(_rnd.YesNo())
                    pos += Vector3.left * TileOffset.x;
                else
                    pos += Vector3.right * TileOffset.x;
            }

            _state.ChunksGenerated++;
            _state.UpGenerated.Add(chunk);
            yield return null;
        }
    }

    private GameObject CreateChunk(GameObject prefab, Vector3 pos, Quaternion rot, string name)
    {
        var chunk = Instantiate(prefab, pos, rot);
        chunk.name = name;
        chunk.transform.SetParent(transform);
        chunk.GetComponent<ChunkRandomization>().Randomize(_rnd);
        _levelEnvSettings.ApplyMaterialToRenderers(chunk.transform.GetComponentsInChildren<Renderer>());
        return chunk;
    }
}
