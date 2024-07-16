using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
    public class VFXFactory : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private Transform _disableVFXParent;
        
        private List<IVFXPlayer> _activeVFXs = new List<IVFXPlayer>();
        private List<IVFXPlayer> _disableVFXs = new List<IVFXPlayer>();

        public IVFXPlayer Create(Vector3 position, VFXData data)
        {
            int index = _disableVFXs.FindIndex(disable => disable.id == data.id);
            IVFXPlayer vfxPlayer;

            if (index > -1)
            {
                vfxPlayer = _disableVFXs[index];
                _activeVFXs.Add(_disableVFXs[index]);
                _disableVFXs.RemoveAt(index);
                vfxPlayer.Reset(position);
                return vfxPlayer;
            }
            
            var instance = Instantiate(data.prefab, position, Quaternion.identity, _parent);
            vfxPlayer = instance.GetComponent<IVFXPlayer>();
            _activeVFXs.Add(vfxPlayer);
            vfxPlayer.OnCreated(data.id, this);
            return vfxPlayer;
        }
        public IVFXPlayer Create(Transform target, VFXData data)
        {
            int index = _disableVFXs.FindIndex(disable => disable.id == data.id);
            IVFXPlayer vfxPlayer;

            if (index > -1)
            {
                vfxPlayer = _disableVFXs[index];
                _activeVFXs.Add(_disableVFXs[index]);
                _disableVFXs.RemoveAt(index);
                vfxPlayer.Reset(target.position);
                return vfxPlayer;
            }
            
            var instance = Instantiate(data.prefab, target.position, Quaternion.identity, target);
            vfxPlayer = instance.GetComponent<IVFXPlayer>();
            _activeVFXs.Add(vfxPlayer);
            vfxPlayer.OnCreated(data.id, this);
            return vfxPlayer;
        }
        public void Remove(IVFXPlayer player)
        {
            //TODO: need to wait for player finish playing
            _activeVFXs.Remove(player);
            _disableVFXs.Add(player);
            player.gameObject.SetActive(false);
            player.gameObject.transform.SetParent(_disableVFXParent);
        }
    }
}