using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGO : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTileType(TileType type)
    {
        _spriteRenderer.color = type._color;
    }
}
