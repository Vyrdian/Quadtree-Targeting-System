using UnityEngine;

public class QTTSTargetFinder : MonoBehaviour
{
    public QTTSTarget CurrentTarget { get; private set; }

    [SerializeField]
    private TargetTypes[] _typesToTarget;

    [SerializeField][Tooltip("Keep it at 0 if you do not want a maximum search radius")]
    private float _searchRadius = 0;

    [SerializeField][Tooltip("If the target starts in scene, use this. Otherwise, inject the quadtree with SetQuadTree")]
    private QTTSQuadtree _quadtree;

    private QTTSTarget _myTarget;

    public void SetQuadTree(QTTSQuadtree quadtree) => _quadtree = quadtree;

    public void SetTargetClosestToCurrentPosition() => GetTarget(transform.position.x, _quadtree.CoordinatePair == CoordinatePairs.XY ? transform.position.y : transform.position.z);

    private void GetTarget(float x, float yz)
    {
        if (!_quadtree)
            return;
        CurrentTarget = _quadtree?.ClosestTargetOfTypesToPosition(x, yz, _typesToTarget, _searchRadius, _myTarget);
    }

    public void SetTargetClosestToPoint(Vector3 point) => GetTarget(point.x, _quadtree.CoordinatePair == CoordinatePairs.XY ? point.y : point.z);

    public void SetNewTypesToTarget(TargetTypes[] types) => _typesToTarget = types;

    public void ChangeSearchRadius(float radius) => _searchRadius = radius;

    public void ClearTarget() => CurrentTarget = null;

    private void Start()
    {
        if (GetComponent<QTTSTarget>())
            _myTarget = GetComponent<QTTSTarget>();
    }
}


