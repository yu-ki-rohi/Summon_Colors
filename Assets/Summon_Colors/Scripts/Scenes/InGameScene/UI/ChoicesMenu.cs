using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ChoicesMenu
{
    public Image[] Fonts;
    public Image[] ChoiceFonts;
    public Image[] Cursors;
    private RectTransform[] _rectTransforms;
    private Animator[] _animators;

    public Animator GetCursorAnimator(int index)
    {
        if (index < 0 || index >= Cursors.Length) { return null; }
        return _animators[index];
    }

    public void ChoiceCursor(int index)
    {
        if (Cursors.Length == 0) { return; }
        Start();
        foreach (var c in Cursors)
        {
            c.enabled = false;
        }
        foreach (var a in _animators)
        {
            a.enabled = false;
        }
        if (index < 0 || index >= Cursors.Length) { return; }
        Cursors[index].enabled = true;
        _animators[index].enabled = true;
    }

    private void SetCursor(int index)
    {
        foreach (var c in Cursors)
        {
            c.enabled = false;
        }
        if (index < 0 || index >= Cursors.Length) { return; }
        Cursors[index].enabled = true;
    }
    private void SetAnimator(int index)
    {
        if(_animators == null) { return; }
        foreach (var a in _animators)
        {
            a.enabled = false;
        }
        if (index < 0 || index >= _animators.Length) { return; }
        _animators[index].enabled = true;
    }

    public void ChoiceScaling(int index)
    {
        foreach (var rt in _rectTransforms)
        {
            Vector2 scale = rt.localScale;
            scale.x = 1.0f;
            scale.y = 1.0f;
            rt.localScale = scale;
        }
        if (index < 0 || index >= _rectTransforms.Length) { return; }
        Vector2 scaling = _rectTransforms[index].localScale;
        scaling.x = 1.25f;
        scaling.y = 1.25f;
        _rectTransforms[index].localScale = scaling;
    }

    public void View(bool flag, int index = 0)
    {
        Start();
        foreach (var f in Fonts)
        {
            f.enabled = flag;
        }
        foreach (var cf in ChoiceFonts)
        {
            cf.enabled = flag;
        }
        if(!flag) { index = -1; }
        ChoiceCursor(index);
        ChoiceScaling(index);
    }

    public void Start()
    {
        SetRectTransforms();
        SetCursorAnimator();
    }
    private void SetRectTransforms()
    {
        if (_rectTransforms != null ||
            ChoiceFonts.Length == 0) { return; }

        _rectTransforms = new RectTransform[ChoiceFonts.Length];
        for (int i = 0; i < _rectTransforms.Length; i++)
        {
            _rectTransforms[i] = ChoiceFonts[i].gameObject.GetComponent<RectTransform>();
        }
    }
    private void SetCursorAnimator()
    {
        if (_animators != null ||
            Cursors.Length == 0) { return; }

        _animators = new Animator[Cursors.Length];
        for (int i = 0; i < _animators.Length; i++)
        {
            _animators[i] = Cursors[i].transform.parent.GetComponent<Animator>();
        }
    }
}
