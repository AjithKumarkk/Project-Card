using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Card : MonoBehaviour
{
    public int id;
    public Image frontImage; 
    public Image backImage; 
    public float flipDuration = 0.18f;

    public bool IsRevealed { get; private set; }
    public bool IsMatched { get; private set; }

    public event Action<Card> OnRevealed;
    public event Action<Card> OnHidden;
    public event Action<Card> OnMatched;

    private bool animating = false;

    private void Reset()
    {
        frontImage = transform.Find("Front")?.GetComponent<Image>();
        backImage = transform.Find("Back")?.GetComponent<Image>();
    }

    public void SetFaceSprite(Sprite s)
    {
        if (frontImage) frontImage.sprite = s;
    }

    public void SetBackSprite(Sprite s)
    {
        if (backImage) backImage.sprite = s;
    }

    public void SetMatched()
    {
        if (IsMatched) return;
        IsMatched = true;
        StopAllCoroutines();
        animating = false;
        StartCoroutine(MatchedScaleAnim());
        OnMatched?.Invoke(this);
    }

    private IEnumerator MatchedScaleAnim()
    {
        float t = 0f;
        Vector3 start = transform.localScale;
        Vector3 target = start * 1.12f;
        while (t < 0.12f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, target, t / 0.12f);
            yield return null;
        }
        t = 0f;
        while (t < 0.12f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(target, start, t / 0.12f);
            yield return null;
        }
    }

    public void Reveal()
    {
        if (IsMatched || IsRevealed || animating) return;
        StartCoroutine(FlipToFront());
    }

    public void Hide()
    {
        if (IsMatched || !IsRevealed || animating) return;
        StartCoroutine(FlipToBack());
    }

    private IEnumerator FlipToFront()
    {
        animating = true;
        float half = flipDuration / 2f;
        float t = 0f;
        Vector3 start = transform.localScale;
        while (t < half)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, 0f, t / half);
            transform.localScale = new Vector3(s, start.y, start.z);
            yield return null;
        }

        if (backImage) backImage.gameObject.SetActive(false);
        if (frontImage) frontImage.gameObject.SetActive(true);

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(0f, 1f, t / half);
            transform.localScale = new Vector3(s, start.y, start.z);
            yield return null;
        }

        transform.localScale = start;
        IsRevealed = true;
        animating = false;
        OnRevealed?.Invoke(this);
    }

    private IEnumerator FlipToBack()
    {
        animating = true;
        float half = flipDuration / 2f;
        float t = 0f;
        Vector3 start = transform.localScale;
        while (t < half)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, 0f, t / half);
            transform.localScale = new Vector3(s, start.y, start.z);
            yield return null;
        }

        if (frontImage) frontImage.gameObject.SetActive(false);
        if (backImage) backImage.gameObject.SetActive(true);

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(0f, 1f, t / half);
            transform.localScale = new Vector3(s, start.y, start.z);
            yield return null;
        }

        transform.localScale = start;
        IsRevealed = false;
        animating = false;
        OnHidden?.Invoke(this);
    }

    // Called by UI Button or EventTrigger
    public void OnClick()
    {
        if (IsMatched || animating) return;
        Reveal();
    }

    // Safety: reset visuals for reuse
    public void ResetState()
    {
        StopAllCoroutines();
        IsMatched = false;
        IsRevealed = false;
        animating = false;
        if (frontImage) frontImage.gameObject.SetActive(false);
        if (backImage) backImage.gameObject.SetActive(true);
        transform.localScale = Vector3.one;
    }
}
