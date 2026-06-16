using UnityEngine;
using UnityEditor;
using System.IO;

public class GameSetupTool : EditorWindow
{
    [MenuItem("Blocked/Sihirli Düzeltme (Auto Setup)")]
    public static void AutoSetup()
    {
        // 1. Klasörleri kontrol et
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // 2. Kamera Ayarı (Portrait modda sığması için Orthographic Size büyütülür)
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 9f; // 5x5 gridin ekrana sığması için
            cam.backgroundColor = new Color(0.9f, 0.85f, 0.75f); // Tatlı bir bej
        }

        // 3. GridManager ayarları
        GridManager gridManager = Object.FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            gridManager.cellSize = 1.5f;
            gridManager.gridOrigin = new Vector2(-3.75f, -3.75f); // 5x5 için tam merkezleme
            EditorUtility.SetDirty(gridManager);
        }

        // 4. Doğru Prefabları Kodla Yarat
        GameObject freeBlock = CreateBlockPrefab("FreeBlock_Auto", new Vector2(1.5f, 1.5f), Color.white);
        GameObject horizBlock = CreateBlockPrefab("Horizontal_Auto", new Vector2(3f, 1.5f), new Color(0.8f, 0.6f, 0.4f));
        GameObject vertBlock = CreateBlockPrefab("Vertical_Auto", new Vector2(1.5f, 3f), new Color(0.8f, 0.6f, 0.4f));
        GameObject targetBlock = CreateBlockPrefab("Target_Auto", new Vector2(3f, 1.5f), Color.red);

        // 5. LevelManager'a Ata
        LevelManager levelManager = Object.FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.freeBlockPrefab = freeBlock.GetComponent<BlockController>();
            levelManager.horizontalBlockPrefab = horizBlock.GetComponent<BlockController>();
            levelManager.verticalBlockPrefab = vertBlock.GetComponent<BlockController>();
            levelManager.targetBlockPrefab = targetBlock.GetComponent<BlockController>();
            
            EditorUtility.SetDirty(levelManager);
        }

        Debug.Log("Sihirli Düzeltme Tamamlandı! Lütfen Play'e basıp test edin.");
    }

    private static GameObject CreateBlockPrefab(string name, Vector2 scale, Color color)
    {
        string path = $"Assets/Prefabs/{name}.prefab";
        
        GameObject go = new GameObject(name);
        
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        // Standart beyaz sprite bul
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        sr.color = color;
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = scale; // 9-slice benzeri boyutlandırma için

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = scale;

        go.AddComponent<BlockController>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        
        return prefab;
    }
}
