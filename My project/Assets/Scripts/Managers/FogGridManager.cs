using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 6;
    public int cols = 6;

    [Header("References")]
    public SpriteRenderer mapRenderer;
    public GameObject fogTilePrefab;
    public Transform fogParent;

    [Header("Accent Layers")]
    public int extraFogLayers = 2; // How many extra layers per tile
    public GameObject accentFogPrefab; // Optional: If empty, it reuses fogTilePrefab

    [Header("Start Area")]

    private FogTile[,] fogTiles;
    private List<FogTile> revealedTiles = new List<FogTile>();

    private float tileWidth;
    private float tileHeight;

    public float TileWidth => tileWidth;
    public float TileHeight => tileHeight;

    void Start()
    {
        GenerateFogGrid();
    }

    void GenerateFogGrid()
    {
        if (mapRenderer == null) return;

        Bounds mapBounds = mapRenderer.bounds;
        tileWidth = mapBounds.size.x / cols;
        tileHeight = mapBounds.size.y / rows;

        fogTiles = new FogTile[cols, rows];
        float startX = mapBounds.min.x;
        float startY = mapBounds.min.y;

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 worldPos = new Vector3(
                    startX + (x * tileWidth) + (tileWidth / 2f),
                    startY + (y * tileHeight) + (tileHeight / 2f),
                    -0.1f
                );

                GameObject tileObj = Instantiate(fogTilePrefab, worldPos, Quaternion.identity);

                SpriteRenderer tileSR = tileObj.GetComponent<SpriteRenderer>();

                if (tileSR != null)
                {
                    Color c = tileSR.color;
                    c.a = Random.Range(0.4f, 0.7f); // softer fog
                    tileSR.color = c;
                }
                if (tileSR != null && tileSR.sprite != null)
                {
                    float sW = tileSR.sprite.bounds.size.x;
                    float sH = tileSR.sprite.bounds.size.y;
                    tileObj.transform.localScale = new Vector3(tileWidth / sW, tileHeight / sH, 1f);
                }
                float randomScale = Random.Range(0.95f, 1.05f);
                tileObj.transform.localScale *= randomScale;

                tileObj.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

                if (fogParent != null) tileObj.transform.SetParent(fogParent, true);

                tileObj.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

                // --- NEW ACCENT LAYER LOGIC ---
                GameObject prefabToUse = accentFogPrefab != null ? accentFogPrefab : fogTilePrefab;

                for (int i = 0; i < extraFogLayers; i++)
                {
                    // 1. Calculate offset so it sits somewhere inside the parent tile
                    Vector3 offset = new Vector3(
                        Random.Range(-tileWidth * 0.3f, tileWidth * 0.3f),
                        Random.Range(-tileHeight * 0.3f, tileHeight * 0.3f),
                        -0.01f * (i + 1)
                    );

                    // 2. Instantiate UNPARENTED first
                    GameObject accentObj = Instantiate(prefabToUse, worldPos + offset, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));

                    SpriteRenderer accentSR = accentObj.GetComponent<SpriteRenderer>();
                    if (accentSR != null)
                    {
                        // Make accents softer/more transparent
                        Color c = accentSR.color;
                        c.a = Random.Range(0.2f, 0.4f);
                        accentSR.color = c;

                        if (accentSR.sprite != null)
                        {
                            float sW = accentSR.sprite.bounds.size.x;
                            float sH = accentSR.sprite.bounds.size.y;

                            // 3. Make it smaller than the parent (e.g., 30% to 70% of the tile size)
                            float sizeMod = Random.Range(0.3f, 0.7f);

                            // Set the scale while it is still in World Space
                            accentObj.transform.localScale = new Vector3(
                                (tileWidth / sW) * sizeMod,
                                (tileHeight / sH) * sizeMod,
                                1f
                            );
                        }
                    }
                

                    // 4. NOW parent it. By passing 'true', Unity automatically adjusts the localScale 
                    // so it doesn't blow up in size when it inherits the parent's scale.
                    accentObj.transform.SetParent(tileObj.transform, true);
                }

                if (fogParent != null) tileObj.transform.SetParent(fogParent, true);
                // Store grid coordinates (x, y) so we can find neighbors later
                fogTiles[x, y] = new FogTile
                {
                    obj = tileObj,
                    position = worldPos,
                    revealed = false,
                    gridX = x,
                    gridY = y
                };
            }
        }
    }

    // =========================
    // REVEAL LOGIC
    // =========================

    public void RevealArea(Vector3 pos, float radius)
    {
        foreach (var tile in fogTiles)
        {
            if (!tile.revealed && Vector2.Distance(tile.position, pos) <= radius)
            {
                RevealTile(tile);
            }
        }
    }

    public void RevealRandomAdjacentTile()
    {
        List<FogTile> candidates = new List<FogTile>();

        // Loop through all tiles
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Only consider hidden tiles
                if (!fogTiles[x, y].revealed)
                {
                    if (HasRevealedNeighbor(x, y))
                    {
                        candidates.Add(fogTiles[x, y]);
                    }
                }
            }
        }

        if (candidates.Count > 0)
        {
            RevealTile(candidates[Random.Range(0, candidates.Count)]);
        }
    }

    private bool HasRevealedNeighbor(int x, int y)
    {
        // Check 4 directions (Up, Down, Left, Right)
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            // Check if neighbor is within grid bounds
            if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
            {
                if (fogTiles[nx, ny].revealed) return true;
            }
        }
        return false;
    }

    public void RevealTile(FogTile tile)
    {
        if (tile == null || tile.revealed) return;

        StartCoroutine(FadeOutTile(tile.obj));
        tile.revealed = true;
        revealedTiles.Add(tile);
    }

    public IEnumerator FadeOutTile(GameObject obj, float duration = 0.5f)
    {
        // Get the parent's SpriteRenderer AND all children SpriteRenderers
        SpriteRenderer[] allRenderers = obj.GetComponentsInChildren<SpriteRenderer>();

        // Store their starting colors so we don't snap the alpha to 1 before fading
        Color[] startColors = new Color[allRenderers.Length];
        for (int i = 0; i < allRenderers.Length; i++)
        {
            startColors[i] = allRenderers[i].color;
        }

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float percent = t / duration;

            for (int i = 0; i < allRenderers.Length; i++)
            {
                if (allRenderers[i] != null)
                {
                    Color c = startColors[i];
                    // Lerp from their specific starting alpha down to 0
                    c.a = Mathf.Lerp(startColors[i].a, 0f, percent);
                    allRenderers[i].color = c;
                }
            }
            yield return null;
        }

        obj.SetActive(false);
    }
    public FogTile? FindTile(Transform target)
    {
        if (fogTiles == null || mapRenderer == null) return null;

        Bounds mapBounds = mapRenderer.bounds;
        Vector3 pos = target.position;

        // 1. Calculate the offset from the bottom-left corner (startX, startY)
        float offsetX = pos.x - mapBounds.min.x;
        float offsetY = pos.y - mapBounds.min.y;

        // 2. Convert that offset into grid indices by dividing by tile dimensions
        int x = Mathf.FloorToInt(offsetX / tileWidth);
        int y = Mathf.FloorToInt(offsetY / tileHeight);

        // 3. Check if the calculated indices are within the array bounds
        if (x >= 0 && x < cols && y >= 0 && y < rows)
        {
            return fogTiles[x, y];
        }

        // Target is outside the fog grid
        return null;
    }
    public List<Vector3> GetRevealedPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (var tile in revealedTiles)
        {
            positions.Add(tile.position);
        }
        return positions;
    }
}

[System.Serializable]
public class FogTile
{
    public GameObject obj;
    public Vector3 position;
    public bool revealed;
    public int gridX; // Added to track coordinates
    public int gridY;
}