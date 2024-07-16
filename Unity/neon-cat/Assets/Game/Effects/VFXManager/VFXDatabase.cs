using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
    [CreateAssetMenu(menuName = "VFX/Create VFXDatabase", fileName = "VFXDatabase", order = 0)]
    public class VFXDatabase : ScriptableObject
    {
        [SerializeField] private VFXData _fallback;
        [SerializeField] private List<VFXData> _datas = new List<VFXData>();

        public VFXData Get(string id)
        {
            var index = _datas.FindIndex(data => data.id == id);
            return index < 0 ? _fallback.Clone() : _datas[index].Clone();
        }
        public bool Add(string id, GameObject prefab)
        {
            var index = _datas.FindIndex(data => data.id == id);
            
            if (index > -1)
                return false;
            
            _datas.Add(new VFXData(id, prefab));
            return true;
        }
        public bool Remove(string id)
        {
            var index = _datas.FindIndex(data => data.id == id);
            
            if (index < 0)
                return false;
            
            _datas.RemoveAt(index);
            return true;
        }
        public bool Remove(int index)
        {
            if (index < 0 || index > _datas.Count - 1)
                return false;
            
            _datas.RemoveAt(index);
            return true;
        }
    }
}