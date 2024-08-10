using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class Absorb : MonoBehaviour
{
    [SerializeField] private ObjectPoolBase _pool;
    [SerializeField] private GameObject _absorbBullet;
    [SerializeField] private Transform _firePosition;
    [SerializeField] private Image[] _GemIcon = new Image[3];
    private Player _player;
    private ColorElements _colorElements = new ColorElements();

    public ObjectPoolBase Pool { get { return _pool; } }

    public bool IsAbsorbing()
    {
        return _player.IsAbsorbing();
    }

    public int GetPower()
    {
        return _player.AbsorbPower;
    }

    public void AddColor(ColorElements.ColorType colorType, int value)
    {
        _colorElements.Add(colorType, value);

        int id;
        switch(colorType)
        {
            case ColorElements.ColorType.Blue:
                id = 0;
                break;
            case ColorElements.ColorType.Red:
                id = 1;
                break;
            case ColorElements.ColorType.Yellow:
                id = 2;
                break;
            default:
                id = -1;
                break;
        }
        if(id >= 0)
        {
            _GemIcon[id].fillAmount = _colorElements.GetRemaining(colorType);
        }
    }

    public int ReduceColor(ColorElements.ColorType colorType, int value)
    {
        return _colorElements.Reduce(colorType, value);
    }

    public void OnAbsorb(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(_player.ChangeToAbsorb())
            {
                GameObject bullet = Instantiate(_absorbBullet, _firePosition.position, Quaternion.identity);
                if (bullet.TryGetComponent<AbsorbBullet>(out var absorbBullet))
                {
                    absorbBullet.Initialize(this, Camera.main.transform.forward);
                }
            }          
        }
        else if(context.canceled)
        {
            _player.TurnToIdle();
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
