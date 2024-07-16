using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Entities
{
    // Eats walls, blocks, decorations inside target pattern
    public class EntPatternEater : MonoBehaviour
    {
        /*
        private class DisableableEntity
        {
            public MeshCollider Collider;
            public MeshRenderer MeshRenderer;
        }

        public Pattern InsidePattern { get; set; }
        public Degradator PrefabDegradator;
        private List<DisableableEntity> _disableables;
        private Degradator _lastDegradator;

        void Start()
        {
            _disableables = GetDisableableObjects();
            Assert.IsTrue(_disableables.Count > 5);
            StartCoroutine(DegradationProcess());

        }

        List<DisableableEntity> GetDisableableObjects()
        {
            List<DisableableEntity> result = new List<DisableableEntity>();

            var colliders = GetComponentsInChildren<MeshCollider>();

            foreach (var mCollider in colliders)
            {
                var mRend = mCollider.gameObject.GetComponent<MeshRenderer>();
                if (mRend == null)
                    continue;
                result.Add(new DisableableEntity { Collider = mCollider, MeshRenderer = mRend });
            }
            return result;
        }

        private float degVal;
        void Update()
        {
            degVal -= Time.deltaTime;
        }

        IEnumerator DegradationProcess()
        {


            while (_disableables.Count > 0)
            {
                SpawnDegradator();
                float nextDegradationStep = Random.Range(8, 20);
                degVal = nextDegradationStep;
                yield return new WaitForSeconds(nextDegradationStep);
                if (_lastDegradator == null)
                    continue; // player picked it up
                var rndObject = _disableables[Random.Range(0, _disableables.Count)];
                _disableables.Remove(rndObject);
                if (rndObject.Collider != null)
                {
                    RunDisappearing(rndObject);
                    PlayDegradatorActivation(_lastDegradator.gameObject, rndObject);
                    yield return new WaitForSeconds(3f);
                }
            }
        }

        void OnGUI()
        {
            if (_lastDegradator != null)
                GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 100), degVal.ToString());
        }

        private void PlayDegradatorActivation(GameObject degradator, DisableableEntity ent)
        {
            const float activationDuration = 2f;
            degradator.transform.DOMove(ent.Collider.transform.position, activationDuration * 0.65f);
            degradator.RemoveComponent<PickupItem>();
            degradator.transform.DOScale(Vector3.one * 5, activationDuration).SetEase(Ease.InOutBack).OnComplete(() => Destroy(degradator.gameObject));
        }

        private void SpawnDegradator()
        {
            if (_lastDegradator != null)
                return;
            //transform.position
            var r = Random.insideUnitSphere * 35;
            r.z = -1;
            _lastDegradator = Instantiate(PrefabDegradator, r, Quaternion.identity, transform.parent);
        }

        public void OnPickupDegradator()
        {
            TimeCounter.Instance.RewardScore(degVal);
        }

        void RunDisappearing(DisableableEntity ent)
        {
            Debug.LogFormat("Annihilation: {0}", ent.Collider.gameObject.name);
            ent.Collider.enabled = false;
            ent.Collider.transform.DOScale(Vector3.zero, 3f)
                .SetEase(Ease.InBounce)
                .OnComplete(() => Destroy(ent.Collider.gameObject));
        }

        void OnDrawGizmos()
        {
            if (_lastDegradator == null)
                return;

            var p1 = EntityJumper.Instance.transform.position;
            var p2 = _lastDegradator.transform.position;
            Gizmos.DrawLine(p1, p2);
        }


        public Degradator GetCurrentDegradator()
        {
            return _lastDegradator;
        }
        */
    }
}
