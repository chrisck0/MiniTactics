using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class ProjectileMover : MonoBehaviour, IPoolable
{
    [SerializeField] protected float _speed = 15f;
    [SerializeField] protected float _projectileSpeed = 3f;
    [SerializeField] protected float _hitOffset = 0f;
    [SerializeField] protected bool _useFirePointRotation;
    [SerializeField] protected Vector3 _rotationOffset = Vector3.zero;

    [Header("Effects")]
    [SerializeField] protected GameObject _hit;
    [SerializeField] protected ParticleSystem _hitPS;
    [SerializeField] protected GameObject _flash;
    [SerializeField] protected ParticleSystem _projectilePS;
    [SerializeField] protected GameObject[] _detached;

    [Header("Components")]
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected Light _lightSourse;

    [Header("Lifetime")]
    [SerializeField] protected int _lifeTime = 2000;

    private CancellationTokenSource _lifeTokenSource;
    private CancellationTokenSource _disableAfterHitTokenSource;

    private ObjectPool<ProjectileMover> _objectPool;
    private Transform _parent;
    private bool _isProjectileMove;
    private Vector3 _projectileVelocity;


    [System.Serializable]
    protected class DetachedState
    {
        public GameObject Obj;
        public Transform OriginalParent;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
    }

    protected DetachedState[] _detachedStates;

    protected virtual void Awake()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

        SetupDetachedCache();
    }

    protected virtual void OnEnable()
    {
        StopRunningLifeRoutine();
        StopRunningDisableAfterHitRoutine();

        if (_flash != null)
            _flash.transform.SetParent(null, true);

        if (_lightSourse != null)
            _lightSourse.enabled = true;

        if (_rb != null)
        {
            _rb.constraints = RigidbodyConstraints.None;
            _rb.linearVelocity = _projectileVelocity;
            _rb.angularVelocity = Vector3.zero;
        }

        if (_projectilePS != null)
        {
            _projectilePS.Clear(true);
            _projectilePS.Play(true);
        }

        RestoreDetachedObjects();

        GetLifeTokenSource();
        LifeTimer(_lifeTime, _lifeTokenSource.Token).Forget();
    }

    protected virtual void OnDisable()
    {
        StopRunningLifeRoutine();
        StopRunningDisableAfterHitRoutine();
    }

    private void StopRunningLifeRoutine()
    {
        if (_lifeTokenSource != null)
        {
            _lifeTokenSource.Cancel();
            _lifeTokenSource.Dispose();
            _lifeTokenSource = null;
        }
    }

    private void StopRunningDisableAfterHitRoutine()
    {
        if (_disableAfterHitTokenSource != null)
        {
            _disableAfterHitTokenSource.Cancel();
            _disableAfterHitTokenSource.Dispose();
            _disableAfterHitTokenSource = null;
        }
    }

    private void GetLifeTokenSource()
    {
        StopRunningLifeRoutine();

        _lifeTokenSource = new CancellationTokenSource();
    }

    private void GetDisableAfterHitTokenSource()
    {
        StopRunningDisableAfterHitRoutine();

        _disableAfterHitTokenSource = new CancellationTokenSource();
    }

    protected virtual void SetupDetachedCache()
    {
        if (_detached == null || _detached.Length == 0)
            return;

        _detachedStates = new DetachedState[_detached.Length];

        for (int i = 0; i < _detached.Length; i++)
        {
            GameObject obj = _detached[i];
            if (obj == null)
                continue;

            _detachedStates[i] = new DetachedState
            {
                Obj = obj,
                OriginalParent = obj.transform.parent,
                LocalPosition = obj.transform.localPosition,
                LocalRotation = obj.transform.localRotation,
                LocalScale = obj.transform.localScale
            };
        }
    }

    protected virtual void RestoreDetachedObjects()
    {
        if (_detachedStates == null || _detachedStates.Length == 0)
            return;

        for (int i = 0; i < _detachedStates.Length; i++)
        {
            DetachedState state = _detachedStates[i];
            if (state == null || state.Obj == null)
                continue;

            Transform t = state.Obj.transform;

            t.SetParent(state.OriginalParent, false);
            t.localPosition = state.LocalPosition;
            t.localRotation = state.LocalRotation;
            t.localScale = state.LocalScale;

            ParticleSystem[] systems = state.Obj.GetComponentsInChildren<ParticleSystem>(true);
            for (int j = 0; j < systems.Length; j++)
            {
                ParticleSystem ps = systems[j];
                if (ps == null)
                    continue;

                ps.Clear(true);
                ps.Play(true);
            }
        }
    }

    private async UniTask LifeTimer(int time, CancellationToken cancellationToken)
    {
        await UniTask.Delay(time, cancellationToken: cancellationToken);

        if (gameObject.activeSelf) ReturnToPool();
    }

    protected virtual void FixedUpdate()
    {
        if (_rb == null || _speed == 0f)
            return;

        if (_isProjectileMove)
        {
            if (_rb.linearVelocity == Vector3.zero)
            {
                _rb.linearVelocity = _projectileVelocity;
            }
            else
            {
                _rb.linearVelocity = new Vector3(
                _projectileVelocity.x * _projectileSpeed,
                _rb.linearVelocity.y + Physics.gravity.y * Time.deltaTime * _projectileSpeed,
                _projectileVelocity.z * _projectileSpeed
                );
            }

            if (_rb.linearVelocity.y < - _projectileVelocity.y)
            {
                Explode();
            }
        }
        else
        {
            _rb.linearVelocity = transform.forward * _speed;
        }
    }

    private void Explode()
    {
        StopRunningLifeRoutine();
        StopRunningDisableAfterHitRoutine();

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (_lightSourse != null)
            _lightSourse.enabled = false;

        ReleaseDetachedObjects();

        if (_projectilePS != null)
            _projectilePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        SpawnHit(transform.position, Vector3.up);

        float endDelay = 1f;
        if (_hitPS != null)
            endDelay = Mathf.Max(_hitPS.main.duration, 0.05f);

        GetDisableAfterHitTokenSource();
        DisableAfterHit((int)endDelay * 1000, _disableAfterHitTokenSource.Token).Forget();
    }

    private async UniTask DisableAfterHit(int delay, CancellationToken cancellationToken)
    {
        await UniTask.Delay(delay, cancellationToken: cancellationToken);

        if (gameObject != null && gameObject.activeSelf)
            ReturnToPool();
    }

    protected virtual void SpawnHit(Vector3 point, Vector3 normal)
    {
        if (_hit == null)
            return;

        Vector3 pos = point + normal * _hitOffset;
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);

        _hit.transform.position = pos;
        _hit.transform.rotation = rot;

        if (_useFirePointRotation)
            _hit.transform.rotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        else if (_rotationOffset != Vector3.zero)
            _hit.transform.rotation = Quaternion.Euler(_rotationOffset);
        else
            _hit.transform.LookAt(point + normal);

        if (_hitPS != null)
        {
            _hitPS.Clear(true);
            _hitPS.Play(true);
        }
    }

    protected virtual void ReleaseDetachedObjects()
    {
        if (_detachedStates == null || _detachedStates.Length == 0)
            return;

        for (int i = 0; i < _detachedStates.Length; i++)
        {
            DetachedState state = _detachedStates[i];
            if (state == null || state.Obj == null)
                continue;

            Transform t = state.Obj.transform;
            t.SetParent(null, true);

            ParticleSystem[] systems = state.Obj.GetComponentsInChildren<ParticleSystem>(true);
            for (int j = 0; j < systems.Length; j++)
            {
                ParticleSystem ps = systems[j];
                if (ps == null)
                    continue;

                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    public void Activate()
    {
        // gameObject.SetActive(true); 자동 활성화하면 버그 발생
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void ReturnToPool()
    {
        _objectPool.Return(this);
        transform.SetParent(_parent);
    }

    public void SetObjectPool(ObjectPool<ProjectileMover> objectPool, Transform parent)
    {
        _objectPool = objectPool;
        _parent = parent;
    }

    public void SetLifeTime(int lifeTime)
    {
        _lifeTime = lifeTime;
    }

    public void SetIsProjectileMove(bool isProjectileMove, Vector3 projectileVelocity)
    {
        _isProjectileMove = isProjectileMove;
        _projectileVelocity = projectileVelocity;
    }
}