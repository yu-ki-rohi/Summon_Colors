using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] private CharacterData _characterData;

    private int _currentHp;

    public int Hp { get {  return _currentHp; } }
    public int MaxHp { get {  return _characterData.MaxHp; } }
    public int Attack { get { return _characterData.Attack; } }
    public int Vitality { get { return _characterData.Vitality; } }
    public float Agility { get {  return _characterData.Agility; } }


    // Start is called before the first frame update
    void Start()
    {
        _currentHp = _characterData.MaxHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
