public class QTTSSpawnableTarget : QTTSTarget
{
    private QTTSTargetPool _pool;

    public void SetPoolTo(QTTSTargetPool pool) => _pool = pool;

    protected override void OnDisable()
    {
        base.OnDisable();
        _pool.ReturnToPool(this);
    }
}
