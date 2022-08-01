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

    public float scaleMultiplier;

    public static HoleMovement Instance;

    public GameObject upgradePanel;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

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
        touch = Vector3.Lerp(holeCenter.position, holeCenter.position + new Vector3(-y, 0f, x), moveSpeed * Time.deltaTime);

        targetPos = new Vector3(Mathf.Clamp(touch.x, -moveLimits.x, moveLimits.x),touch.y,Mathf.Clamp(touch.z, -moveLimits.y, moveLimits.y));

        holeCenter.position = targetPos;
    }

    public void SetHoleScale(float multiplier) 
    {
        /*
         * 
         *FIRST VERSION OF SET HOLE SCALE FUNCTION
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < holeVerticesCount; i++)
        {
            vertices[holeVertices[i]] = holeCenter.position + offSets[i] * multiplier;

        }

        //update mesh
        mesh.vertices = vertices;
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        holeCenter.localScale *= multiplier;

        for (int i = 0; i < offSets.Count; i++)
        {
            offSets[i] *= multiplier;
        }

        moveLimits /= 1.3f; // set up correctly
        
        */
        IEnumerator c = SetHoleScaleCoroutine(multiplier);
        StartCoroutine(c);

    }

    IEnumerator SetHoleScaleCoroutine(float multiplier)
    {
        //if > offsets, then dont scale

        Vector3[] vertices = mesh.vertices;
        Vector3 holeCenterLocalScaleEnd = holeCenter.localScale * multiplier;

        List<Vector3> offSetsEnd = new List<Vector3>();

        for (int i = 0; i < offSets.Count; i++)
        {
            offSetsEnd.Add(offSets[i] * multiplier);
        }

        while (vertices[holeVertices[0]] != holeCenter.position + offSets[0] * multiplier)
        {
            for (int i = 0; i < holeVerticesCount; i++)
            {
                vertices[holeVertices[i]] = Vector3.Lerp(vertices[holeVertices[i]], holeCenter.position + offSetsEnd[i], 0.5f);


            }

            //update mesh
            mesh.vertices = vertices;
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;

            //****************

            holeCenter.localScale = Vector3.Lerp(holeCenter.localScale, holeCenterLocalScaleEnd, 0.4f);


            yield return null;
        }

        for (int i = 0; i < offSets.Count; i++)
        {
            offSets[i] *= multiplier;
        }
        

        moveLimits /= 1.3f; // set up correctly
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


    public void UpdateRadius()
    {
        SetHoleScale(1.5f);
    }

    public void UpdateSpeed()
    {
        moveSpeed += 1;
    }


    public void UpdateCapacity()
    {
        FallCollider.Instance.collectiblesMaxCount += 3;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "upgrade")
        {
            Debug.Log("Upgrade panel open");
            upgradePanel.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "upgrade")
        {
            Debug.Log("Upgrade panel closed");
            upgradePanel.SetActive(false);

        }
    }


}
