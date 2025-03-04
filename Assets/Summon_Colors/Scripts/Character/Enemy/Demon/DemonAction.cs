using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.LookDev;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

public class DemonAction : EnemyAction
{
    private enum Skill
    {
        Bite,
        Clap,
        EarthQuake,
        Tail,
        Rush,
        Fire_Head,
        Fire_Body,
        Fire_Remain,
        Ball_Direct,
        Ball_Explosion,
        Ball_Remain,
        Landing_Direct,
        Landing_Explosion,
        Landing_Remain
    }


    [SerializeField] private ShaderManager _shaderManager;

    [SerializeField] private Transform _breathPosition;
    [SerializeField] private Transform _firePosition;
    [SerializeField] private Collider[] _rushColliders;
    [SerializeField] private Collider _handAttackCollider;
    [SerializeField] private Collider[] _tailColliders;

    [SerializeField] private GameObject _volcanicBomb;
    [SerializeField] private GameObject _earthQuake;
    [SerializeField] private GameObject _fireBall;
    [SerializeField] private GameObject _tackleExplosion;
    [SerializeField] private GameObject _tackleFire;
    [SerializeField] private ParticleSystem _flameStream;
    [SerializeField] private ParticleSystem _fireEmbers;
    [SerializeField] private ObjectPoolBase _breathPool;
    [SerializeField] private ObjectPoolBase _fireBallPool;
    [SerializeField] private ObjectPoolBase[] _embersPools;
    [SerializeField]private Renderer _renderer;

    [SerializeField] private float _firePower = 10.0f;
    [SerializeField] private float _rushSpeed = 2.0f;
    [SerializeField] private float _explosionTackleSpeed = 60.0f;
    [SerializeField] private float _breathCoolTime = 0.3f;
    [SerializeField] private Material _sky01;
    [SerializeField] private Material _sky02;

    private float _breathTimer = 0.0f;
    private Vector3 _rushVector = Vector3.zero;
    private bool _isRush = false;
    private bool _isBreath = false;
    private float _roarIntensity = 0.0f;
    private int _power = 0;
    private AudioSource _audioSource;
    private Vector3 _targetPosition = Vector3.zero;
    private bool _stateLock = false;
    private bool _willChangeSky = false;
    private float _red = 0.5f;
    private float _green = 0.1f;
    private float _blue = 0.1f;
    private Timer _vibrationTimer = null;
    public void IgnitEventMove()
    {
        _willChangeSky = true;
        _state = State.Action;
        _agent.updateRotation = false;
        _agent.updatePosition = false;
        _animator.SetTrigger("Event");
    }

    public void ChangeSky()
    {
        if (_sky01 != null && !InGameManager.Instance.IsEvent)
        {
            Debug.Log("Called!");
            _willChangeSky = false;
#if false
            RenderSettings.skybox = _sky01;
#else
            SetSkyBox(_sky01);
#endif
            IgnitTacckle();
        }
    }

    public override void ChangeDown()
    {
        base.ChangeDown();
        _agent.speed = _enemyBase.Agility;
    }

    public void StartAmbientChange()
    {
        StartCoroutine(ChangeAmbient());
    }
    public void ChangeSkyOnDie()
    {
        if (_sky02 != null)
        {
            Debug.Log("Called!!");
#if true
            RenderSettings.skybox = _sky02;
#else
            SetSkyBox(_sky02);
#endif
        }
    }

    public void CreateVolcanicBomb()
    {
        GameObject volcanicBomb = Instantiate(_volcanicBomb, _firePosition.position, Quaternion.identity);
        volcanicBomb.transform.forward = _firePosition.forward;
        if(volcanicBomb.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            float sin = _firePosition.forward.y;
            if(Mathf.Abs(sin) == 1.0f) { sin = 0.9848f; }
            float cos = Mathf.Sqrt(1.0f - sin * sin);
            float tan = sin / cos;

            float height = 4.0f;
            float sqrDistance = (_targetPosition - transform.position).sqrMagnitude;

            float sqrVelocity = (-Physics.gravity.y * (tan * tan + 1) * sqrDistance) /
                    (2.0f * (height + tan * Mathf.Sqrt(sqrDistance)));
            float firePower = rigidbody.mass * Mathf.Sqrt(sqrVelocity);
            rigidbody.AddForce(_firePosition.forward * firePower, ForceMode.Impulse);
        }
        if (volcanicBomb.TryGetComponent<VolcanicBomb>(out var projectile))
        {
            projectile.Initialize(
                (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Ball_Direct)),
                (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Ball_Explosion)),
                (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Ball_Remain)));
        }
    }

    public void CreateEarthQuake()
    {
        GameObject earthQuake = Instantiate(_earthQuake, _handAttackCollider.transform.position + Vector3.down * 1.0f, Quaternion.identity);
        if(earthQuake.TryGetComponent<EarthQuake>(out var component))
        {
            component.Initialize((int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.EarthQuake)));
        }
        earthQuake.transform.forward = transform.forward;
        _handAttackCollider.enabled = true;
        _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Clap));
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Attack, transform);
    }

    public void CreateExplosion()
    {
        GameObject explosionObj = Instantiate(_tackleExplosion, _rushColliders[2].transform.position, Quaternion.identity);
        if (explosionObj.TryGetComponent<Explosion>(out var explosion))
        {
            explosion.Initialize((int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Landing_Explosion)));
        } 
        GameObject fireObj = Instantiate(_tackleFire, _rushColliders[2].transform.position, Quaternion.identity);
        if (fireObj.TryGetComponent<Embers>(out var embers))
        {
            embers.Initialize((int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Landing_Remain)));
        }
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Rush, transform);
    }

    public void IgnitTacckle()
    {
        if(_enemyBase.TargetCharacter == null) { return; }
        _state = State.Action;
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position,
                    out navMeshHit, 10.0f, NavMesh.AllAreas))
        {
            _rushVector = navMeshHit.position;
            _agent.updateRotation = false;
            _animator.SetTrigger("Explosion");
            _stateLock = true;
        }
    }

    public void PlayFootStep01()
    {
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Walk01, transform);
    }
    public void PlayFootStep02()
    {
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Walk02, transform);
    }

    public void PlaySmallBite()
    {
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Bite_Small, transform);
    }
    
    public void PlayBite()
    {
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Bite, transform);
    }

    public void PlayVolcanicPrepare()
    {
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.FireBall_Prepare, transform);
    }

    public void PlayTacklePrepare01()
    {
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Tackle_Prepare01, transform);
    }

    public void PlayTacklePrepare02()
    {
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Tackle_Prepare02, transform);
    }

     public void PlayTacklePrepare03()
    {
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Tackle_Prepare03, transform);
    }

    public void Bite()
    {
        _rushColliders[0].enabled = true;
        _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Bite));
    }


    public void CreateFireBall()
    {
        if(_fireBallPool == null)
        {
            GameObject fireBall = Instantiate(_fireBall, _breathPosition.position, Quaternion.identity);

            Transform target = null;
            Vector3 vec = Vector3.zero;
            if (_enemyBase.TargetCharacter!= null)
            {
                target = _enemyBase.TargetCharacter.GetNearestPart(transform);
            }
            if(target != null)
            {
                vec = ((target.position - _breathPosition.position).normalized + Vector3.up * 0.7f + _breathPosition.forward * 0.5f).normalized;
            }

            float dot = Vector3.Dot(vec, _breathPosition.forward);
            if(dot > 0.1f)
            {
                Debug.Log("Fire on Vec");
                fireBall.transform.forward = vec;
            }
            else
            {
                fireBall.transform.forward = _breathPosition.forward;
            }
            if (fireBall.TryGetComponent<Projectiles>(out var projectile))
            {
                projectile.Initialize(
                    (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Fire_Head)));
            }
        }
        else
        {
            GameObject breath = _fireBallPool.Get(_breathPosition.position);
            Transform target = null;
            Vector3 vec = Vector3.zero;
            if (_enemyBase.TargetCharacter != null)
            {
                target = _enemyBase.TargetCharacter.GetNearestPart(transform);
            }
            if (target != null)
            {
                vec = (target.position - _breathPosition.position + Vector3.up * 0.7f).normalized;
            }

            float dot = Vector3.Dot(vec, _breathPosition.forward);
            if (dot > 0.4f)
            {
                breath.transform.forward = vec;
            }
            else
            {
                breath.transform.forward = _breathPosition.forward;
            }
            if (breath.TryGetComponent<FireBall>(out var projectile))
            {
                projectile.Initialize(
                    (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Fire_Head)),
                    (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Fire_Remain)),
                    _embersPools[1]);
            }
        }
    }
    public void StartBreath()
    {
        _flameStream.Play();
        _fireEmbers.Play();
        _isBreath = true;
        StopSound();
        _audioSource = AudioManager.Instance.PlaySound((int)AudioManager.DemonSound.Breath, transform);
    }

    public void StartTailAttack()
    {
        foreach (Collider col in _tailColliders)
        {
            col.enabled = true;
        }

        foreach (Collider col in _rushColliders)
        {
            col.enabled = true;
        }
        _state = State.Action;
        _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Tail));
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.TailAttack, transform);
    }

    public void StartRush()
    {
        _agent.speed = _enemyBase.Agility * _rushSpeed;
        _agent.velocity = transform.forward * _agent.speed * 0.05f;
        _isRush = true;
        _agent.SetDestination(_rushVector);
        foreach(Collider col in _rushColliders)
        {
            col.enabled = true;
        }
        _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Rush));
    }

    public void StartExplosionTackle()
    {
        _agent.speed = _enemyBase.Agility * _explosionTackleSpeed;
        _agent.velocity = (_rushVector- transform.position).normalized * _agent.speed * 0.5f;
        _stateLock = false;
        _agent.SetDestination(_rushVector);
        _agent.updatePosition = true;
        foreach (Collider col in _rushColliders)
        {
            col.enabled = true;
        }
        _power = (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Landing_Direct));
    }

    public void FinishExplosionTackle()
    {
        _agent.speed = _enemyBase.Agility;
        _agent.updateRotation = true;
        _agent.SetDestination(transform.position);
    }

    public void Roar()
    {
        _roarIntensity = 0.8f;
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Roar, transform);
        SetMotor(_roarIntensity, _roarIntensity);
        _vibrationTimer = new Timer(StopVibration, 0.8f);
    }

    private void FinishRush()
    {
        _agent.speed = _enemyBase.Agility;
        _isRush = false;
        _animator.SetTrigger("Rush");
    }

    public override void FinishAttack()
    {
        foreach (var collider in _rushColliders)
        {
            collider.enabled = false;
        }
        _handAttackCollider.enabled = false;
        foreach (var collider in _tailColliders)
        {
            collider.enabled = false;
        }

        _flameStream.Stop();
        _fireEmbers.Stop();
        _isBreath = false;
        StopSound();
        _agent.updateRotation = true;
        _agent.updatePosition = true;
        _power = 0;
    }

    public override void FinishAction()
    {
        base.FinishAction();
        if(_willChangeSky)
        {
            IgnitEventMove();
        }
        else if(_stateLock)
        {
            IgnitTacckle();
        }
    }

    public override void Attack(Collider collider)
    {
        if (collider.tag == "Player" || collider.tag == "Summoned")
        {
            CharacterBase character = collider.GetComponentInParent<CharacterBase>();
            if (character != null)
            {
                int damage = character.Damaged(_power, _enemyBase.Break, _enemyBase.Appearance, _enemyBase);
                if (damage > 0)
                {
                    float time = 18.0f;
                    float forcePower = 550.0f;
                    float powerMagni = Mathf.Clamp01((damage + 40.0f) / 100.0f);
                    Vector3 forceVec = (collider.transform.position - transform.position);
                    character.KnockBack(forceVec, forcePower * powerMagni, time * powerMagni);
                    HitEffectManager.Instance.Play(HitEffectManager.Type.Hit, collider.ClosestPointOnBounds(transform.position));
                }
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        _animator.SetTrigger("Event");
        FinishAttack();
    }

    void FixedUpdate()
    {
        if(_roarIntensity > 0)
        {
            _shaderManager.SetIntensity(_firePosition.position, _roarIntensity);
            _roarIntensity *= 0.95f;
        }
        if(_vibrationTimer != null)
        {
            _vibrationTimer.CountUp(Time.fixedDeltaTime);
        }
    }

    protected override void Combat()
    {
        _agent.SetDestination(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position);
        SelectAttack();
        _timer += Time.deltaTime;
    }
    protected override void Action()
    {
        if(_isRush)
        {
            if((_rushVector - transform.position).sqrMagnitude < 9.0f ||
                _agent.velocity.sqrMagnitude <= 0)
            {
                FinishRush();
            }
        }
        else if(_isBreath)
        {
            if(_breathTimer < _breathCoolTime)
            {
                _breathTimer += Time.deltaTime;
            }
            else
            {
                _breathTimer = 0.0f;
                GameObject breath = _breathPool.Get(_firePosition.position);
                float rangeAdjust = 0.4f;
                Vector3 forward = (_firePosition.forward + transform.forward * rangeAdjust).normalized;
                breath.transform.forward = forward;
                if (breath.TryGetComponent<FireBall>(out var projectile))
                {
                    projectile.Initialize(
                        (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Fire_Body)),
                        (int)(_enemyBase.Attack * _enemyBase.GetPowerMagnification((int)Skill.Fire_Remain)),
                        _embersPools[0]);
                }
            }
        }
    }

    private void SelectAttack()
    {
        float dot = _enemyBase.GetDot();
        if (dot < 0) { return; }
        float sqrDistance = _enemyBase.GetDistance();
        float buffar = 2.0f;
        float borderDistance = _enemyBase.StopDistance + buffar;
        borderDistance *= borderDistance;
        if (sqrDistance > borderDistance)
        {
            if (_timer > _enemyBase.CoolTime)
            {
                _timer = 0.0f;
                SelectFarAttack(dot, sqrDistance);
            }
        }
        else
        {
            if (sqrDistance < _enemyBase.StopDistance * _enemyBase.StopDistance)
            { _agent.velocity = Vector3.zero; }
                
            if (_timer > _enemyBase.CoolTime)
            {
                _timer = 0.0f;
                SelectNearAttack(dot);
            }
        }
    }

    private void SelectFarAttack(float dot, float sqrDistance)
    {
        if(dot < 0.866f) { return; }

        _agent.velocity = Vector3.zero;
        _agent.SetDestination(transform.position);
        _state = State.Action;
        int judge;
        if (dot < 0.9659f)
        {
            judge = Random.Range(1, 4);
        }
        else
        {
            if(sqrDistance < 500.0f)
            {
                judge = Random.Range(0, 4);
            }
            else
            {
                judge = Random.Range(0, 2) * 3;
            }
        }
        NavMeshPath path = new NavMeshPath();
        NavMeshHit navMeshHit;
        switch (judge)
        {
            case 0:
                _animator.SetTrigger("Ball");
                Transform target = _enemyBase.TargetCharacter.GetNearestPart(this.transform);
                _targetPosition = target.position;
                _agent.updateRotation = false;
                break;
            case 1:
                _animator.SetTrigger("Breath");
                AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Growl, transform);
                _agent.updateRotation = false;
                break;
            case 2:
                if (NavMesh.SamplePosition(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position +
                    (_enemyBase.TargetCharacter.GetNearestPart(this.transform).position - transform.position).normalized * 10.0f,
                    out navMeshHit, 10.0f, NavMesh.AllAreas))
                {
                    _rushVector = navMeshHit.position;
                    _animator.SetTrigger("Rush");
                    AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Growl, transform);
                }
                else
                {
                    _state = State.Combat;
                }
                break;
            case 3:

#if UNITY_EDITOR
#else
                if (_enemyBase.Hp > _enemyBase.MaxHp * 0.5f)
                {
                    Debug.Log("Re Far");
                    SelectFarAttack(dot, sqrDistance);
                    break;
                }
                
#endif
                if (NavMesh.SamplePosition(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position,
                    out navMeshHit, 10.0f, NavMesh.AllAreas))
                {
                    if(NavMesh.CalculatePath(transform.position,navMeshHit.position, NavMesh.AllAreas,path))
                    {
                        _rushVector = path.corners[path.corners.Length - 1];
                    }
                    else
                    {
                        _rushVector = navMeshHit.position;
                    }
                    Debug.Log(path.status);
                    _agent.updateRotation = false;
                    _animator.SetTrigger("Explosion");
                }
                else
                {
                    _state = State.Combat;
                }
                break;
        }
    }

    private void SelectNearAttack(float dot)
    {
        _agent.SetDestination(transform.position);
        _state = State.Action;
        int judge;
        if (dot < 0.7071f)
        {
            judge = Random.Range(3, 6);
        }
        else if(dot < 0.9397f)
        {
            judge = Random.Range(1, 6);
        }
        else
        {
            judge = Random.Range(0, 6);
        }
        

        NavMeshHit navMeshHit;
        switch (judge)
        {
            case 0:
                _animator.SetTrigger("Bite");
                _agent.updateRotation = false;
                break;
            case 1:
                _animator.SetTrigger("Attack");
                _agent.updateRotation = false;
                break;
            case 2:
                _animator.SetTrigger("Breath");
                AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Growl, transform);
                _agent.updateRotation = false;
                break;
            case 3:
                _animator.SetTrigger("Tail");
                AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Growl, transform);
                _agent.updateRotation = false;
                break;
            case 4:
                
                if (NavMesh.SamplePosition(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position +
                    (_enemyBase.TargetCharacter.GetNearestPart(this.transform).position - transform.position).normalized * 12.0f,
                    out navMeshHit, 10.0f, NavMesh.AllAreas))
                {
                    _rushVector = navMeshHit.position;
                    _animator.SetTrigger("Rush");
                    AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.Growl, transform);
                }
                else
                {
                    _state = State.Combat;
                }
                break;
            case 5:
#if UNITY_EDITOR
#else
                if (_enemyBase.Hp > _enemyBase.MaxHp * 0.5f)
                {
                    Debug.Log("Re Near");
                    SelectNearAttack(dot);
                    break;
                }
#endif
                if (NavMesh.SamplePosition(_enemyBase.TargetCharacter.GetNearestPart(this.transform).position,
                    out navMeshHit, 10.0f, NavMesh.AllAreas))
                {
                    _rushVector = navMeshHit.position;
                    _agent.updateRotation = false;
                    _animator.SetTrigger("Explosion");
                }
                else
                {
                    _state = State.Combat;
                }
                break;
        }
    }

    private void StopSound()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource = null;
        }
    }
    private void SetSkyBox(Material sky)
    {
        Debug.Log("Change!");
        // 環境光のライティング設定
        // ソースをSkyboxに変更する
        RenderSettings.ambientMode = AmbientMode.Skybox;
        // スカイボックスのマテリアルを設定する
        RenderSettings.skybox = sky;
        // 光の強度を1に設定する
        RenderSettings.ambientIntensity = 1.0f;

        //// 環境光の反射設定
        //// ソースをSkyboxに変更する
        //RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
        //// 解像度を設定する
        //RenderSettings.defaultReflectionResolution = 128;
        //// 反射の強度を1に設定する
        //RenderSettings.reflectionIntensity = 1.0f;
        //// 反射の回数を1に設定する
        //RenderSettings.reflectionBounces = 1;

        // 環境光の更新(Skyboxマテリアル更新のため）
        DynamicGI.UpdateEnvironment();

    }
    private IEnumerator ChangeAmbient()
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        while (_blue < 0.65f)
        {
            _blue += 0.05f;
            _green += 0.03f;
            _red -= 0.005f;
            RenderSettings.ambientLight = new Color(_red, _green, _blue);
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
            yield return new WaitForSeconds(0.03f);
        }
    }

    private void StopVibration()
    {
        InputSystem.ResetHaptics();
        _vibrationTimer = null;
    }

    private void SetMotor(float low, float high)
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) { return; }
        gamepad.SetMotorSpeeds(low, high);
    }
}
