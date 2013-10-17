/**
 *  @ Matt1988
 *  This extension was built by @Matt1988
 */
using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;


public class PivotTool : Editor {

    [MenuItem("Window/ProBuilder/Actions/Set Pivot _%j")]
    static void init()
    {
        pb_Object[] pbObjects = GetSelection();
        if (pbObjects.Length > 0)
        {
            foreach (pb_Object pbo in pbObjects)
            {
                if (pbo.selected_indices.Count > 0)
                {
                    SetPivot(pbo, pbo.selected_indices.ToArray());
                }
                else
                {
                    SetPivot(pbo, pbo.uniqueIndices);
                }
            }
        }
    }
   
    static pb_Object[] GetSelection()
    {
        return pbUtil.GetComponents<pb_Object>(Selection.transforms);       
    }

    private static void SetPivot(pb_Object pbo, int[] testIndices)
    {
        Vector3 center = Vector3.zero;
        foreach (Vector3 vector in pbo.VerticesInWorldSpace(testIndices))
        {
            center += vector;
        }
        center /= testIndices.Length;
        Vector3 dir = (pbo.transform.position - center);

        pbo.transform.position = center;

        pbo.TranslateVertices(pbo.uniqueIndices, dir);
    }
}
