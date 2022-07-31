using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class HoleMovement : MonoBehaviour
{

    [Header("Hole mesh")]
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;


    [Header("Hole vertices radius")]
    public Vector2 moveLimits;
    public float radius;
    public Transform holeCenter;

    Mesh mesh;
    List<int> holeVertices;
    List<Vector3> offSets;
    int holeVerticesCount;

    float x, y;
    Vector3 touch, targetPos;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {

        GameManager.Instance.isGameOver = false;
        GameManager.Instance.isMoving = false;

        holeVertices = new List<int>();
        offSets = new List<Vector3>();

        mesh = meshFilter.mesh;

        //find hole vertices in mesh
        FindHoleVertices();
    }

    void FindHoleVertices()
    {
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            float distance = Vector3.Distance(holeCenter.position, mesh.vertices[i]);

            if(distance < radius)
            {
                holeVertices.Add(i);
                offSets.Add(mesh.vertices[i] - holeCenter.position);
            }

        }

        holeVerticesCount = holeVertices.Count;
    }

    private void Update()
    {
        
        GameManager.Instance.isMoving = Input.GetMouseButton(0);

        if(!GameManager.Instance.isGameOver && GameManager.Instance.isMoving)
        {
            MoveHole();

            UpdateHoleVerticesPosition();

        }

    }


   void MoveHole()
   {
        x = Input.GetAxis("Mouse X");
        y = Input.GetAxis("Mouse Y");

        //lerp (smooth) movement
        touch = Vector3.Lerp(holeCenter.position, holeCenter.position + new Vector3(x, 0f, y), moveSpeed * Time.deltaTime
        );

        targetPos = new Vector3(Mathf.Clamp(touch.x, -moveLimits.x, moveLimits.x),touch.y,Mathf.Clamp(touch.z, -moveLimits.y, moveLimits.y));

        holeCenter.position = targetPos;
    }

    void UpdateHoleVerticesPosition()
    {
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < holeVerticesCount; i++)
        {
            vertices[holeVertices[i]] = holeCenter.position + offSets[i];

        }

        //update mesh
        mesh.vertices = vertices;
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(holeCenter.position, radius);
    }

}
