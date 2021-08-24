using System;
using System.Collections.Generic;
using UnityEngine;


public class Structure : MonoBehaviour
{
    #region Type Infos

    private StructureType _type;

    public string Name => _type.name;
    public Sprite Sprite => _type.sprite;
    public int TotalHealth => _type.totalHealth;
    public int MaxInhabitants => _type.maxInhabitants;
    public bool IsHabitable => _type.maxInhabitants > 0;

    #endregion


    #region Instance Infos

    public int CurrentHealth { get; private set; }

    #endregion

    
    #region Housing

    private readonly List<Inhabitant> _inhabitants = new List<Inhabitant>();

    public int CurrentInhabitantCount => _inhabitants.Count;
    public int FreeHabitableSpace => MaxInhabitants - CurrentInhabitantCount;
    
    public void AddInhabitant(Inhabitant newInhabitant)
    {
        if (CurrentInhabitantCount >= MaxInhabitants)
            throw new ArgumentOutOfRangeException(
                $"A house can't have more inhabitants than it's MaxInhabitants value! newInhabitant: {newInhabitant}, maxInhabitants: {MaxInhabitants}, previous amount of habitants: {CurrentInhabitantCount}");
        _inhabitants.Add(newInhabitant);
    }

    #endregion

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