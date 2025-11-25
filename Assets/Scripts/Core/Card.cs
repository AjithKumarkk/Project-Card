using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Simple UI card: front/back images, flip animation, basic events.
public class Card : MonoBehaviour
{
    public int id = -1;
    public Image frontImage;
    public Image backImage;
    public float flipTime = 0.18f;

    // Simple callbacks you can set from other scripts
    public Action<Card> OnRevealed;
    public Action<Card> OnHidden;
    public Action<Card> OnMatched;

    public bool IsRevealed { get; private set; }
    public bool IsMatched { get; private set; }

    bool _animating = false;

    // Show front (with small flip)
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

    // Mark matched (keeps front shown)
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

        // shrink X
        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, 0f, t / half);
            transform.localScale = new Vector3(s, start.y, start.z);
            yield return null;
        }

        // swap images
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

        // expand X
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

    // reset for reuse
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

    // helper to be called from Button onClick
    public void OnClicked()
    {
        Reveal();
    }
}
