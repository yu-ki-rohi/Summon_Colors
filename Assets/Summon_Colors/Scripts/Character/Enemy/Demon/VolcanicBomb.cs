using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                characterBase.Damaged(_power);
            }
        }
    }
}
