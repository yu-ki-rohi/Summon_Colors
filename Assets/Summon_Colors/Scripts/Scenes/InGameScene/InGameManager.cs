using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] InGameBase _inGameBase;
    // Start is called before the first frame update
    void Start()
    {
        _inGameBase.Start();
        AudioManager.Instance.PlayMusic(0);
    }

    // Update is called once per frame
    void Update()
    {
        _inGameBase.Update(Time.deltaTime);
    }
}
