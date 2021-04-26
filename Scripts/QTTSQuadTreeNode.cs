using System.Collections.Generic;

public class QTTSQuadtreeNode
{
    public QTTSQuadtreeNode(QTTSQuadtreeNode parent, float centerX, float centerZ, float radius)
    {
        (Parent, MinX, MaxX, MinYZ, MaxYZ, Radius) = (Parent, centerX - radius, centerX + radius, centerZ - radius, centerZ + radius, radius);
        if (Parent != null)
        {
            AmountOfTargetsPerType = new int[parent.AmountOfTargetsPerType.Length];
            Parent.Children.Add(this);
        }
        Targets = new List<QTTSTarget>();
        Children = new List<QTTSQuadtreeNode>();
    }

    public QTTSQuadtreeNode Parent { get; private set; }
    public float MinX { get; private set; }
    public float MaxX { get; private set; }
    public float MinYZ { get; private set; }
    public float MaxYZ { get; private set; }
    public float Radius { get; private set; }
    public List<QTTSQuadtreeNode> Children { get; private set; }
    public int[] AmountOfTargetsPerType;

    public List<QTTSTarget> Targets { get; private set; }
}