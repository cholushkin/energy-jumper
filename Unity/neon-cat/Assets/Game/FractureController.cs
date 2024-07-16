using DG.Tweening;
using GameLib.Log;
using LeakyAbstraction;
using UnityEngine;
using UnityEngine.Assertions;


namespace Game
{
    public class FractureController : MonoBehaviour
    {
        public BlockVisual BlockVisual;
        public GameObject Parent;
        public GameObject Fractures;
        
        [SerializeField]
        private MeshCollider[] _fractures;

        private int _hits;

        [SerializeField]
        private int _aliveFractures;
        
        [Tooltip("If 0 than take it from RigidBody or use hardcoded constant")]
        public float EachFractureMass;
        
        public LogChecker LogChecker;
        public bool InitOnAwake;

        void Awake()
        {
            if (InitOnAwake)
            {
                if (Fractures != null)
                {
                    _fractures = Fractures.GetComponentsInChildren<MeshCollider>(true);
                    foreach (var fracture in _fractures)
                        fracture.enabled = false;
                    _aliveFractures = _fractures.Length;
                }

                if (LogChecker.Verbose())
                    Debug.Log($"{gameObject.name}. Fractures {_fractures.Length}");
                Assert.IsNotNull(BlockVisual);
            }
            Assert.IsTrue(_fractures.Length != 0);
        }

        public void Crack(Vector3 direction, float energy, Vector3 sourcePos)
        {
            if (LogChecker.Verbose())
                Debug.Log($" {Parent.name}. Crack!");

            if (_hits++ == 0)
            {
                Fractures.SetActive(true);
                BlockVisual.EnableVisual(false);
            }

            ScaleDown();
            SoundManager.Instance.PlaySound(GameSound.SfxBlockCrack);
        }

        public void Explode(Vector3 direction, float energy, Vector3 sourcePosition, bool addAdditionalForceFromCenter = false)
        {
            if (_hits++ == 0)
            {
                Fractures.SetActive(true);
                BlockVisual.EnableVisual(false);
            }


            if (LogChecker.Verbose())
                Debug.Log($" {Parent.name} Explode!");
            Parent.GetComponent<MeshCollider>().enabled = false;

            var prb = Parent.GetComponent<Rigidbody>();
            if (EachFractureMass == 0f)
                EachFractureMass = prb != null ? prb.mass / _fractures.Length : 1f / 42;

            int index = 0;
            foreach (var fracture in _fractures)
            {
                var rDir = addAdditionalForceFromCenter
                    ? (direction.normalized + (fracture.transform.position - transform.position).normalized).normalized
                    : direction;

                var rb = fracture.gameObject.AddComponent<Rigidbody>();
                fracture.enabled = true;
                rb.isKinematic = false;
                rb.AddForceAtPosition(energy * rDir, sourcePosition, ForceMode.Force);

                if (index++ % 5 == 0)
                    fracture.gameObject.AddComponent<Obstacle>();
                fracture.gameObject.transform.DOScale(Vector3.zero, Random.Range(2f, 4f))
                    .SetDelay(2f)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        _aliveFractures--;
                        Destroy(fracture.gameObject);
                        if (_aliveFractures < 1)
                        {
                            Destroy(Parent);
                        }
                    });
            }

            SoundManager.Instance.PlaySound(GameSound.SfxBlockExplode);
        }


        [ContextMenu("Dbg ScaleDown")]
        private void ScaleDown()
        {
            int i = 0;
            foreach (var fracture in _fractures)
            {
                var isScaled = fracture.gameObject.transform.localScale.x >= 0.999f;
                if (isScaled && Random.value > 0.5f)
                {
                    fracture.gameObject.transform.localScale *= 0.925f;
                    i++;
                }
            }
        }
    }
}