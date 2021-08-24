using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingController : MonoBehaviour
{
    #region Members

    #region public Members

    public static BuildingController Instance { get; private set; }

    public const bool ShouldSingleBuildModeStayWhileDragging = true;
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

    /// <summary>
    /// True if a Structure is currently selected (i.e. the player is in building mode). False otherwise.
    /// </summary>
    public bool IsBuilding => _selectedStructureType != null;

    #endregion

    #region private Members

    private StructureType _selectedStructureType = null;

    [SerializeField] private int _selectedStructureTypeID =
        0; // Only for debugging, to switch between the different structure types easily, before we have a GUI to build with. 

    private Vector2Int _currentDraggingStartPosition;
    private bool _isDragging;

    /// <summary>
    /// The Area which is used to generate the preview(s) from and to place new buildings.
    /// </summary>
    private List<Vector2Int> _draggingBuildingArea = new List<Vector2Int>();

    private SpriteRenderer _previewSpriteRendererPrefab;
    private List<SpriteRenderer> _previewSpriteObjects = new List<SpriteRenderer>();

    #endregion

    #endregion


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
                    Debug.Log("Did not build something, because building/dragging was canceled!");
                    //Debug.LogWarning("OnBuild should probably not be called from a MouseUp event (canceled) if it wasn't dragging in the first place. This could maybe have happened when the click event started outside of BuildingControllers responsibility, but then ended somehow inside BuildingControllers responsibility (like it started on a GUI Element, but ended somewhere on the world)");
                    return;
                }

                Build();

                ResetDraggingAndPreviews();

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
        if (!IsDragging && !IsBuilding)
        {
            Debug.LogWarning("Should not call this code if we are not in Build Mode!");
            return;
        }

        UpdateDraggingArea(newTileCoord, oldTileCoord);
        UpdatePreviews();
    }

    /// <summary>
    /// Select the next structure in the list of all structures.
    /// Only for Debugging/Testing while we have no GUI yet!
    /// </summary>
    /// <param name="context"></param>
    public void OnSelectNext(InputAction.CallbackContext context)
    {
        if (IsDragging) return;
        if (context.performed)
        {
            Debug.Log("Tab pressed");
            var nextStructureType = StructureManager.Instance.GetStructureTypeForID(++_selectedStructureTypeID);
            if (nextStructureType == null)
            {
                _selectedStructureTypeID = 0;
                nextStructureType = StructureManager.Instance.GetStructureTypeForID(_selectedStructureTypeID);
            }

            _selectedStructureType = nextStructureType;
            
            ResetDraggingAndPreviews();
        }
    }

    public void OnCancelBuild(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ResetDraggingAndPreviews();
        }
    }

    #endregion


    #region Building Methods

    // TODO use the new draggingArea to build the structures
    private void Build()
    {
        foreach (var buildingCoord in _draggingBuildingArea)
        {
            PlaceSingleStructure(buildingCoord);
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

    private void UpdatePreviews()
    {
        var previewSprite = GetPreviewSpriteFromStructureType(_selectedStructureType);
        RemovePreviews(); // If we optimise UpdateDraggingArea to not completely recalculate everything, but only change some tiles, we should also update this, to only remove/add the needed previews.
        foreach (var tileCoord in _draggingBuildingArea)
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

    /// <summary>
    /// Calls update Dragging Area with the current Mouse Position as default arguments.
    /// </summary>
    private void UpdateDraggingArea()
    {
        var currentMousePosition = InputManager.GetCurrentTileUnderMouse();
        UpdateDraggingArea(currentMousePosition, currentMousePosition);
    }

    private void UpdateDraggingArea(Vector2Int currMouseTileCoord, Vector2Int lastMouseTileCoord)
    {
        // lastMouseTileCoord can later be used to optimise this to only update some tiles instead of recalculate everything.
        if (_selectedStructureType.buildMode == BuildMode.Single ||
            currMouseTileCoord == _currentDraggingStartPosition || !IsDragging)
        {
            _draggingBuildingArea = new List<Vector2Int>(new[]
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                (IsDragging && ShouldSingleBuildModeStayWhileDragging)
                    ? _currentDraggingStartPosition
                    : currMouseTileCoord
            });
            return;
        }

        _draggingBuildingArea = new List<Vector2Int>();
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
                _draggingBuildingArea.Add(new Vector2Int(x, y));
            }
        }
    }

    /// <summary>
    /// Sets IsDragging to false and updates the dragging/building area and the previews for the current mouse position.
    /// </summary>
    private void ResetDraggingAndPreviews()
    {
        IsDragging = false;
        UpdateDraggingArea();
        UpdatePreviews();
    }

    #endregion
}