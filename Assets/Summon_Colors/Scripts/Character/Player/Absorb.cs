using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static ColorElements;

[RequireComponent(typeof(Player))]
public class Absorb : MonoBehaviour
{
    [SerializeField] private ObjectPoolBase _pool;
    [SerializeField] private ObjectPoolBase _bulletPool;
    [SerializeField] private GameObject _absorbBullet;
    [SerializeField] private Transform _firePosition;
    [SerializeField] private Image[] _GemIcon = new Image[3];
    private Player _player;
    private ColorElements _colorElements = new ColorElements();
    private Timer _shootTimer;

    public ObjectPoolBase Pool { get { return _pool; } }
    public PlayerActionController ActionController { get { return _player.ActionController;} }
    public bool IsAbsorbing()
    {
        return _player.ActionController.IsAbsorbing();
    }

    public int GetPower()
    {
        return _player.AbsorbPower;
    }

    public void AddColor(ColorElements.ColorType colorType, int value)
    {
        _colorElements.Add(colorType, value);
        ReflectGemIcon();
    }

    public int ReduceColor(ColorElements.ColorType colorType, int value, bool exhoust = false)
    {
        int diff = _colorElements.Reduce(colorType, value, exhoust);
        ReflectGemIcon();
        return diff;
    }

    public void Shoot()
    {
#if true
        GameObject bullet = _bulletPool.Get(_firePosition.position);
#else
        GameObject bullet = Instantiate(_absorbBullet, _firePosition.position, Quaternion.identity);
#endif
        if (bullet.TryGetComponent<AbsorbBullet>(out var absorbBullet))
        {
            float up = 0;
            float right = 0;
            if (_player.Accuracy!=0.0f)
            {
                up = Random.Range(-1.0f / _player.Accuracy, 1.0f / _player.Accuracy);
                right = Random.Range(-1.0f / _player.Accuracy, 1.0f / _player.Accuracy);
            }

            absorbBullet.Initialize(this, transform.forward + Vector3.up * up + Vector3.right * right);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _player = this.gameObject.GetComponent<Player>();
        if ( _player == null )
        {
            Debug.LogError("Player is Null!!");
            return;
        }
        _colorElements.Blue = _player.ColorCapacity;
        _colorElements.Red = _player.ColorCapacity;
        _colorElements.Yellow = _player.ColorCapacity;

        for(int i = 0; i < _GemIcon.Length; i++)
        {
            _GemIcon[i].fillAmount = 0.0f;
        }

        if(_player.RateOfFire > 0.0f)
        {
            _shootTimer = new Timer(Shoot, 1.0f / _player.RateOfFire);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsAbsorbing())
        {
            _shootTimer.CountUp(Time.deltaTime);
        }
    }

    private void ReflectGemIcon()
    {
        for(int i = 0; i < _GemIcon.Length; i++)
        {
            _GemIcon[i].fillAmount = _colorElements.GetRemaining((ColorType)i);
        }
    }
}
