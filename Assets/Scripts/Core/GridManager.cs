using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public const int Width = 5;
    public const int Height = 5;

    // 2D matrisimiz blok referanslarını tutar.
    private BlockController[,] grid;

    [Header("Grid Settings")]
    public float cellSize = 1.5f; // Her bir kare hücrenin Unity birim cinsinden boyutu
    public Vector2 gridOrigin = new Vector2(-4.5f, -4.5f); // Sol alt köşe başlangıç noktası

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            grid = new BlockController[Width, Height];
        } else {
            Destroy(gameObject);
        }
    }

    public void ResetGrid()
    {
        grid = new BlockController[Width, Height];
    }

    public bool IsCellEmptyOrSameBlock(int x, int y, BlockController block)
    {
        // Izgara sınırlarının dışı her zaman "dolu" sayılır (duvar)
        if (x < 0 || x >= Width || y < 0 || y >= Height) return false;
        
        // Eğer hücre boşsa veya kontrol eden bloğun kendisi varsa izin verilir
        return grid[x, y] == null || grid[x, y] == block;
    }

    public void RegisterBlock(int x, int y, int length, bool isHorizontal, BlockController block)
    {
        for (int i = 0; i < length; i++)
        {
            if (isHorizontal)
            {
                if (x + i >= 0 && x + i < Width && y >= 0 && y < Height)
                    grid[x + i, y] = block;
            }
            else
            {
                if (x >= 0 && x < Width && y + i >= 0 && y + i < Height)
                    grid[x, y + i] = block;
            }
        }
    }

    public void UnregisterBlock(int x, int y, int length, bool isHorizontal)
    {
        for (int i = 0; i < length; i++)
        {
            if (isHorizontal)
            {
                if (x + i >= 0 && x + i < Width && y >= 0 && y < Height)
                    grid[x + i, y] = null;
            }
            else
            {
                if (x >= 0 && x < Width && y + i >= 0 && y + i < Height)
                    grid[x, y + i] = null;
            }
        }
    }

    public Vector2 GridToWorld(int x, int y, int length, bool isHorizontal)
    {
        // Bloğun dünya koordinatını hesaplarken, merkeze (pivot) göre hesaplıyoruz.
        // Pivot tam ortada olduğu varsayılır.
        float widthOffset = isHorizontal ? (length * cellSize) / 2f : cellSize / 2f;
        float heightOffset = !isHorizontal ? (length * cellSize) / 2f : cellSize / 2f;

        return new Vector2(
            gridOrigin.x + (x * cellSize) + widthOffset,
            gridOrigin.y + (y * cellSize) + heightOffset
        );
    }
}
