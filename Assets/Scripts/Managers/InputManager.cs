using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Vector2Int _lastMousePositionInTileCoordinates;

    public void OnMouseMovement(InputAction.CallbackContext context)
    {
        var currentTileUnderMouse = GetCurrentTileUnderMouse();
        if (_lastMousePositionInTileCoordinates.Equals(currentTileUnderMouse)) { }
        else
        {
            // This should one day be centralised, to have one place to check for what the Mouse is currently doing
            // But for now, it's ok to just ask the different things that could currently use the mouse.
            if (BuildingController.Instance.IsBuilding)
            {
                BuildingController.Instance.OnMouseMoveOverTile(_lastMousePositionInTileCoordinates, currentTileUnderMouse);
            }

            _lastMousePositionInTileCoordinates = currentTileUnderMouse;
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (BuildingController.Instance.IsBuilding)
        {
            BuildingController.Instance.OnCancelBuild(context);
        }
    }
    
    

    public static Vector2Int GetCurrentTileUnderMouse()
    {
        Vector2 mouseScreenCoordinates = Mouse.current.position.ReadValue();
        if (Camera.main == null)
            throw new Exception("No main Camera!");
        Vector2 mouseWorldCoordinates = Camera.main.ScreenToWorldPoint(mouseScreenCoordinates);
        var tileCoordinates =
            WorldManager.Instance.GetTileCoordinatesForRealWorldCoordinates(mouseWorldCoordinates);
        return tileCoordinates;
    }
}