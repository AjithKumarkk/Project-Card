using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MatchManager : MonoBehaviour
{
    public float mismatchDelay = 0.6f;
    public Action<Card, Card, bool> OnPairResolved; // a,b,isMatch

    Queue<Card> queue = new Queue<Card>();
    List<Card> pendingHide = new List<Card>();
    bool running = false;

    public void EnqueueWhenRevealed(Card c)
    {
        if (c == null || c.IsMatched) return;
        if (queue.Contains(c)) return;
        if (pendingHide.Contains(c)) return;
        queue.Enqueue(c);

        if (!running) { running = true; StartCoroutine(Worker()); }
    }

    IEnumerator Worker()
    {
        while (queue.Count >= 2)
        {
            var a = queue.Dequeue();
            var b = queue.Dequeue();

            if (a == null || b == null) continue;
            if (a == b) continue;
            if (a.IsMatched || b.IsMatched) continue;

            bool isMatch = a.id == b.id;
            if (isMatch)
            {
                a.MarkMatched();
                b.MarkMatched();
                OnPairResolved?.Invoke(a, b, true);
            }
            else
            {
                pendingHide.Add(a);
                pendingHide.Add(b);
                OnPairResolved?.Invoke(a, b, false);
                StartCoroutine(HideAfter(a, b, mismatchDelay));
            }

            yield return null;
        }
        running = false;
    }

    IEnumerator HideAfter(Card a, Card b, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (a != null && !a.IsMatched) a.Hide();
        if (b != null && !b.IsMatched) b.Hide();
        pendingHide.Remove(a);
        pendingHide.Remove(b);
    }

    public void ResetAll()
    {
        StopAllCoroutines();
        queue.Clear();
        pendingHide.Clear();
        running = false;
    }
}
