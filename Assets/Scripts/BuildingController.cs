using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingController : MonoBehaviour
{
    #region public Members

    public static BuildingController Instance { get; private set; }

    public const bool ShouldSingleBuildModeStayWhileDragging = false;
    public static readonly Color PreviewSpriteColorOverlay = new Color(0, 0, 1, 0.2f);

    public bool IsDragging
    {
        get => _isDragging;
        private set
        {
            _isDragging = value;
            if (!_isDragging)
            {
                RemovePreviews();
            }
        }
    }

    #endregion


    #region private Members

    private StructureType _selectedStructureType = null;

    private int
        _selectedStructureTypeID =
            0; // Only for debugging, to switch between the different structure types easily, before we have a GUI to build with. 

    private Vector2Int _currentDraggingStartPosition;
    private bool _isDragging;
    private List<Vector2Int> _draggingArea = new List<Vector2Int>();

    private SpriteRenderer _previewSpriteRendererPrefab;

    private List<SpriteRenderer> _previewSpriteObjects = new List<SpriteRenderer>();

    #endregion


    /// <summary>
    /// True if a Structure is currently selected (i.e. the player is in building mode). False otherwise.
    /// </summary>
    public bool IsBuilding => _selectedStructureType != null;


    #region Unity Life Cycle Methods

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Something weird happened? BuildingController was already set?!");
        }

        Instance = this;

        var go = new GameObject
        {
            name = "BuildingPreview",
            layer = 0,
            transform =
            {
                parent = this.transform
            }
        };
        _previewSpriteRendererPrefab = go.AddComponent<SpriteRenderer>();
        _previewSpriteRendererPrefab.color = PreviewSpriteColorOverlay;
        _previewSpriteRendererPrefab.sortingLayerName = "Structures";
    }

    private void Start()
    {
        _selectedStructureType = StructureManager.Instance.GetStructureTypeForID(_selectedStructureTypeID);
    }

    #endregion


    #region Unity Input Event Methods

    public void OnBuild(InputAction.CallbackContext context)
    {
        if (_selectedStructureType == null)
        {
            Debug.LogWarning("Should not reach OnBuild if no structure to build is selected!");
            return;
        }

        if (_selectedStructureType == null)
        {
            Debug.LogWarning("Somehow a structure was selected, which does not exist");
            return;
        }

        var currentMouseTilePosition = InputManager.GetCurrentTileUnderMouse();
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                _currentDraggingStartPosition = currentMouseTilePosition;
                IsDragging = true;
                break;
            case InputActionPhase.Canceled:
                if (IsDragging == false)
                {
                    Debug.LogWarning(
                        "OnBuild should probably not be called from a MouseUp event (canceled) if it wasn't dragging in the first place. This could maybe have happened when the click event started outside of BuildingControllers responsibility, but then ended somehow inside BuildingControllers responsibility (like it started on a GUI Element, but ended somewhere on the world)");
                    return;
                }

                var buildMode = _selectedStructureType.buildMode;
                switch (buildMode)
                {
                    case BuildMode.Single:
                        PlaceSingleStructure(_currentDraggingStartPosition);
                        break;
                    case BuildMode.Row:
                        PlaceRow(_currentDraggingStartPosition,
                            currentMouseTilePosition);
                        break;
                    case BuildMode.Area:
                        PlaceArea(_currentDraggingStartPosition,
                            currentMouseTilePosition);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                IsDragging = false;
                CreateSinglePreview(currentMouseTilePosition,
                    GetPreviewSpriteFromStructureType(_selectedStructureType));
                break;
        }
    }


    /// <summary>
    /// Mainly sed to updated the preview object
    /// </summary>
    /// <param name="oldTileCoord"></param>
    /// <param name="newTileCoord"></param>
    public void OnMouseMoveOverTile(Vector2Int oldTileCoord, Vector2Int newTileCoord)
    {
        var previewSprite = GetPreviewSpriteFromStructureType(_selectedStructureType);
        UpdateDraggingArea(newTileCoord, oldTileCoord);
        if (IsDragging)
        {
            CreateDraggingPreview(previewSprite);
        }
        else if (IsBuilding)
        {
            CreateBuildingPreview(newTileCoord, previewSprite);
        }
        else
        {
            Debug.LogWarning("Should not call this code if we are not in Build Mode!");
        }
        // TODO update building preview
    }

    /// <summary>
    /// Select the next structure in the list of all structures.
    /// Only for Debugging/Testing while we have no GUI yet!
    /// </summary>
    /// <param name="context"></param>
    public void OnSelectNext(InputAction.CallbackContext context)
    {
        var nextStructureType = StructureManager.Instance.GetStructureTypeForID(++_selectedStructureTypeID);
        if (nextStructureType == null)
        {
            _selectedStructureTypeID = 0;
            nextStructureType = StructureManager.Instance.GetStructureTypeForID(_selectedStructureTypeID);
        }

        _selectedStructureType = nextStructureType;
    }

    #endregion


    #region Building Methods

    // TODO use the new draggingArea to build the structures
    private void PlaceRow(Vector2Int rowStart, Vector2Int rowEnd)
    {
        int startX, startY, endX, endY;
        if (Mathf.Abs(rowStart.x - rowEnd.x) >= Mathf.Abs(rowStart.y - rowEnd.y))
        {
            startX = Mathf.Max(rowStart.x, rowEnd.x);
            endX = Mathf.Min(rowStart.x, rowEnd.x);
            startY = endY = rowStart.y;
        }
        else
        {
            startY = Mathf.Max(rowStart.y, rowEnd.y);
            endY = Mathf.Min(rowStart.y, rowEnd.y);
            startX = endX = rowStart.x;
        }

        PlaceArea(startX, startY, endX, endY);
    }

    private void PlaceArea(Vector2Int start, Vector2Int end)
    {
        PlaceArea(start.x, start.y, end.x, end.y);
    }

    private void PlaceArea(int startX, int startY, int endX, int endY)
    {
        Debug.Log($"Placing an area from ({startX}, {startY}) to ({endX}, {endY})");
        Debug.Log($"_currentDraggingStartPosition: {_currentDraggingStartPosition}");
        if (startX > endX) (endX, startX) = (startX, endX);
        if (startY > endY) (startY, endY) = (endY, startY);
        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                PlaceSingleStructure(new Vector2Int(x, y));
            }
        }
    }

    private void PlaceSingleStructure(Vector2Int coord)
    {
        var tileToBuildOn = WorldManager.Instance.GetTileAt(coord);
        if (tileToBuildOn == null)
        {
            Debug.Log("Placed a Tile outside of the world. Ignored.");
            return;
        }

        if (!tileToBuildOn.HasStructure)
        {
            tileToBuildOn.BuildStructureOnTile(_selectedStructureType);
        }
        else
        {
            Debug.LogWarning("Tile already has a structure on it!");
        }
    }

    #endregion

    #region Building Preview

    private Sprite GetPreviewSpriteFromStructureType(StructureType type)
    {
        return
            type.sprite; // Maybe one day each sprite will have it's separate preview sprite, but for now, we use the original, but adjusted to look like a preview.
    }

    private void CreateBuildingPreview(Vector2Int newTileCoord, Sprite previewSprite)
    {
        RemovePreviews();
        CreateSinglePreview(newTileCoord, previewSprite);
    }

    private void CreateDraggingPreview(Sprite previewSprite)
    {
        RemovePreviews(); // If we optimise UpdateDraggingArea to not completely recalculate everything, but only change some tiles, we should also update this, to only remove/add the needed previews.
        foreach (var tileCoord in _draggingArea)
        {
            CreateSinglePreview(tileCoord, previewSprite);
        }
    }

    private void CreateSinglePreview(Vector2Int previewCoord, Sprite previewSprite)
    {
        var worldManager = WorldManager.Instance;
        if (worldManager.GetTileAt(previewCoord)?.HasStructure ?? true) return;

        var pos = worldManager.GetWorldCoordinatesForTileCoordinates(previewCoord);
        var spriteRenderer = Instantiate(_previewSpriteRendererPrefab, pos, Quaternion.identity);
        spriteRenderer.transform.parent = this.transform;
        spriteRenderer.sprite = previewSprite;
        _previewSpriteObjects.Add(spriteRenderer);
    }

    private void RemovePreviews()
    {
        foreach (var previewSpriteObject in _previewSpriteObjects)
        {
            Destroy(previewSpriteObject.gameObject);
        }

        _previewSpriteObjects = new List<SpriteRenderer>();
    }

    #endregion

    #region Helper Methods

    private void UpdateDraggingArea(Vector2Int currMouseTileCoord, Vector2Int lastMouseTileCoord)
    {
        // lastMouseTileCoord can later be used to optimise this to only update some tiles instead of recalculate everything.
        if (_selectedStructureType.buildMode == BuildMode.Single || currMouseTileCoord == _currentDraggingStartPosition)
        {
            _draggingArea = new List<Vector2Int>(new[]
            {
                ShouldSingleBuildModeStayWhileDragging ? _currentDraggingStartPosition : currMouseTileCoord
            });
            return;
        }

        _draggingArea = new List<Vector2Int>();
        int startX = _currentDraggingStartPosition.x, startY = _currentDraggingStartPosition.y;
        int endX = currMouseTileCoord.x, endY = currMouseTileCoord.y;

        if (_selectedStructureType.buildMode == BuildMode.Row)
        {
            if (Mathf.Abs(startX - endX) >= Mathf.Abs(startY - endY))
            { // We build a horizontal row
                endY = startY;
            }
            else
            { // We build a vertical row
                endX = startX;
            }
        }

        if (startX > endX) (endX, startX) = (startX, endX);
        if (startY > endY) (startY, endY) = (endY, startY);
        // Now start <= end

        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                _draggingArea.Add(new Vector2Int(x, y));
            }
        }
    }

    #endregion
}