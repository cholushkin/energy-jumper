using UnityEngine;

public class DecorationTriangleBasis : MonoBehaviour
{
    public Vector3 Left { get; private set; }
    public Vector3 Right { get; private set; }
    public Vector3 Peak { get; private set; }
    public Vector3 Normal { get; private set; }
    public float Scale { get; private set; }
    public bool IsPeakOnTop { get; private set; }

    private static readonly float SideLength = 16f;

    [ContextMenu("CalculateReferencePoints")]
    public void CalculateReferencePoints()
    {

        Vector3[] normals = GetComponent<MeshFilter>().sharedMesh.normals;
        Vector3[] verts = GetComponent<MeshFilter>().sharedMesh.vertices;

        Vector3 topVertex = new Vector3(0, float.NegativeInfinity, 0);
        Vector3 bottomVertex = new Vector3(0, float.PositiveInfinity, 0);
        Vector3 leftVertex = new Vector3(float.PositiveInfinity, 0, 0);
        Vector3 rightVertex = new Vector3(float.NegativeInfinity, 0, 0);

        for (int i = 0; i < verts.Length; i++)
        {
            var vert = transform.TransformPoint(verts[i]);
            var normal = transform.TransformPoint(verts[i] + normals[i]);

            // get only vertex that has normal faced to the viewer (-forward))
            var dot = Vector3.Dot(-transform.forward, normal);
            if (dot > 0f && Mathf.Abs(dot) > 0.7f)
            {
                if (vert.y > topVertex.y)
                    topVertex = vert;
                if (vert.y < bottomVertex.y)
                    bottomVertex = vert;
                if (vert.x < leftVertex.x)
                    leftVertex = vert;
                if (vert.x > rightVertex.x)
                    rightVertex = vert;
                Normal = normal;
            }
        }

        // calculate peak
        // for a wrong vertex y coordinate is almost the same as left or right coordinate
        var leftY = leftVertex.y;
        var topVertexDelta = Mathf.Abs(topVertex.y - leftY);
        var bottomVertexDelta = Mathf.Abs(bottomVertex.y - leftY);

        IsPeakOnTop = topVertexDelta > bottomVertexDelta;
        Peak = IsPeakOnTop ? topVertex : bottomVertex;
        Left = leftVertex;
        Right = rightVertex;
        Scale = (Left - Right).magnitude / SideLength;
    }

    void OnDrawGizmos()
    {

        //Vector3[] normals = GetComponent<MeshFilter>().sharedMesh.normals;
        //Vector3[] verts = GetComponent<MeshFilter>().sharedMesh.vertices;
        //DrawNormals(verts, normals);



        //CalculateReferencePoints();

        // lines of triangle and the center
        //Gizmos.color = Color.white;
        //Gizmos.DrawLine(Left, Peak);
        //Gizmos.DrawLine(Peak, Right);
        //Gizmos.DrawLine(Right, Left);
        //Gizmos.DrawSphere(transform.position, 0.1f);

        //// draw normal
        //Gizmos.DrawLine(transform.position, transform.position + Normal);

        //// normals

        //Vector3[] normals = GetComponent<MeshFilter>().sharedMesh.normals;
        //Vector3[] verts = GetComponent<MeshFilter>().sharedMesh.vertices;

        //for (int i = 0; i < normals.Length; i++)
        //{
        //    Vector3 vert = transform.TransformPoint(verts[i]);
        //    var wNormal = transform.TransformPoint(normals[i]).normalized;
        //    Gizmos.DrawLine(vert, vert + wNormal*3f);
        //}

        //// reference points
        //Gizmos.color = Color.blue;
        //Gizmos.DrawCube(Left, Vector3.one);

        //Gizmos.color = Color.green;
        //Gizmos.DrawCube(Right, Vector3.one);

        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(Peak, Vector3.one);
    }

    private void DrawNormals(Vector3[] verts, Vector3[] normals)
    {
        const float normalLength = 10f;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 vert = transform.TransformPoint(verts[i]);
            var normal = transform.TransformPoint(verts[i] + normals[i]);
            Gizmos.DrawLine(vert, vert + (normal-vert).normalized * normalLength);
        }

        //Gizmos.color = Color.red;
        //Gizmos.matrix = transform.localToWorldMatrix;
        //for (int i = 0; i < verts.Length; i++)
        //{
        //    Vector3 vert = (verts[i]);
        //    var normal = (normals[i]);
        //    Gizmos.DrawLine(vert, vert + normal.normalized * normalLength);
        //}
    }
}
