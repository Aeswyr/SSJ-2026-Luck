using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicShader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Material mat;
    private int index = -1;

    public void ShowShader() {
        HideShader();

        List<Material> mats = new();

        sprite.GetSharedMaterials(mats);
        mats.Add(mat);
        sprite.materials = mats.ToArray();
        index = mats.Count - 1;
    }

    public void HideShader() { 
        if (index == -1)
            return;

        List<Material> mats = new();

        sprite.GetSharedMaterials(mats);
        mats.RemoveAt(index);
        sprite.materials = mats.ToArray();

        index = -1;
    }
}
