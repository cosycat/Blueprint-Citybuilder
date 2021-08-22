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
            if (Instance != null)
            {
                Debug.LogWarning("Something weird happened? StructureManager was already set?!");
            }

            Instance = this;
        }

        [CanBeNull]
        public StructureType GetStructureTypeForName(string typeName)
        {
            return structures.FirstOrDefault(structureType => structureType.name == typeName);
        }

        public Structure CreateNewStructureForName([NotNull] string typeName)
        {
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));
            var structureType = GetStructureTypeForName(typeName);
            if (structureType == null) throw new ArgumentException($"No StructureType with name {typeName} found!");
            return CreateNewStructureForStructureType(structureType);
        }

        public Structure CreateNewStructureForStructureType([NotNull] StructureType structureType)
        {
            if (structureType == null) throw new ArgumentNullException(nameof(structureType));
            var go = new GameObject();
            go.transform.parent = this.transform;
            go.name = structureType.name;
            var structure = go.AddComponent<Structure>();
            structure.Setup(structureType);
            return structure;
        } 
        
    }
}