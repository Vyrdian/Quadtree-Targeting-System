using UnityEngine;

public class QTTSTarget : MonoBehaviour
{
    [SerializeField][Tooltip("If the target starts in scene, use this. Otherwise, inject the quadtree with SetQuadTree")]
    private QTTSQuadtree _quadtree;

    [SerializeField]
    private TargetTypes _targetType;

    public Transform Trans { get; private set; }
    public float PosX { get; private set; }
    public float PosYZ { get; private set; }
    private CoordinatePairs _coordinatePair;
    private QTTSQuadtreeNode _node;

    private void Start() => Trans = GetComponent<Transform>();

    public void SetQuadTree(QTTSQuadtree quadtree)
    {
        _quadtree = quadtree;
        quadtree.AddTarget(this);
    }

    public TargetTypes GetTargetType() => _targetType;

    #region Position Changing

    // Call this whenever this target moves
    public void OnPositionChanged()
    {
        if(_node != null && !StillInSameNode(Trans.position.x, _quadtree.CoordinatePair == CoordinatePairs.XY ? Trans.position.y : Trans.position.z))
            ChangeNode();
    }

    public void SetNewNode(QTTSQuadtreeNode node) => _node = node;

    private bool StillInSameNode(float x, float yz) =>_node.MinX <= x && _node.MaxX >= x && _node.MinYZ <= yz && _node.MaxYZ >= yz ? true : false;

    private void ChangeNode()
    {
        _quadtree.AddTarget(this);
        _quadtree.RemoveTarget(_node, this, false);
    }
    #endregion

    private void OnEnable() => _quadtree?.AddTarget(this);

    protected virtual void OnDisable()
    {
        if (_node != null)
            _quadtree?.RemoveTarget(_node, this, true);
    }

    private void OnDestroy()
    {
        if (_node != null)
            _quadtree?.RemoveTarget(_node, this, true);
    }

}
public enum TargetTypes { Player, Enemy, Ally}