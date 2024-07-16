using GameLib.Alg;
using UnityEngine;

namespace Game.VFX
{
    public class VFXManager : Singleton<VFXManager>
    {
        [SerializeField] private VFXFactory _factory;
        [SerializeField] private VFXDatabase _database;
        [SerializeField] private VFXGroupHelper _helperPrefab;
        
        public void SpawnVFX(string effectID, Vector3 spawnPosition)
        {
            var vfx = _database.Get(effectID);
            var vfxPlayer = _factory.Create(spawnPosition, vfx);
            vfxPlayer.Play();
        }
        public void SpawnVFX(string effectID, Transform transform)
        {
            var helper = transform.GetComponent<VFXGroupHelper>();
            helper ??= Instantiate(_helperPrefab, transform);
            var vfx = _database.Get(effectID);
            var vfxPlayer = _factory.Create(transform, vfx);
            helper.Register(vfxPlayer);
            vfxPlayer.Play();
        }
    }
}
