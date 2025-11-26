using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int id = -1;
    public Image frontImage;
    public Image backImage;
    public float flipTime = 0.18f;

    public Action<Card> OnRevealed;
    public Action<Card> OnHidden;
    public Action<Card> OnMatched;

    public bool IsRevealed { get; private set; }
    public bool IsMatched { get; private set; }

    bool _animating = false;
    public void Reveal()
    {
        if (IsMatched || IsRevealed || _animating) return;
        StartCoroutine(DoFlip(true));
    }

    public void Hide()
    {
        if (IsMatched || !IsRevealed || _animating) return;
        StartCoroutine(DoFlip(false));
    }

    public void MarkMatched()
    {
        IsMatched = true;
        OnMatched?.Invoke(this);
    }

    IEnumerator DoFlip(bool toFront)
    {
        _animating = true;
        float half = flipTime / 2f;
        Vector3 start = transform.localScale;

        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, 0f, t / half);
            transform.localScale = new Vector3(s, start.y, start.z);
            yield return null;
        }

        if (toFront)
        {
            if (backImage) backImage.gameObject.SetActive(false);
            if (frontImage) frontImage.gameObject.SetActive(true);
            IsRevealed = true;
            OnRevealed?.Invoke(this);
        }
        else
        {
            if (frontImage) frontImage.gameObject.SetActive(false);
            if (backImage) backImage.gameObject.SetActive(true);
            IsRevealed = false;
            OnHidden?.Invoke(this);
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(0f, 1f, t / half);
            transform.localScale = new Vector3(s, start.y, start.z);
            yield return null;
        }

        transform.localScale = start;
        _animating = false;
    }

    public void ResetState()
    {
        StopAllCoroutines();
        IsRevealed = false;
        IsMatched = false;
        _animating = false;
        if (frontImage) frontImage.gameObject.SetActive(false);
        if (backImage) backImage.gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        OnRevealed = null;
        OnHidden = null;
        OnMatched = null;
    }

    public void OnClicked()
    {
        Reveal();
    }
}
