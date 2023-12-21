using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Metelab.PeakGameCase
{
    [CustomEditor(typeof(GridSO))]
    public class EditorGridSO : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GridSO myScript = (GridSO)target;
            if (GUILayout.Button(nameof(myScript.LoadFromTexture)))
            {
                myScript.LoadFromTexture();
            }
        }
    }
}

