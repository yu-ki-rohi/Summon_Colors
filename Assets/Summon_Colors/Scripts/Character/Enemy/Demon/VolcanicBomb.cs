using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class VolcanicBomb : Projectiles
{
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _flames;
    private int _embersPower = 3;
    private int _explosionPower;
    private AudioSource _audioSource;

    public void Initialize(int power, int explosionPower, int embersPower)
    {
        _explosionPower = explosionPower;
        _embersPower = embersPower;
        Initialize(power);
        _audioSource = AudioManager.Instance.PlaySound((int)AudioManager.DemonSound.Breath, transform);
    }

    protected override void BehaviorOnHitStage(Collider stage)
    {
        if(_explosion != null)
        {
            GameObject explosionObj = Instantiate(_explosion,transform.position,Quaternion.identity);
            if (explosionObj.TryGetComponent<Explosion>(out var explosion))
            {
                explosion.Initialize(_explosionPower);
            }
            AudioManager.Instance.PlaySoundOneShot((int)AudioManager.DemonSound.FireBall, transform);
        }
        if(_flames != null)
        {
            GameObject embersObj = Instantiate(_flames, stage.ClosestPointOnBounds(this.transform.position), Quaternion.identity);
            if (embersObj.TryGetComponent<Embers>(out var embers))
            {
                embers.Initialize(_embersPower);
            }
        }
        if(_audioSource != null)
        {
            _audioSource.Stop();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (_audioSource != null)
        {
            _audioSource.gameObject.transform.position = transform.position;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.tag == "Player" || other.tag == "Summoned")
        {
            CharacterBase characterBase = other.GetComponentInParent<CharacterBase>();
            if(characterBase != null)
            {
                int damage = characterBase.Damaged(_power);
                if (damage > 0)
                {
                    float time = 18.0f;
                    float forcePower = 1000.0f;
                    float powerMagni = Mathf.Clamp01((damage + 40.0f) / 100.0f);
                    Vector3 forceVec = (other.transform.position - transform.position);
                    characterBase.KnockBack(forceVec, forcePower * powerMagni, time * powerMagni);
                    HitEffectManager.Instance.Play(HitEffectManager.Type.Fire, other.ClosestPointOnBounds(transform.position));
                }
            }
        }
    }
}
