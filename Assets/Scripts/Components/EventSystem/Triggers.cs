using System;
using Unity.Entities;

namespace Components.EventSystem
{
    public enum Logic
    {
        And, // Default.
        Or, // If current trigger fails, check next to override.
        Not // Inverse comparison. Neither Or is inherent in this by combining And and Not.
    }

    [Serializable]
    public struct Triggers : IBufferElementData
    {
        public int Property; // To be replaced with an enum.
        public float Comparison;
        public Logic Logic;
    }
}