using UnityEngine;

namespace Metelab
{
    [CreateAssetMenu(fileName = "New LogData", menuName = "Metelab/LogData")]
    public class MeteLogData : ScriptableObject
    {
        public Color Color;
        public bool IsDisabled;
        public string Filter;
    }
}