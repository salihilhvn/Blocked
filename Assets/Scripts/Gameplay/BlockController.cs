using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class BlockController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public int Id { get; private set; }
    public int Length { get; private set; }
    public bool IsHorizontal { get; private set; }
    public bool IsTarget { get; private set; }
    public bool IsFree { get; private set; }
    public Vector2Int GridPosition { get; private set; }

    private Vector3 offset;
    private Camera mainCamera;
    private bool isDragging = false;
    
    private enum DragAxis { None, Horizontal, Vertical }
    private DragAxis currentDragAxis = DragAxis.None;
    private Vector3 dragStartMousePos;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Initialize(int id, int length, bool isHorizontal, bool isTarget, bool isFree, Vector2Int startPos)
    {
        Id = id;
        Length = length;
        IsHorizontal = isHorizontal;
        IsTarget = isTarget;
        IsFree = isFree;
        GridPosition = startPos;

        mainCamera = Camera.main;

        UpdateWorldPosition();
        RegisterToGrid();
    }

    private void UpdateWorldPosition()
    {
        transform.position = GridManager.Instance.GridToWorld(GridPosition.x, GridPosition.y, Length, IsHorizontal);
    }

    private void RegisterToGrid()
    {
        GridManager.Instance.RegisterBlock(GridPosition.x, GridPosition.y, Length, IsHorizontal, this);
    }

    private void UnregisterFromGrid()
    {
        GridManager.Instance.UnregisterBlock(GridPosition.x, GridPosition.y, Length, IsHorizontal);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        offset = transform.position - GetMouseWorldPos(eventData.position);
        dragStartMousePos = GetMouseWorldPos(eventData.position);
        currentDragAxis = DragAxis.None;
        isDragging = true;
        UnregisterFromGrid();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        Vector3 currentMousePos = GetMouseWorldPos(eventData.position);
        Vector3 targetPos = currentMousePos + offset;

        bool moveHorizontal = IsHorizontal;

        if (IsFree)
        {
            if (currentDragAxis == DragAxis.None)
            {
                Vector3 delta = currentMousePos - dragStartMousePos;
                if (delta.magnitude > 0.1f) // Eşik değeri
                {
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                        currentDragAxis = DragAxis.Horizontal;
                    else
                        currentDragAxis = DragAxis.Vertical;
                }
                else
                {
                    return;
                }
            }
            moveHorizontal = (currentDragAxis == DragAxis.Horizontal);
        }

        if (moveHorizontal)
        {
            float targetX = targetPos.x;
            float minX = GetMinXWorld();
            float maxX = GetMaxXWorld();
            
            targetX = Mathf.Clamp(targetX, minX, maxX);
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        }
        else
        {
            float targetY = targetPos.y;
            float minY = GetMinYWorld();
            float maxY = GetMaxYWorld();
            
            targetY = Mathf.Clamp(targetY, minY, maxY);
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        // Snap to grid
        Vector2Int newGridPos = CalculateGridPositionFromWorld();
        GridPosition = newGridPos;
        UpdateWorldPosition();
        RegisterToGrid();

        if (IsTarget)
        {
            GameManager.Instance.CheckLevelComplete(this);
        }
    }

    private Vector3 GetMouseWorldPos(Vector2 screenPos)
    {
        Vector3 mousePoint = screenPos;
        mousePoint.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private float GetMinXWorld()
    {
        int emptyLeft = 0;
        for (int i = GridPosition.x - 1; i >= 0; i--) {
            if (GridManager.Instance.IsCellEmptyOrSameBlock(i, GridPosition.y, this)) emptyLeft++;
            else break;
        }
        return GridManager.Instance.GridToWorld(GridPosition.x - emptyLeft, GridPosition.y, Length, IsHorizontal).x;
    }

    private float GetMaxXWorld()
    {
        int emptyRight = 0;
        for (int i = GridPosition.x + Length; i < GridManager.Width; i++) {
            if (GridManager.Instance.IsCellEmptyOrSameBlock(i, GridPosition.y, this)) emptyRight++;
            else break;
        }
        
        // Hedef blok (Target) kapıdan dışarı çıkabilir (Sağdan)
        if (IsTarget && GridPosition.y == 2) { 
            emptyRight += 2; // Çıkış boşluğunu telafi et
        }

        return GridManager.Instance.GridToWorld(GridPosition.x + emptyRight, GridPosition.y, Length, IsHorizontal).x;
    }

    private float GetMinYWorld()
    {
        int emptyDown = 0;
        for (int i = GridPosition.y - 1; i >= 0; i--) {
            if (GridManager.Instance.IsCellEmptyOrSameBlock(GridPosition.x, i, this)) emptyDown++;
            else break;
        }
        return GridManager.Instance.GridToWorld(GridPosition.x, GridPosition.y - emptyDown, Length, IsHorizontal).y;
    }

    private float GetMaxYWorld()
    {
        int emptyUp = 0;
        for (int i = GridPosition.y + Length; i < GridManager.Height; i++) {
            if (GridManager.Instance.IsCellEmptyOrSameBlock(GridPosition.x, i, this)) emptyUp++;
            else break;
        }
        return GridManager.Instance.GridToWorld(GridPosition.x, GridPosition.y + emptyUp, Length, IsHorizontal).y;
    }

    private Vector2Int CalculateGridPositionFromWorld()
    {
        float cellSize = GridManager.Instance.cellSize;
        Vector2 origin = GridManager.Instance.gridOrigin;

        float leftEdgeWorld = IsHorizontal ? transform.position.x - (Length * cellSize) / 2f : transform.position.x - cellSize / 2f;
        float bottomEdgeWorld = !IsHorizontal ? transform.position.y - (Length * cellSize) / 2f : transform.position.y - cellSize / 2f;

        int x = Mathf.RoundToInt((leftEdgeWorld - origin.x) / cellSize);
        int y = Mathf.RoundToInt((bottomEdgeWorld - origin.y) / cellSize);

        if (IsTarget && x >= GridManager.Width - Length) {
            // Dışarı çıkmışsa grid içine zorlama
            return new Vector2Int(x, y);
        }

        x = Mathf.Clamp(x, 0, GridManager.Width - (IsHorizontal ? Length : 1));
        y = Mathf.Clamp(y, 0, GridManager.Height - (!IsHorizontal ? Length : 1));

        return new Vector2Int(x, y);
    }
}
