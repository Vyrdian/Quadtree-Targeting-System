using System;
using UnityEngine;

public class QTTSQuadtree : MonoBehaviour
{
    [SerializeField][Tooltip("This area should cover the entire area your targets can be in")]
    private float _centerX, _centerZ, _radius;

    [SerializeField][Min(1)]
    private int _maxTargetsPerNode = 10;

    private QTTSQuadtreeNode _mainNode;

    private void Awake() => InitializeMainNode();

    #region Target Search

    //Use this when searching for a single target type. If you have a maximum search range, fill in searchRadius. 
    //If your searcher is also a target and shares the same type as the search is looking for, fill in searcherToIgnore.
    public QTTSTarget ClosestTargetOfTypeToPosition(float positionX, float positionZ, TargetTypes type, float searchRadius = 0, QTTSTarget searcherToIgnore = null)
    {
        if (_mainNode.AmountOfTargetsPerType[(int)type] <= 0 || searcherToIgnore == null && _mainNode.AmountOfTargetsPerType[(int)type] <= 1)
            return null;

        QTTSQuadtreeNode nodeToCheck = FindChildlessNodeForClosestTarget(positionX, positionZ, type, _mainNode, searcherToIgnore);
        float distance = float.MaxValue;
        QTTSTarget newTarget = null;
        foreach (QTTSTarget target in nodeToCheck.Targets)
        {
            if (SquareMagnitudeBetweenStartAndTarget(positionX, positionZ, target.PositionX, target.PositionZ) < distance && target.GetTargetType() == type && searcherToIgnore != target ? true : false)
                newTarget = target;
        }
        return searchRadius == 0 ? newTarget : SquareMagnitudeBetweenStartAndTarget(positionX, positionZ, newTarget.PositionX, newTarget.PositionZ) <= searchRadius * searchRadius ? newTarget : null;
    }

    //Use this when searching for multiple target types at once. If you have a maximum search range, fill in searchRadius. 
    //If your searcher is also a target and shares the same type as one that the search is looking for, fill in searcherToIgnore.
    public QTTSTarget ClosestTargetOfTypesToPosition(float positionX, float positionZ, TargetTypes[] types, float searchRadius = 0, QTTSTarget searcherToIgnore = null)
    {
        if (!AvailableTargetFromMultipleTypes(types, searcherToIgnore))
            return null;

        QTTSQuadtreeNode nodeToCheck = FindChildlessNodeForClosestTargetOfMultipleTypes(positionX, positionZ, types, _mainNode, searcherToIgnore);
        float distance = float.MaxValue;
        QTTSTarget newTarget = null;
        foreach (QTTSTarget target in nodeToCheck.Targets)
        {
            if (SquareMagnitudeBetweenStartAndTarget(positionX, positionZ, target.PositionX, target.PositionZ) < distance && TargetTypeContainedWithinListOfTypes(target.GetTargetType(), types) && searcherToIgnore != target ? true : false)
                newTarget = target;
        }
        return searchRadius == 0 ? newTarget : SquareMagnitudeBetweenStartAndTarget(positionX, positionZ, newTarget.PositionX, newTarget.PositionZ) <= searchRadius * searchRadius ? newTarget : null;
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

    private bool AvailableTargetFromMultipleTypes(TargetTypes[] multipleTypes, QTTSTarget searcherToIgnore)
    {
        bool noTarget = true;
        foreach (TargetTypes type in multipleTypes)
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

    private QTTSQuadtreeNode FindChildlessNodeForClosestTargetOfMultipleTypes(float positionX, float positionZ, TargetTypes[] types, QTTSQuadtreeNode nodeToCheck, QTTSTarget searcherToIgnore = null)
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
                    if (NodeContainsPoint(child, positionX, positionZ) && searcherToIgnore == null ? child.AmountOfTargetsPerType[(int)type] > 0 : child.AmountOfTargetsPerType[(int)type] > 1)
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

    private float SquareMagnitudeBetweenStartAndTarget(float startPointX, float startPointZ, float targetX, float targetZ) => targetX - startPointX * targetX - startPointX + targetZ - startPointZ * targetZ - startPointZ;
    #endregion


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
                if (NodeContainsPoint(child, target.PositionX, target.PositionZ))
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


    #region Node Creation and Checks
    private void InitializeMainNode()
    {
        _mainNode = new QTTSQuadtreeNode(null, _centerX, _centerZ, _radius);
        _mainNode.AmountOfTargetsPerType = new int[Enum.GetNames(typeof(TargetTypes)).Length];
    }

    private void CreateNode(QTTSQuadtreeNode parent, float centerX, float centerZ, float radius) => new QTTSQuadtreeNode(parent, centerX, centerZ, radius);

    private void CheckForSubdivision(QTTSTarget target, QTTSQuadtreeNode currentNode)
    {
        currentNode.AmountOfTargetsPerType[(int)target.GetTargetType()]++;
        if (currentNode.Targets.Count > _maxTargetsPerNode)
            SubdivideNode(currentNode);
    }

    private void SubdivideNode(QTTSQuadtreeNode node)
    {
        CreateNode(node, node.MaxX - node.Radius / 2, node.MaxZ - node.Radius / 2, node.Radius / 2);
        CreateNode(node, node.MaxX + node.Radius / 2, node.MaxZ - node.Radius / 2, node.Radius / 2);
        CreateNode(node, node.MaxX - node.Radius / 2, node.MaxZ + node.Radius / 2, node.Radius / 2);
        CreateNode(node, node.MaxX + node.Radius / 2, node.MaxZ + node.Radius / 2, node.Radius / 2);
        DistributeTargetsToChildren(node);
    }

    private void DistributeTargetsToChildren(QTTSQuadtreeNode node)
    {
        foreach (QTTSTarget target in node.Targets)
            foreach (QTTSQuadtreeNode child in node.Children)
            {
                if (NodeContainsPoint(child, target.PositionX, target.PositionZ))
                {
                    child.Targets.Add(target);
                    break;
                }
            }
        node.Targets.Clear();
    }

    private bool NodeContainsPoint(QTTSQuadtreeNode node, float x, float z) => node.MinX <= x && node.MaxX >= x && node.MinZ <= z && node.MaxZ >= z ? true : false;
    #endregion
}