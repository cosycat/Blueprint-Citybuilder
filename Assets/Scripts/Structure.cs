using System;
using UnityEngine;


public class Structure : MonoBehaviour
{
    private StructureType _type;

    public string Name => _type.name;
    public Sprite Sprite => _type.sprite;
    public int TotalHealth => _type.totalHealth;

    public int CurrentHealth { get; private set; }

    public void Setup(StructureType type)
    {
        if (_type != null)
        {
            throw new Exception("Structure Type already set!");
        }
        _type = type;
        CurrentHealth = TotalHealth;

        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite;
    }
    
}

