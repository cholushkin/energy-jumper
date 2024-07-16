using System;
using UnityEngine;

namespace Game.VFX
{
    [Serializable]
    public class VFXData
    {
        public string id;
        public GameObject prefab;
        public VFXData(string id, GameObject prefab)
        {
            this.id = id;
            this.prefab = prefab;
        }
        private VFXData(VFXData data)
        {
            id = data.id;
            prefab = data.prefab;
        }
        public VFXData Clone()
        {
            return new VFXData(this);
        }
    }
}