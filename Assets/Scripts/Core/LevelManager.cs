using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    
    public static event Action<int> OnLevelLoaded;

    public int currentLevelIndex = 1;

    [Header("Prefabs")]
    public BlockController horizontalBlockPrefab;
    public BlockController verticalBlockPrefab;
    public BlockController targetBlockPrefab;
    public BlockController freeBlockPrefab;

    [Header("Settings")]
    public Transform boardContainer;

    [Header("Testing")]
    public bool autoStartTestLevel = false;
    public TextAsset testLevelJson;

    private List<BlockController> currentBlocks = new List<BlockController>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (autoStartTestLevel)
        {
            // Önce test level var mı bak, yoksa Resources'dan Level 1'i yükle
            if (testLevelJson != null)
            {
                LoadLevelFromJson(testLevelJson.text);
            }
            else
            {
                LoadLevel(currentLevelIndex);
            }
        }
    }

    public void LoadLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        TextAsset levelFile = Resources.Load<TextAsset>("Levels/Level" + levelIndex);
        
        if (levelFile != null)
        {
            LoadLevelFromJson(levelFile.text);
        }
        else
        {
            Debug.LogWarning("Level file not found for level: " + levelIndex + ". Oyun bitti veya dosya eksik!");
            // Başa dönebilir
            currentLevelIndex = 1;
            LoadLevel(currentLevelIndex);
        }
    }

    public void LoadNextLevel()
    {
        LoadLevel(currentLevelIndex + 1);
    }

    public void ReloadCurrentLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    private void LoadLevelFromJson(string jsonText)
    {
        ClearLevel();

        LevelData data = JsonUtility.FromJson<LevelData>(jsonText);
        if (data == null)
        {
            Debug.LogError("Failed to parse level JSON");
            return;
        }

        // Grid boyutunu dinamik olarak LevelData'dan ayarla
        if (GridManager.Instance != null)
        {
            GridManager.Instance.InitializeGrid(data.gridSize, data.gridSize);
        }

        foreach (var b in data.blocks)
        {
            BlockController prefab;
            if (b.isFree) prefab = freeBlockPrefab;
            else if (b.isTarget) prefab = targetBlockPrefab;
            else prefab = b.isHorizontal ? horizontalBlockPrefab : verticalBlockPrefab;
            
            BlockController newBlock = Instantiate(prefab, boardContainer);
            newBlock.Initialize(b.id, b.length, b.isHorizontal, b.isTarget, b.isFree, new Vector2Int(b.x, b.y));

            // JSON'dan gelen length (uzunluk) değerine göre görseli ve fiziği dinamik büyüt
            SpriteRenderer sr = newBlock.GetComponent<SpriteRenderer>();
            BoxCollider2D col = newBlock.GetComponent<BoxCollider2D>();
            if (sr != null && col != null && GridManager.Instance != null)
            {
                float cellSize = GridManager.Instance.cellSize;
                if (b.isFree) {
                    sr.size = new Vector2(cellSize, cellSize);
                } else if (b.isHorizontal) {
                    sr.size = new Vector2(b.length * cellSize, cellSize);
                } else {
                    sr.size = new Vector2(cellSize, b.length * cellSize);
                }
                col.size = sr.size;
            }

            currentBlocks.Add(newBlock);
        }

        OnLevelLoaded?.Invoke(currentLevelIndex);
        GameManager.Instance.StartLevel(data.maxMoves, data.timeLimit);
    }

    public bool RemoveRandomBlock()
    {
        // Hedef blok (Kırmızı) olmayanları listele
        List<BlockController> removableBlocks = new List<BlockController>();
        foreach(var block in currentBlocks)
        {
            if (block != null && !block.IsTarget)
            {
                removableBlocks.Add(block);
            }
        }

        if (removableBlocks.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, removableBlocks.Count);
            BlockController blockToRemove = removableBlocks[randomIndex];
            
            // Grid'den boşalt
            GridManager.Instance.UnregisterBlock(blockToRemove.GridPosition.x, blockToRemove.GridPosition.y, blockToRemove.Length, blockToRemove.IsHorizontal);
            
            currentBlocks.Remove(blockToRemove);
            Destroy(blockToRemove.gameObject);
            Debug.Log("Random Block Removed!");
            return true;
        }
        
        Debug.LogWarning("No removable block found.");
        return false;
    }

    public void ClearLevel()
    {
        foreach (var b in currentBlocks)
        {
            if (b != null) Destroy(b.gameObject);
        }
        currentBlocks.Clear();
    }
}
