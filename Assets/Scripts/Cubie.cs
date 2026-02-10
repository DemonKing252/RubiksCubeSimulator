using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cubie : MonoBehaviour
{
    public List<Side> cubeFace;
    public RubiksCubeManager cubeMan;
    public Vector3 diff;
    public float dotProduct;
    public Vector3 startPos;
    public Quaternion currentRot;
    public CubieFace cubieFace;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cubeMan = transform.parent.GetComponent<RubiksCubeManager>();
    }

    // Update is called once per frame

    // Right Vector is actually Back Vector
    // Back Vector is actually Left Vector
    void Update()
    {
        Vector3 R = (transform.position - Vector3.zero);
        dotProduct = Vector3.Dot(R, Camera.main.transform.right);
    }
    // Right = Backward
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Camera.main.transform.right);
    }

    public void _OnMouseDown(CubieFace cubieFace)
    {
        this.cubieFace = cubieFace;
        cubeMan.CheckClickedSide(this);
    }
    public void _OnMouseUp(CubieFace cubieFace)
    {
        this.cubieFace = null;
        cubeMan.OnCubeReleased(this);        
        GetComponent<MeshRenderer>().material.color = Color.black;
    }
}
