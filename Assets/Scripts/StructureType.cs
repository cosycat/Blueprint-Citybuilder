using System;
using UnityEngine;

[Serializable]
public class StructureType
{
    // TODO make the fields readonly once they are read in from a file.
    // TODO once we read them in from a file, we can let them inherit from each other.
    [SerializeField] public string name;
    [SerializeField] public Sprite sprite;
    [SerializeField] public int totalHealth = 100;
    [SerializeField] public BuildMode buildMode = BuildMode.Single;
    [SerializeField] public int maxInhabitants = 0;

}

public enum BuildMode
{
    Single,
    Row,
    Area
}