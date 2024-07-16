using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
    public class VFXGroupHelper : MonoBehaviour
    {
        private List<IVFXPlayer> _vfxPlayers = new List<IVFXPlayer>(64);
        public void Register(IVFXPlayer player)
        {
            _vfxPlayers.Add(player);
        }
        private void OnDestroy()
        {
            for (int i = 0; i < _vfxPlayers.Count; i++)
                _vfxPlayers[i].Stop();
        }
    }
}