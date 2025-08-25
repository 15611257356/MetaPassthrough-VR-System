using UnityEngine;

public class PassthroughProjectionSurface : MonoBehaviour
{
    private MeshRenderer _mr;
    private Color _originalColor;
    public Color highlightColor = Color.yellow;
    private bool _isPreview = false; 

    void Start()
    {
        OVRPassthroughLayer layer = FindAnyObjectByType<OVRPassthroughLayer>();
        if (layer != null)
            layer.AddSurfaceGeometry(gameObject, true);

        _mr = GetComponent<MeshRenderer>();
        if (_mr != null && _mr.material != null)
        {
            _originalColor = _mr.material.color;

            
            Color c = _originalColor;
            c.a = 0.001f;
            _mr.material.color = c;
        }

        if (_isPreview)
            UnHighlight();
    }

    public void Highlight()
    {
        if (_mr != null && _mr.material != null)
        {
            Color c = highlightColor;
            c.a = 1f; 
            _mr.material.color = c;
        }
    }

    public void UnHighlight()
    {
        if (_mr != null && _mr.material != null)
            _mr.material.color = _originalColor;
    }

    
    public void SetPreview(bool preview)
    {
        _isPreview = preview;
        if (preview)
            UnHighlight();
    }
}
