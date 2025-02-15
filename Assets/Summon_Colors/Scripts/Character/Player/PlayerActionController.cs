using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static ColorElements;

[RequireComponent(typeof(Absorb))]
[RequireComponent(typeof(Summon))]
[RequireComponent(typeof(Direction))]
public class PlayerActionController : MonoBehaviour
{
    enum State
    {
        Idle,
        Summon,
        Absorb,
        Direction,
        Avoid,
        Throw,
        Throw_Prepare,
        Prepare
    }

    [SerializeField] private CameraMove _cameraMove;
    [SerializeField] private ColorPalette _colorPalette;
    [SerializeField] private ColorPalette _lightPalette;
    [SerializeField, Range(0.0f,1.0f)] private float _directionDelayScale = 0.5f;
    [SerializeField, Range(0.0f,1.0f)] private float _changeDelayScale = 0.5f;
    [SerializeField] private Transform _initTransform;
    [SerializeField] private GameObject _bloomObject;
    [SerializeField] private ParticleSystem _particleSystem01;
    [SerializeField] private ParticleSystem _particleSystem02;
    [SerializeField] private TrailRenderer _trailRenderer;
    private PlayerMove _playerMove;
    private Absorb _absorb;
    private Summon _summon;
    private Direction _direction;
    private State _state = State.Idle;
    private bool _isChangingColor = false;
    private bool _canMove = true;
    private Animator _animator;
    private Rigidbody _rigidbody;
    private Timer _knockBackTimer;
    private bool _isLookAtCameraTarget = false;
    private AudioSource _audioSource;
    private Timer _stayVoiceTimer;

    public ColorElements.ColorType Color { get { return _summon.Color; } }
    public bool IsAbsorbing()
    {
        return _state == State.Absorb;
    }

    public bool IsSummoning()
    {
        return _state == State.Summon;
    }

    public void FinishSummon()
    {
        _animator.SetBool("Summon", false);
    }

    public bool IsBoolAnimation()
    {
        if (_state == State.Avoid || 
            _state == State.Throw ||
            _state == State.Throw_Prepare)
        {
            return false;
        }
        return true;
    }

    public void CreateStayVoiceTimer()
    {
        if (_canMove &&
            _stayVoiceTimer == null &&
           !InGameManager.Instance.IsClear
           && !InGameManager.Instance.IsEvent)
        {
            Debug.Log("Create Voice Timer");
            _stayVoiceTimer = new Timer(PlayStayVoice, 5.0f);
        }
    }

    public void ChangeToIdle()
    {
        _state = State.Idle;
        _canMove = true;
        _isLookAtCameraTarget = false;
        _animator.SetBool("Summon", false);
        _animator.SetBool("Absorb", false);
        _animator.SetBool("Order", false);
        StopSound();
        ViewBloom(false);
        _summon.StopSummon();
    }
    public void ChangeToSummon()
    {
        if (_state == State.Direction)
        {
            _animator.SetBool("Order", false);
            Time.timeScale = 1.0f;
            _cameraMove.ChangeTarget(transform, false);
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
        }
        _state = State.Summon;
        StopSound();
        _audioSource = AudioManager.Instance.PlaySound((int)AudioManager.PlayerSound.Summon, transform);
    }

    public void ChangeToThrow()
    {
        if (_state == State.Direction)
        {
            _animator.SetBool("Order", false);
            Time.timeScale = 1.0f;
            _cameraMove.ChangeTarget(transform, false);
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
        }
        _state = State.Throw;
        _isLookAtCameraTarget = true;
    }

    public void ChangeToAbsorb()
    {
        if (_state == State.Direction)
        {
            _animator.SetBool("Order", false);
            Time.timeScale = 1.0f;
            _cameraMove.ChangeTarget(transform, false);
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
        }
        _state = State.Absorb;
    }

    public void ViewBloom(bool flag)
    {
        if (flag) { SetBloom(); }
        _bloomObject.SetActive(flag);
    }

    public void ThrowObject()
    {
        _playerMove.ThrowItem();
        _isLookAtCameraTarget = false;
        AudioManager.Instance.PlaySoundOneShot((int)AudioManager.PlayerSound.Throw, transform);
    }

    public void KnockBack(Vector3 dir, float strength, float time)
    {
        _canMove = false;
        _knockBackTimer = new Timer(FinishKnockBack, time);
        dir.y = 0;
        if(dir.sqrMagnitude != 1.0f)
        {
            dir = dir.normalized;
        }
        _rigidbody.AddForce(dir * strength, ForceMode.Impulse);
    }

    public void OnDie()
    {
        _canMove = false;
    }

    public void OnMoveCamera(InputAction.CallbackContext context)
    {
        var stick = context.ReadValue<Vector2>();
        if (_isChangingColor && 
            (_state == State.Direction || _state == State.Idle))
        {
            _summon.ChangeColors(stick);
            _lightPalette.ReflectStick(stick);
            _cameraMove.MoveCamera(Vector2.zero);
            _direction.Direct(Vector2.zero);
        }
        else if (_state == State.Direction)
        {
            _cameraMove.MoveCamera(Vector2.zero);
            _direction.Direct(stick);
        }
        else
        {
            _cameraMove.MoveCamera(stick);
            _direction.Direct(Vector2.zero);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var stick = context.ReadValue<Vector2>();

        if(_canMove)
        {
            _playerMove.Move(stick);
            if(stick != Vector2.zero)
            {
                _stayVoiceTimer = null;
            }
        }
        else
        {
            _playerMove.Move(Vector2.zero); 
        }
    }

    public void OnAbsorb(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(_state == State.Idle)
            {
                _canMove = false;
                _playerMove.Move(Vector2.zero);

                _animator.SetFloat("Speed", 0.0f);
                _animator.SetBool("Absorb", true);

                _absorb.SetShootDirection(Camera.main.transform.forward);

                StopSound();
                _audioSource = AudioManager.Instance.PlaySound((int)AudioManager.PlayerSound.Vacuum, transform);

                _state = State.Prepare;

                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0.0f;
                gameObject.transform.forward = forward.normalized;
                _stayVoiceTimer = null;
            }
        }
        else if (context.canceled)
        {
            _animator.SetBool("Absorb", false);
            StopSound();
        }
    }

    public void OnSummon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_state == State.Idle)
            {
                _canMove = false;
                _animator.SetBool("Summon", true);
                _playerMove.Move(Vector2.zero);
                _animator.SetFloat("Speed", 0.0f);
                _state = State.Prepare;
                AudioManager.Instance.PlayRandomVoice((int)AudioManager.Voice.Summon01, 2, transform);
                _summon.StartSummon();
                _stayVoiceTimer = null;
                ViewBloom(true);
            }
        }
        else if (context.canceled)
        {
            _animator.SetBool("Summon", false);
            StopSound();
            _summon.StopSummon();
            ViewBloom(false);
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_state == State.Idle)
            {
                _canMove = false;
                _state = State.Throw_Prepare;
                _animator.SetTrigger("Throw");
                _playerMove.Move(Vector2.zero);
                _animator.SetFloat("Speed", 0.0f);
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0.0f;
                gameObject.transform.forward = forward.normalized;
                AudioManager.Instance.PlayRandomVoice((int)AudioManager.Voice.Attack01, 2, transform);
                _stayVoiceTimer = null;
            }
        }
        else if (context.canceled)
        {
            
        }
    }

    public void OnAvoid(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            if (_state == State.Avoid)
            {
                return;
            }
            _state = State.Avoid;
            _playerMove.Move(Vector2.zero);
            _animator.SetFloat("Speed", 0.0f);
            _animator.SetTrigger("Avoid");
            _animator.SetBool("Summon", false); 
            _animator.SetBool("Absorb", false);
            _animator.SetBool("Order", false);
            _canMove = false;
            ViewBloom(false);
            Time.timeScale = 1.0f;
            _cameraMove.ChangeTarget(transform, false);
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
            AudioManager.Instance.PlayRandomVoice((int)AudioManager.Voice.Evasion01, 2, transform);
            _stayVoiceTimer = null;
        }
    }

    public void OnDirect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           
            if (_state == State.Idle)
            {
                _state = State.Direction;
                _animator.SetBool("Order",true);
                _canMove = false;
                Time.timeScale = _directionDelayScale;
                _cameraMove.ChangeTarget(_summon.GetHomeBase(_summon.Color).transform, true);
                Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("UI"));
                _stayVoiceTimer = null;
                ViewBloom(true);
            }
        }
        else if (context.canceled)
        {
            if (_state == State.Direction)
            {
                _animator.SetBool("Order", false);
                ChangeToIdle();
                Time.timeScale = 1.0f;
                _cameraMove.ChangeTarget(transform, false);
                Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
                ViewBloom(false);
            }
        }
    }

    public void OnReturn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_state == State.Direction)
            {
                _direction.Return();
            }
            else
            {
               
            }
        }
        else if (context.canceled)
        {

        }
    }
     public void OnCallBack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_state == State.Direction)
            {
                _direction.CallBack();
            }
            else
            {
               
            }
        }
        else if (context.canceled)
        {

        }
    }

    public void OnChange(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_state == State.Direction || _state == State.Idle)
            {
                _isChangingColor = true;
                Time.timeScale = _changeDelayScale;
                _colorPalette.DisplayColorPalette();
                _lightPalette.DisplayColorPalette();
                _lightPalette.TurnOffLight();
                ViewBloom(true);
            }
            
        }
        else if (context.canceled)
        {
            if (_state != State.Direction)
            {
                Time.timeScale = 1.0f;
                ViewBloom(false);
            }
            else
            {
                Time.timeScale = _directionDelayScale;
                _cameraMove.ChangeTarget(_summon.GetHomeBase(_summon.Color).transform, true);
            }
            
            _lightPalette.ReflectStick(Vector2.zero);
            _isChangingColor = false;
            _colorPalette.HideColorPalette();
            _lightPalette.HideColorPalette();
        }
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (InGameManager.Instance.IsClear) { return; }
            InGameManager.Instance.ChangePause();
        }
        else if (context.canceled)
        {
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
        _absorb = GetComponent<Absorb>();
        _summon = GetComponent<Summon>();
        _direction = GetComponent<Direction>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        ViewBloom(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(_knockBackTimer != null)
        {
            _knockBackTimer.CountUp(Time.deltaTime);
        }
        // Changed
        if(_state != State.Throw)
        {
            _isLookAtCameraTarget = false;
        }
        if(_isLookAtCameraTarget)
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0.0f;
            gameObject.transform.forward = forward.normalized;
        }
        if (transform.position.y < -50.0f)
        {
            if(_initTransform != null)
            {
                transform.position =_initTransform.position;
            }
            _rigidbody.velocity = Vector3.zero;
        }
        if(InGameManager.Instance.IsGameOver)
        {
            _isLookAtCameraTarget = false;
            _canMove = true;
        }
        if(_stayVoiceTimer != null)
        {
            _stayVoiceTimer.CountUp();
        }
       
    }

    private void FinishKnockBack()
    {
        if(_state == State.Idle)
        {
            _canMove = true;
        }
        _knockBackTimer = null;
    }

    private void StopSound()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource = null;
        }
    }

    private void PlayStayVoice()
    {
        AudioManager.Instance.PlayRandomVoice((int)AudioManager.Voice.Stay01, 4, transform);
    }
    private void SetBloom()
    {
        Color color = UnityEngine.Color.black;
        switch (_summon.Color)
        {
            case ColorElements.ColorType.Blue:
                color = UnityEngine.Color.blue;
                break;
            case ColorElements.ColorType.Red:
                color = UnityEngine.Color.red;
                break;
            case ColorElements.ColorType.Yellow:
                color = UnityEngine.Color.yellow;
                break;
            case ColorElements.ColorType.Orange:
                color = new Color(1, 0.3f, 0, 1);
                break;
            case ColorElements.ColorType.Green:
                color = UnityEngine.Color.green;
                break;
            case ColorElements.ColorType.Violet:
                color = new Color(0.9f, 0, 0.9f, 1);
                break;
        }
        var main = _particleSystem01.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color);
        main = _particleSystem02.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color);
        _trailRenderer.startColor = color;
    }
}
