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

    public void SetTargetClosestToCurrentPosition()
    {
        if (_typesToTarget.Length == 1)
            CurrentTarget = _quadtree?.ClosestTargetOfTypeToPosition(transform.position.x, transform.position.z, _typesToTarget[0], _searchRadius, _myTarget);
        else
            CurrentTarget = _quadtree?.ClosestTargetOfTypesToPosition(transform.position.x, transform.position.z, _typesToTarget, _searchRadius, _myTarget);
    }

    public void SetTargetClosestToPoint(float x, float z)
    {
        if (_typesToTarget.Length == 1)
            CurrentTarget = _quadtree?.ClosestTargetOfTypeToPosition(x, z, _typesToTarget[0], _searchRadius, _myTarget);
        else
            CurrentTarget = _quadtree?.ClosestTargetOfTypesToPosition(x, z, _typesToTarget, _searchRadius, _myTarget);
    }

    public void SetNewTypesToTarget(TargetTypes[] types) => _typesToTarget = types;

    public void ChangeSearchRadius(float radius) => _searchRadius = radius;

    public void ClearTarget() => CurrentTarget = null;

    private void Start()
    {
        if (GetComponent<QTTSTarget>())
            _myTarget = GetComponent<QTTSTarget>();
    }
}


