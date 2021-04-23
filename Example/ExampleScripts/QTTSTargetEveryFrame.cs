using System;
using UnityEngine;

public class QTTSTargetEveryFrame : MonoBehaviour
{
    private QTTSTargetFinder _targetSystem;
    private QTTSTarget _target;
    private QTTSTargetPool _pool;

    [SerializeField]
    TargetTypes[] _searchTypes;

    void Start()
    {
        if(GetComponent<QTTSTargetFinder>())
            _targetSystem = GetComponent<QTTSTargetFinder>();
        if (GetComponent<QTTSTarget>())
            _target = GetComponent<QTTSTarget>();
    }

    void Update()
    {
        TargetUsingQuadTree();
        TargetUsingList();
    }

    private void TargetUsingQuadTree() => _targetSystem?.SetTargetClosestToCurrentPosition();

    private void TargetUsingList()
    {
        float distance = float.MaxValue;
        QTTSTarget newTarget;
        foreach(TargetTypes type in _searchTypes)
        {
            switch (type)
            {
                case TargetTypes.Player:
                    foreach (QTTSTarget target in _pool.Enemies)
                    {
                        if (SquareMagnitudeBetweenStartAndTarget(transform.position.x, transform.position.z, target.PositionX, target.PositionZ) < distance)
                        {
                            newTarget = target;
                            distance = SquareMagnitudeBetweenStartAndTarget(transform.position.x, transform.position.z, target.PositionX, target.PositionZ);
                        }
                    }
                    break;
                case TargetTypes.Ally:
                    foreach (QTTSTarget target in _pool.Enemies)
                    {
                        if (SquareMagnitudeBetweenStartAndTarget(transform.position.x, transform.position.z, target.PositionX, target.PositionZ) < distance)
                        {
                            newTarget = target;
                            distance = SquareMagnitudeBetweenStartAndTarget(transform.position.x, transform.position.z, target.PositionX, target.PositionZ);
                        }
                    }
                    break;
                case TargetTypes.Enemy:
                    foreach (QTTSTarget target in _pool.Enemies)
                    {
                        if (SquareMagnitudeBetweenStartAndTarget(transform.position.x, transform.position.z, target.PositionX, target.PositionZ) < distance)
                        {
                            newTarget = target;
                            distance = SquareMagnitudeBetweenStartAndTarget(transform.position.x, transform.position.z, target.PositionX, target.PositionZ);
                        }
                    }
                    break;
            }
        }
    }

    public void SetTargetPool(QTTSTargetPool pool) => _pool = pool;

    private float SquareMagnitudeBetweenStartAndTarget(float startPointX, float startPointZ, float targetX, float targetZ) => targetX - startPointX * targetX - startPointX + targetZ - startPointZ * targetZ - startPointZ;
}
