using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Queue-based match manager that allows continuous flipping.
/// - EnqueueRevealed(card) is called when a card finishes its reveal animation.
/// - Consumes pairs from the queue and resolves them (match or mismatch).
/// - Does not disable input while resolving pairs.
/// </summary>
public class MatchManager : MonoBehaviour
{
    [Tooltip("Delay before hiding mismatched cards (seconds)")]
    public float mismatchRevealDelay = 0.6f;

    // Event: (cardA, cardB, isMatch)
    public event Action<Card, Card, bool> OnPairResolved;

    // internal queue of revealed cards (in order of reveal completion)
    private Queue<Card> revealedQueue = new Queue<Card>();

    // track pending hide operations so we don't double-hide or re-enqueue
    private HashSet<Card> pendingHide = new HashSet<Card>();

    private bool consumerRunning = false;

    /// <summary>
    /// Call this when a card completes its reveal animation (Card.OnRevealed).
    /// Enqueues the card for pairing. Safe to call while resolving other pairs.
    /// </summary>
    public void EnqueueRevealed(Card card)
    {
        if (card == null) return;
        if (card.IsMatched) return;       // already matched — ignore
        if (revealedQueue.Contains(card)) return; // avoid duplicate enqueue
        if (pendingHide.Contains(card)) return;   // card is scheduled to hide — ignore

        revealedQueue.Enqueue(card);

        // start consumer if not already running
        if (!consumerRunning)
        {
            consumerRunning = true;
            StartCoroutine(ConsumerCoroutine());
        }
    }

    private IEnumerator ConsumerCoroutine()
    {
        // Keep running while we have at least two cards to evaluate
        while (revealedQueue.Count >= 2)
        {
            Card a = revealedQueue.Dequeue();
            Card b = revealedQueue.Dequeue();

            // Safety checks (they might have been matched/changed)
            if (a == null || b == null) continue;
            if (a == b) continue;
            if (a.IsMatched || b.IsMatched) continue;

            bool isMatch = a.id == b.id;

            if (isMatch)
            {
                // Immediately mark matched; this prevents them from being flipped again or hidden
                a.SetMatched();
                b.SetMatched();

                // emit event
                OnPairResolved?.Invoke(a, b, true);
            }
            else
            {
                // schedule hide after delay but do not block enqueuing further reveals
                pendingHide.Add(a);
                pendingHide.Add(b);

                OnPairResolved?.Invoke(a, b, false);

                // start hide coroutine (non-blocking for queue consumer)
                StartCoroutine(HideAfterDelay(a, b, mismatchRevealDelay));
            }

            // Yield one frame to let more reveals enqueue (keeps UI responsive)
            yield return null;
        }

        consumerRunning = false;
    }

    private IEnumerator HideAfterDelay(Card a, Card b, float delay)
    {
        yield return new WaitForSeconds(delay);

        // If they were matched in the meantime, don't hide
        if (a != null && !a.IsMatched)
        {
            a.Hide();
        }
        if (b != null && !b.IsMatched)
        {
            b.Hide();
        }

        pendingHide.Remove(a);
        pendingHide.Remove(b);
    }

    /// <summary>
    /// Useful when restarting/clearing the board: stop routines and clear internal queues/pending state.
    /// </summary>
    public void ResetState()
    {
        StopAllCoroutines();
        revealedQueue.Clear();
        pendingHide.Clear();
        consumerRunning = false;
    }

    // Debug helper - optional
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (mismatchRevealDelay < 0f) mismatchRevealDelay = 0.1f;
    }
    #endif
}
