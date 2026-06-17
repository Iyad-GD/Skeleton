using System.Collections.Generic;
using UnityEngine;

/// Seamlessly scrolls a SpriteRenderer horizontally forever.
/// At runtime it spawns enough duplicate "segments" of the sprite to cover the
/// camera view, moves them all left, and recycles any segment that leaves the
/// view back to the right edge. Works for both parallax background layers and
/// a tiled ground strip.
[RequireComponent(typeof(SpriteRenderer))]
public class EndlessScroller : MonoBehaviour
{
    [Tooltip("World units per second to scroll. Positive moves content to the left.")]
    public float scrollSpeed = 10f;

    [Tooltip("How much wider than the camera view to keep covered (1 = exactly the view).")]
    public float coverWidthMultiplier = 1.25f;

    private readonly List<Transform> _segments = new List<Transform>();
    private float _segmentWidth;
    private float _spanWidth;
    private float _leftWrapX;
    private SpriteRenderer _source;

    void Start()
    {
        _source = GetComponent<SpriteRenderer>();
        if (_source == null || _source.sprite == null)
        {
            enabled = false;
            return;
        }

        _segmentWidth = _source.bounds.size.x;
        if (_segmentWidth <= 0.0001f)
        {
            enabled = false;
            return;
        }

        float viewWidth = GetViewWorldWidth();
        float needed = viewWidth * coverWidthMultiplier + _segmentWidth;
        int count = Mathf.Max(2, Mathf.CeilToInt(needed / _segmentWidth) + 1);

        float startX = transform.position.x;
        _segments.Add(transform);

        for (int i = 1; i < count; i++)
        {
            var go = new GameObject(gameObject.name + "_seg" + i);
            go.transform.SetParent(transform.parent, false);
            go.transform.localScale = transform.localScale;
            go.transform.rotation = transform.rotation;
            go.transform.position = new Vector3(startX + _segmentWidth * i,
                                                transform.position.y,
                                                transform.position.z);

            var sr = go.AddComponent<SpriteRenderer>();
            CopyRenderer(_source, sr);
            _segments.Add(go.transform);
        }

        _spanWidth = _segmentWidth * count;
        _leftWrapX = startX - _segmentWidth;
    }

    void Update()
    {
        if (_segments.Count == 0) return;

        float delta = scrollSpeed * Time.deltaTime;
        for (int i = 0; i < _segments.Count; i++)
        {
            Transform seg = _segments[i];
            Vector3 p = seg.position;
            p.x -= delta;
            if (p.x < _leftWrapX)
                p.x += _spanWidth;
            seg.position = p;
        }
    }

    private static void CopyRenderer(SpriteRenderer src, SpriteRenderer dst)
    {
        dst.sprite = src.sprite;
        dst.sharedMaterial = src.sharedMaterial;
        dst.color = src.color;
        dst.flipX = src.flipX;
        dst.flipY = src.flipY;
        dst.drawMode = src.drawMode;
        dst.size = src.size;
        dst.tileMode = src.tileMode;
        dst.sortingLayerID = src.sortingLayerID;
        dst.sortingOrder = src.sortingOrder;
        dst.maskInteraction = src.maskInteraction;
        dst.spriteSortPoint = src.spriteSortPoint;
    }

    private float GetViewWorldWidth()
    {
        Camera cam = Camera.main;
        if (cam == null) return _segmentWidth;

        if (cam.orthographic)
            return cam.orthographicSize * 2f * cam.aspect;

        float dist = Mathf.Abs(cam.transform.position.z - transform.position.z);
        float h = 2f * dist * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        return h * cam.aspect;
    }
}
