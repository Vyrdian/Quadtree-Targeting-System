using System;
using UnityEngine;

public class QTTSQuadtree : MonoBehaviour
{
    [SerializeField][Tooltip("Place this at or near the center of the area you need")]
    private Vector3 _center;
    [SerializeField][Tooltip("Make sure this covers the entire area from the center that you need")]
    private float _radius;

    [SerializeField]
    [Tooltip("Choose the coordinate pair you would like to use(generally XY for 2D, XZ for 3D")]
    private CoordinatePairs _startingCoordinatePair;
    public CoordinatePairs CoordinatePair { get; private set; }

    [SerializeField][Min(1)][Tooltip("You can change this to fit your needs, but this seems to be a decent sweet spot")]
    private int _maxTargetsPerNode = 5;

    private QTTSQuadtreeNode _mainNode;

    private void Awake()
    {
        CoordinatePair = _startingCoordinatePair;
        InitializeMainNode();
    }

    #region Target Addition and Removal
    public void AddTarget(QTTSTarget target)
    {
        QTTSQuadtreeNode currentNode = FindChildlessNodeForNewTarget(target, _mainNode);
        currentNode.Targets.Add(target);
        target.SetNewNode(currentNode);
        CheckForSubdivision(target, currentNode);
    }

    public void RemoveTarget(QTTSQuadtreeNode node, QTTSTarget target, bool targetDisabledOrDestroyed)
    {
        node.Targets.Remove(target);
        node.AmountOfTargetsPerType[(int)target.GetTargetType()]--;
        while (node.Parent != null)
        {
            node = node.Parent;
            node.AmountOfTargetsPerType[(int)target.GetTargetType()]--;
        }
    }

    private QTTSQuadtreeNode FindChildlessNodeForNewTarget(QTTSTarget target, QTTSQuadtreeNode currentNode)
    {
        while (currentNode.Children.Count > 0)
        {
            foreach (QTTSQuadtreeNode child in currentNode.Children)
            {
                if (NodeContainsPoint(child, target.PosX, target.PosYZ))
                {
                    currentNode.AmountOfTargetsPerType[(int)target.GetTargetType()]++;
                    currentNode = child;
                    break;
                }
                if (child == currentNode.Children[currentNode.Children.Count - 1])
                    return currentNode;
            }
        }
        return currentNode;
    }
    #endregion


    #region Target Search

    public QTTSTarget ClosestTargetOfTypesToPosition(float positionX, float positionZ, TargetTypes[] types, float searchRadius = 0, QTTSTarget searcherToIgnore = null)
    {
        if (!AvailableTarget(types, searcherToIgnore))
            return null;

        QTTSQuadtreeNode nodeToCheck = types.Length > 1? FindChildlessNodeForClosestTargetOfMultipleTypes(positionX, positionZ, types, _mainNode, searcherToIgnore) : FindChildlessNodeForClosestTarget(positionX, positionZ, types[0], _mainNode, searcherToIgnore);
        float distance = float.MaxValue;
        QTTSTarget newTarget = null;
        foreach (QTTSTarget target in nodeToCheck.Targets)
        {
            if (SqrMagnitudeBetweenPoints(positionX, positionZ, target.PosX, target.PosYZ) < distance && TargetTypeContainedWithinListOfTypes(target.GetTargetType(), types) && searcherToIgnore != target ? true : false)
                newTarget = target;
        }
        return searchRadius == 0 ? newTarget : SqrMagnitudeBetweenPoints(positionX, positionZ, newTarget.PosX, newTarget.PosYZ) <= searchRadius * searchRadius ? newTarget : null;
    }

    private bool TargetTypeContainedWithinListOfTypes(TargetTypes targetType, TargetTypes[] types)
    {
        foreach (TargetTypes type in types)
        {
            if (targetType == type)
                return true;
        }
        return false;
    }

    private bool AvailableTarget(TargetTypes[] types, QTTSTarget searcherToIgnore)
    {
        bool noTarget = true;
        foreach (TargetTypes type in types)
        {
            if (searcherToIgnore == null && _mainNode.AmountOfTargetsPerType[(int)type] > 0)
            {
                noTarget = false;
                break;
            }
            else if (searcherToIgnore != null && searcherToIgnore.GetTargetType() == type && _mainNode.AmountOfTargetsPerType[(int)type] <= 1 || searcherToIgnore.GetTargetType() != type && _mainNode.AmountOfTargetsPerType[(int)type] <= 1)
            {
                noTarget = false;
                break;
            }
        }
        return noTarget;
    }

    private QTTSQuadtreeNode FindChildlessNodeForClosestTarget(float positionX, float positionZ, TargetTypes type, QTTSQuadtreeNode nodeToCheck, QTTSTarget searcherToIgnore = null)
    {
        while (nodeToCheck.Children.Count > 0)
        {
            foreach (QTTSQuadtreeNode child in nodeToCheck.Children)
            {
                if (NodeContainsPoint(child, positionX, positionZ) && searcherToIgnore == null ? child.AmountOfTargetsPerType[(int)type] > 0 : child.AmountOfTargetsPerType[(int)type] > 1)
                {
                    nodeToCheck = child;
                    break;
                }
                if (child == nodeToCheck.Children[nodeToCheck.Children.Count - 1])
                    return nodeToCheck;
            }
        }
        return nodeToCheck;
    }

    private QTTSQuadtreeNode FindChildlessNodeForClosestTargetOfMultipleTypes(float positionX, float positionYZ, TargetTypes[] types, QTTSQuadtreeNode nodeToCheck, QTTSTarget searcherToIgnore = null)
    {
        while (nodeToCheck.Children.Count > 0)
        {
            bool breakMiddleLoop = false;
            foreach (QTTSQuadtreeNode child in nodeToCheck.Children)
            {
                if (breakMiddleLoop)
                    break;
                foreach (TargetTypes type in types)
                {
                    if (NodeContainsPoint(child, positionX, positionYZ) && searcherToIgnore == null ? child.AmountOfTargetsPerType[(int)type] > 0 : child.AmountOfTargetsPerType[(int)type] > 1)
                    {
                        nodeToCheck = child;
                        breakMiddleLoop = true;
                        break;
                    }
                }
                if (child == nodeToCheck.Children[nodeToCheck.Children.Count - 1])
                    return nodeToCheck;
            }
        }
        return nodeToCheck;
    }

    private float SqrMagnitudeBetweenPoints(float startPointX, float startPointYZ, float targetX, float targetYZ) => targetX - startPointX * targetX - startPointX + targetYZ - startPointYZ * targetYZ - startPointYZ;
    #endregion


    #region Node Creation and Checks
    private void InitializeMainNode()
    {
        switch (CoordinatePair)
        {
            case CoordinatePairs.XY:
                _mainNode = new QTTSQuadtreeNode(null, _center.x, _center.y, _radius);
                _mainNode.AmountOfTargetsPerType = new int[Enum.GetNames(typeof(TargetTypes)).Length];
                break;
            case CoordinatePairs.XZ:
                _mainNode = new QTTSQuadtreeNode(null, _center.x, _center.z, _radius);
                _mainNode.AmountOfTargetsPerType = new int[Enum.GetNames(typeof(TargetTypes)).Length];
                break;
        }
    }

    private void CreateNode(QTTSQuadtreeNode parent, float centerX, float centerYZ, float radius) => new QTTSQuadtreeNode(parent, centerX, centerYZ, radius);

    private void CheckForSubdivision(QTTSTarget target, QTTSQuadtreeNode currentNode)
    {
        currentNode.AmountOfTargetsPerType[(int)target.GetTargetType()]++;
        if (currentNode.Targets.Count > _maxTargetsPerNode)
            SubdivideNode(currentNode);
    }

    private void SubdivideNode(QTTSQuadtreeNode node)
    {
        CreateNode(node, node.MaxX - node.Radius / 2, node.MaxYZ - node.Radius / 2, node.Radius / 2);
        CreateNode(node, node.MaxX + node.Radius / 2, node.MaxYZ - node.Radius / 2, node.Radius / 2);
        CreateNode(node, node.MaxX - node.Radius / 2, node.MaxYZ + node.Radius / 2, node.Radius / 2);
        CreateNode(node, node.MaxX + node.Radius / 2, node.MaxYZ + node.Radius / 2, node.Radius / 2);
        DistributeTargetsToChildren(node);
    }

    private void DistributeTargetsToChildren(QTTSQuadtreeNode node)
    {
        foreach (QTTSTarget target in node.Targets)
            foreach (QTTSQuadtreeNode child in node.Children)
            {
                
                if (NodeContainsPoint(child, target.PosX , target.PosYZ))
                {
                    child.Targets.Add(target);
                    target.SetNewNode(child);
                    break;
                }
            }
        node.Targets.Clear();
    }

    private bool NodeContainsPoint(QTTSQuadtreeNode node, float x, float yz) => node.MinX <= x && node.MaxX >= x && node.MinYZ <= yz && node.MaxYZ >= yz ? true : false;
    #endregion
}
public enum CoordinatePairs { XY, XZ }