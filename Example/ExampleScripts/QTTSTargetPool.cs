using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTTSTargetPool : MonoBehaviour
{
    [SerializeField]
    private QTTSSpawnableTarget _targetPrefab = null;
    [SerializeField]
    private QTTSQuadtree _quadTree = null;

    [SerializeField]
    private float _spawnRadius = 5f;

    private void Start() => AddTarget(_maxTargetAmount);

    #region Pool
    public void ReturnToPool(QTTSSpawnableTarget target)
    {
        target.gameObject.SetActive(false);
        TargetQueue.Enqueue(target);
        _currentTargetAmount--;
    }

    private Queue<QTTSSpawnableTarget> TargetQueue = new Queue<QTTSSpawnableTarget>();

    private QTTSSpawnableTarget GetTarget()
    {
        if (TargetQueue.Count == 0)
        {
            AddTarget(1);
            _currentTargetAmount++;
        }
        return TargetQueue.Dequeue();
    }

    private void AddTarget(int count)
    {
        for (int i = 0; i < count; i++)
        {
            QTTSSpawnableTarget target = Instantiate(_targetPrefab);
            target.transform.SetParent(transform);
            target.SetPoolTo(this);
            target.SetQuadTree(_quadTree);
            target.GetComponent<QTTSTargetEveryFrame>().SetTargetPool(this);
            target.gameObject.SetActive(false);
            TargetQueue.Enqueue(target);
        }
    }
    #endregion

    //Spawning For Example Purposes
    //If you actually want the pool feel free to remove everything under this as well as the references to the target amounts

    [SerializeField]
    private int _maxTargetAmount = 10;
    public int _currentTargetAmount = 0;

    private void Update()
    {
        if(_currentTargetAmount < _maxTargetAmount)
        {
            SpawnTarget();
        }
    }
    private void SpawnTarget()
    {
        QTTSSpawnableTarget target = GetTarget();
        target.transform.position = new Vector3(Random.Range(-_spawnRadius, _spawnRadius), .5f, Random.Range(-_spawnRadius, _spawnRadius));
        target.gameObject.SetActive(true);
        _currentTargetAmount++;
    }

    //List for comparison purposes
    private void AddToList(QTTSTarget target)
    {
        switch (target.GetTargetType())
        {
            case TargetTypes.Player:
                Players.Add(target);
                break;
            case TargetTypes.Ally:
                Allies.Add(target);
                break;
            case TargetTypes.Enemy:
                Enemies.Add(target);
                break;
        }
    }

    public List<QTTSTarget> Players = new List<QTTSTarget>();
    public List<QTTSTarget> Allies = new List<QTTSTarget>();
    public List<QTTSTarget> Enemies = new List<QTTSTarget>();
}
