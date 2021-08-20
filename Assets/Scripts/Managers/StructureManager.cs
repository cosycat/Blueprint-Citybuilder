using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Managers
{
    public class StructureManager : MonoBehaviour
    {
        public static StructureManager Instance { get; private set; }

        // TODO load them from a file
        [SerializeField] private List<StructureType> structures;

        private void Awake()
        {
            Debug.Log("StructureManager awoken");
            if (Instance == null)
            {
                Debug.Log("StructureManager set");
                Instance = this;
            }
        }

        [CanBeNull]
        public StructureType GetStructureTypeForName(string typeName)
        {
            return structures.FirstOrDefault(structureType => structureType.name == typeName);
        }

        public Structure CreateNewStructureForName(string typeName)
        {
            var structureType = GetStructureTypeForName(typeName);
            if (structureType == null) throw new ArgumentException($"No StructureType with name {typeName} found!");
            var go = new GameObject();
            go.transform.parent = this.transform;
            go.name = structureType.name;
            var structure = go.AddComponent<Structure>();
            structure.Setup(structureType);
            return structure;
        }
    }
}