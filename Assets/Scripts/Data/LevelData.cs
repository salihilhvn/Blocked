using System;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
    public int levelId;
    public int gridSize = 6;
    public int maxMoves = 15; 
    public float timeLimit = 60f; // Saniye cinsinden (0 veya altı = süresiz)
    public List<BlockData> blocks;
}

[Serializable]
public class BlockData
{
    public int id;
    public bool isHorizontal;
    public int length;
    public int x; // Grid X koordinatı (Sol alt)
    public int y; // Grid Y koordinatı (Sol alt)
    public bool isTarget;
    public bool isFree; // 1x1 her yöne gidebilen blok
}
