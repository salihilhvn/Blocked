using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Prefabs")]
    public BlockController horizontalBlockPrefab;
    public BlockController verticalBlockPrefab;
    public BlockController targetBlockPrefab;

    [Header("Settings")]
    public Transform boardContainer;

    private List<BlockController> currentBlocks = new List<BlockController>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadLevel(TextAsset levelJson)
    {
        ClearLevel();

        LevelData data = JsonUtility.FromJson<LevelData>(levelJson.text);
        if (data == null)
        {
            Debug.LogError("Failed to parse level JSON");
            return;
        }

        foreach (var b in data.blocks)
        {
            BlockController prefab = b.isTarget ? targetBlockPrefab : (b.isHorizontal ? horizontalBlockPrefab : verticalBlockPrefab);
            
            BlockController newBlock = Instantiate(prefab, boardContainer);
            newBlock.Initialize(b.id, b.length, b.isHorizontal, b.isTarget, new Vector2Int(b.x, b.y));
            currentBlocks.Add(newBlock);
        }

        GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }

    public void ClearLevel()
    {
        foreach (var b in currentBlocks)
        {
            if (b != null) Destroy(b.gameObject);
        }
        currentBlocks.Clear();
        
        if (GridManager.Instance != null)
        {
            GridManager.Instance.ResetGrid();
        }
    }
}
