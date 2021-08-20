using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingController : MonoBehaviour
{
    public static BuildingController Instance { get; private set; }

    [SerializeField] private string selectedStructureType = null;

    private Vector2Int _lastBuildingStartPosition;
    public bool IsDragging { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnBuild(InputAction.CallbackContext context)
    {
        if (selectedStructureType == null)
        {
            Debug.LogWarning("Should not reach OnBuild if no structure to build is selected!");
            return;
        }

        StructureType currentStructureType = StructureManager.Instance.GetStructureTypeForName(selectedStructureType);
        if (currentStructureType == null)
        {
            Debug.LogWarning("Somehow a structure was selected, which does not exist");
            return;
        }

        Vector2 mouseScreenCoordinates = Mouse.current.position.ReadValueFromPreviousFrame();
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                _lastBuildingStartPosition = GetTileCoordinatesFromMouseCoordinates(mouseScreenCoordinates);
                IsDragging = true;
                break;
            case InputActionPhase.Canceled:
                if (IsDragging == false)
                {
                    Debug.LogWarning("OnBuild should probably not be called from a MouseUp event (canceled) if it wasn't dragging in the first place. This could maybe have happened when the click event started outside of BuildingControllers responsibility, but then ended somehow inside BuildingControllers responsibility (like it started on a GUI Element, but ended somewhere on the world)");
                    return;
                }
                var buildMode = currentStructureType.buildMode;
                switch (buildMode)
                {
                    case BuildMode.Single:
                        PlaceSingleStructure(_lastBuildingStartPosition);
                        break;
                    case BuildMode.Row:
                        PlaceRow(_lastBuildingStartPosition,
                            GetTileCoordinatesFromMouseCoordinates(mouseScreenCoordinates));
                        break;
                    case BuildMode.Area:
                        PlaceArea(_lastBuildingStartPosition, GetTileCoordinatesFromMouseCoordinates(mouseScreenCoordinates));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                IsDragging = false;
                break;
        }
        
    }

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
            tileToBuildOn.BuildStructureOnTile(selectedStructureType);
        }
        else
        {
            Debug.LogWarning("Tile already has a structure on it!");
        }
    }


    private static Vector2Int GetTileCoordinatesFromMouseCoordinates(Vector2 mouseScreenCoordinates)
    {
        if (Camera.main == null)
            throw new Exception("No main Camera!");
        Vector2 mouseWorldCoordinates = Camera.main.ScreenToWorldPoint(mouseScreenCoordinates);
        var tileCoordinates =
            WorldManager.Instance.GetTileCoordinatesForRealWorldCoordinates(mouseWorldCoordinates);
        return tileCoordinates;
    }
}