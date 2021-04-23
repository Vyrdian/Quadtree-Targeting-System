using UnityEngine;

public class QTTSTarget : MonoBehaviour
{
    [SerializeField][Tooltip("If the target starts in scene, use this. Otherwise, inject the quadtree with SetQuadTree")]
    private QTTSQuadtree _quadtree;

    [SerializeField]
    private TargetTypes _targetType;
    public float PositionX { get; private set; }
    public float PositionZ { get; private set; }

    private QTTSQuadtreeNode _node;

    public void SetQuadTree(QTTSQuadtree quadtree)
    {
        _quadtree = quadtree;
        quadtree.AddTarget(this);
    }

    public TargetTypes GetTargetType() => _targetType;

    #region Position Changing
    // Call this whenever this target moves
    public void UpdatePosition()
    {
        (PositionX, PositionZ) = (transform.position.x, transform.position.z);
        if (_node != null)
            CheckIfInDifferentNode();
    }

    public void SetNewNode(QTTSQuadtreeNode node) => _node = node;

    private void CheckIfInDifferentNode()
    {
        if (!StillInSameNode())
            ChangeNode();
    }

    private bool StillInSameNode() => _node.MinX <= PositionX && _node.MaxX >= PositionX && _node.MinZ <= PositionZ && _node.MaxZ >= PositionZ ? true : false;

    private void ChangeNode()
    {
        _quadtree.AddTarget(this);
        _quadtree.RemoveTarget(_node, this, false);
    }
    #endregion

    private void OnEnable()
    {
        (PositionX, PositionZ) = (transform.position.x, transform.position.z);
        _quadtree?.AddTarget(this);
    }

    protected virtual void OnDisable() => _quadtree?.RemoveTarget(_node, this, true);

    private void OnDestroy() => _quadtree?.RemoveTarget(_node, this, true);

}
public enum TargetTypes { Player, Enemy, Ally}