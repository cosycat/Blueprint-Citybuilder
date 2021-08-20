using System;
using UnityEngine;

[Serializable]
public class StructureType
{ 
    // TODO make the fields readonly once they are read in from a file.
    [SerializeField] public string name;
    [SerializeField] public Sprite sprite;
    [SerializeField] public int totalHealth = 100;
    [SerializeField] public BuildMode buildMode = BuildMode.Single;


}

public enum BuildMode
{
    Single,
    Row,
    Area
}