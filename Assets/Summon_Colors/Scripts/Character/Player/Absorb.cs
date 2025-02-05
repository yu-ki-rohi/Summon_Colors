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
    private Vector3 _shootDir = Vector3.zero;

    private const float SHOOT_UP_LIM = 0.7071f;
    private const float SHOOT_DOWN_LIM = -0.5f;

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

            Vector3 cross = Vector3.Cross(_shootDir, Vector3.up);
            absorbBullet.Initialize(this, _shootDir + Vector3.Cross(_shootDir, cross) * up + cross * right);
        }
    }

    public void SetShootDirection(Vector3 direction)
    {
        if(direction.sqrMagnitude > 1.0f)
        {
            direction = direction.normalized;
        }
        _shootDir = ClampHeight(direction, SHOOT_DOWN_LIM, SHOOT_UP_LIM);
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

    private Vector3 ClampHeight(Vector3 value, float min, float max)
    {
        if (value.y < min)
        {
            return AdjustY(value, min);
        }
        else if (value.y > max)
        {
            return AdjustY(value, max);
        }
        return value;
    }

    private Vector3 AdjustY(Vector3 value, float valueY)
    {
        if (Mathf.Abs(value.y) == 1.0f) { return value; }

        float scalar = Mathf.Sqrt((1.0f - valueY * valueY) / (1.0f - value.y * value.y));
        value.x *= scalar;
        value.y = valueY;
        value.z *= scalar;
        if (value.sqrMagnitude != 1.0f)
        {
            value = value.normalized;
        }
        return value;
    }
}
