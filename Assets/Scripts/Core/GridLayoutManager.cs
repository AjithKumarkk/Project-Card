using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spawns cards in a grid inside a UI RectTransform container.
/// Uses CardPool to reuse card prefabs. Assigns pair ids and face sprites.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class GridLayoutManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform container; // UI panel area to place cards (assign)
    public CardPool pool;           // reference to CardPool (assign)
    public Sprite cardBackSprite;   // back sprite for all cards
    public List<Sprite> faceSprites = new List<Sprite>(); // faces for pairs

    [Header("Layout")]
    public float padding = 8f; // spacing between cards
    public int rows = 4;
    public int cols = 4;

    private List<Card> activeCards = new List<Card>();

    public IReadOnlyList<Card> ActiveCards => activeCards;
    void Start()
    {
        GenerateGrid(rows, cols, seed: 12345);
    }
    public void GenerateGrid(int rows, int cols, int seed = 0)
    {
        this.rows = rows;
        this.cols = cols;

        // return existing cards to pool
        if (pool != null && activeCards.Count > 0)
        {
            pool.ReturnAll(activeCards);
            activeCards.Clear();
        }

        int total = rows * cols;
        if (total <= 0)
        {
            Debug.LogWarning("GridLayoutManager: rows * cols must be > 0");
            return;
        }

        // Ensure even total (pairs). If odd, reduce one slot.
        if (total % 2 != 0)
        {
            Debug.LogWarning("Total cells is odd - reducing by 1 to make pairs.");
            total -= 1;
        }

        // Build id list: two of each id [0 .. pairCount-1]
        int pairCount = total / 2;
        List<int> ids = new List<int>(total);
        for (int i = 0; i < pairCount; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        // Shuffle ids
        System.Random rng = seed == 0 ? new System.Random() : new System.Random(seed);
        for (int i = ids.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            int tmp = ids[i];
            ids[i] = ids[j];
            ids[j] = tmp;
        }

        // compute card size to fit container
        float containerW = container.rect.width;
        float containerH = container.rect.height;

        float cardW = (containerW - (cols + 1) * padding) / cols;
        float cardH = (containerH - (rows + 1) * padding) / rows;
        float cardSize = Mathf.Max(8f, Mathf.Min(cardW, cardH)); // clamp min size

        // Starting positions 
        float startX = -containerW / 2f + padding + cardSize / 2f;
        float startY = containerH / 2f - padding - cardSize / 2f;

        int idx = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (idx >= ids.Count) break;

                Card card = pool.Get();
                card.transform.SetParent(container, false);

                RectTransform rt = card.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(cardSize, cardSize);

                float posX = startX + c * (cardSize + padding);
                float posY = startY - r * (cardSize + padding);
                rt.anchoredPosition = new Vector2(posX, posY);

                int assignedId = ids[idx];
                card.id = assignedId;

                // assign face sprite 
                if (faceSprites != null && faceSprites.Count > 0)
                    card.SetFaceSprite(faceSprites[assignedId % faceSprites.Count]);
                else
                    card.SetFaceSprite(null);

                // assign back sprite
                card.SetBackSprite(cardBackSprite);

                activeCards.Add(card);
                idx++;
            }
        }

    }

    public void ClearGrid()
    {
        if (pool != null)
        {
            pool.ReturnAll(activeCards);
            activeCards.Clear();
        }
    }
}
