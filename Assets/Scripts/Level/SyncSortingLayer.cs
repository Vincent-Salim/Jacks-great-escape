using UnityEngine;

public class SyncSortingLayer : MonoBehaviour
{
    [SerializeField] private bool syncOnStart = true;
    [SerializeField] private int childOrderOffset = 0;
    
    void Start()
    {
        if (syncOnStart)
            SyncChildrenSortingLayers();
    }
    
    public void SyncChildrenSortingLayers()
    {
        SpriteRenderer parentRenderer = GetComponent<SpriteRenderer>();
        if (parentRenderer == null) return;
        
        SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();
        
        foreach (var renderer in childRenderers)
        {
            if (renderer != parentRenderer)
            {
                renderer.sortingLayerName = parentRenderer.sortingLayerName;
                renderer.sortingOrder = parentRenderer.sortingOrder + childOrderOffset;
            }
        }
    }
}