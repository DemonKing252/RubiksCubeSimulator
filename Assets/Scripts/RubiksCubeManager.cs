using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public enum MouseScrollDir
{
    Up,
    Down,
    Left,
    Right,

    None
}

public enum Side
{
    Right = 0,
    Back = 1,
    Left = 2,
    Front = 3,
    Top = 4,
    Bottom = 5
}

[Serializable]
public class Cube_Face
{
    public Side side;
    public List<GameObject> sideList;
    public Vector3 originPoint;
}
public enum RotateDir
{
    CW,
    CCW
}

public class RubiksCubeManager : MonoBehaviour
{
    private static RubiksCubeManager sInstance;
    public static RubiksCubeManager Instance { get { return sInstance; } }

    private Cubie selectedCube;
    private List<Side> selectedSides;

    private bool ready = true;
    public bool Ready { get { return ready; } }
    public List<Cubie> cubies;

    public List<Cube_Face> cube_faces;

    public TMP_Text scrolldirText;
    public TMP_Text selectedCubeText;
    public TMP_Text selectedFaceText;
    public bool inputLocked = false;

    

    public Dictionary<CameraHoriz, Vector3> cameraVectors = new Dictionary<CameraHoriz, Vector3>()
    {
        { CameraHoriz.Minus_X, new Vector3(-1f, 0f, 0f)},
        { CameraHoriz.Minus_Z, new Vector3(0f, 0f, -1f)},
        { CameraHoriz.Plus_X, new Vector3(1f, 0f, 0f)},
        { CameraHoriz.Plus_Z, new Vector3(0f, 0f, 1f)},
    };

    public void ReCallibrateCube()
    {
        foreach (var f in cube_faces)
        {
            f.sideList.Clear();
        }
        foreach (Cubie go in cubies)
        {
            go.GetComponent<MeshRenderer>().material.color = Color.black;
        }

        foreach (var c in cubies)
        {
            // Vector3.ProjectOnPlane --> Zero's the Quaternion rotation while preserving world space position
            c.cubeFace.Clear();

            // Right face
            if (Vector3.Dot(c.transform.position - Vector3.zero, CameraManager.Instance.cameraPlane.transform.right) > 0.95f)
            {
                c.cubeFace.Add(Side.Right);
                cube_faces[(int)Side.Right].sideList.Add(c.gameObject);
                cube_faces[(int)Side.Right].originPoint = CameraManager.Instance.cameraPlane.transform.right;
            }

            // Left face
            if (Vector3.Dot(c.transform.position - Vector3.zero, -1f * CameraManager.Instance.cameraPlane.transform.right) > 0.95f)
            {
                c.cubeFace.Add(Side.Left);
                cube_faces[(int)Side.Left].sideList.Add(c.gameObject);
                cube_faces[(int)Side.Left].originPoint = -1f * CameraManager.Instance.cameraPlane.transform.right;
            }

            // Front face
            if (Vector3.Dot(c.transform.position - Vector3.zero, -1f * CameraManager.Instance.cameraPlane.transform.forward) > 0.95f)
            {
                c.cubeFace.Add(Side.Front);
                cube_faces[(int)Side.Front].sideList.Add(c.gameObject);
                cube_faces[(int)Side.Front].originPoint = -1f * CameraManager.Instance.cameraPlane.transform.forward;
            }

            // Back face
            if (Vector3.Dot(c.transform.position - Vector3.zero, CameraManager.Instance.cameraPlane.transform.forward) > 0.95f)
            {
                c.cubeFace.Add(Side.Back);
                cube_faces[(int)Side.Back].sideList.Add(c.gameObject);
                cube_faces[(int)Side.Back].originPoint = CameraManager.Instance.cameraPlane.transform.forward;
            }

            // Up Face
            if (Vector3.Dot(c.transform.position - Vector3.zero, CameraManager.Instance.cameraPlane.transform.up) > 0.95f)
            {
                c.cubeFace.Add(Side.Top);
                cube_faces[(int)Side.Top].sideList.Add(c.gameObject);
                cube_faces[(int)Side.Top].originPoint = CameraManager.Instance.cameraPlane.transform.up;
            }

            // Down Face
            if (Vector3.Dot(c.transform.position - Vector3.zero, -1f * CameraManager.Instance.cameraPlane.transform.up) > 0.95f)
            {
                c.cubeFace.Add(Side.Bottom);
                cube_faces[(int)Side.Bottom].sideList.Add(c.gameObject);
                cube_faces[(int)Side.Bottom].originPoint = -1f * CameraManager.Instance.cameraPlane.transform.up;
            }

        }
        foreach (var c in cubies)
        {
            c.GetComponent<Cubie>().startPos = c.transform.position;
            c.GetComponent<Cubie>().currentRot = c.transform.rotation;
        }
        ready = true;
    }
    void Awake()
    {
        sInstance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cubies = FindObjectsByType<Cubie>(FindObjectsSortMode.None).ToList();
        ReCallibrateCube();

        foreach (var f in cube_faces)
        {
            foreach (var c in f.sideList)
            {
                c.GetComponent<Cubie>().startPos = c.transform.localPosition;
                c.GetComponent<Cubie>().currentRot = c.transform.rotation;
            }
        }
        
    }
    IEnumerator DemoScramble()
    {   
        yield return StartCoroutine(RotateFace(Side.Right, RotateDir.CW));
        yield return StartCoroutine(RotateFace(Side.Left, RotateDir.CW));
        yield return StartCoroutine(RotateFace(Side.Top, RotateDir.CW));
        yield return StartCoroutine(RotateFace(Side.Bottom, RotateDir.CCW));
        yield return StartCoroutine(RotateFace(Side.Right, RotateDir.CW));
        yield return StartCoroutine(RotateFace(Side.Bottom, RotateDir.CCW));
        yield return StartCoroutine(RotateFace(Side.Right, RotateDir.CW));
        yield return StartCoroutine(RotateFace(Side.Bottom, RotateDir.CW));
        yield return StartCoroutine(RotateFace(Side.Back, RotateDir.CW));
        yield return StartCoroutine(RotateFace(Side.Top, RotateDir.CW));
        yield return StartCoroutine(RotateFace(Side.Bottom, RotateDir.CW));
        yield return StartCoroutine(RotateFace(Side.Front, RotateDir.CW));
        yield return StartCoroutine(RotateFace(Side.Front, RotateDir.CW));
        yield return StartCoroutine(RotateFace(Side.Back, RotateDir.CW));
    }

    public IEnumerator ScrambleCube(int s)
    {
        int seed = s;
        UnityEngine.Random.InitState(seed);
        int num_rotations = UnityEngine.Random.Range(15, 26); // 15 to 25

        Debug.Log("Num rotations: " + num_rotations);

        Action NextSeed = () =>
        {
            seed++;
            UnityEngine.Random.InitState(seed);
        };

        for (int rot = 0; rot < num_rotations; rot++)
        {
            NextSeed();
            int rand_dir_num = UnityEngine.Random.Range(0, 2); // 0 to 1
            RotateDir rand_dir = (rand_dir_num == 0 ? RotateDir.CW : RotateDir.CCW);
            
            NextSeed();
            int rand_face_num = UnityEngine.Random.Range(0, 6); // 0 to 5
            Side rand_side = (Side)rand_face_num;

            yield return StartCoroutine(RotateFace(rand_side, rand_dir, 0.05f));
        }

        yield return null;
    }

    IEnumerator MouseScrollBehaviour()
    {
        // Continue checking 
        while (selectedCube != null)
        {
            yield return null;
            Vector2 mouseDelta;

            // Wait until the player provides a strong enough scroll to register. 
            while (true)
            {
                mouseDelta = new Vector2(
                    Input.GetAxisRaw("Mouse X"),
                    Input.GetAxisRaw("Mouse Y")
                );

                if (mouseDelta.sqrMagnitude >= 0.004f)
                    break;
                
                if (Input.GetMouseButtonUp(0))  // Break if the player releases.
                    yield break;

                // Check again the next frame
                yield return null;
            }
            MouseScrollDir probableScrollDirection;

            if (Mathf.Abs(mouseDelta.x) > Mathf.Abs(mouseDelta.y))
                probableScrollDirection = mouseDelta.x > 0 ? MouseScrollDir.Right : MouseScrollDir.Left;
            else
                probableScrollDirection = mouseDelta.y > 0 ? MouseScrollDir.Up : MouseScrollDir.Down;

            scrolldirText.text = probableScrollDirection.ToString();

            // Check for sides that don't support rotation yet (Will add in v2.0)
            if (selectedCube == null)
                yield break;
            else if (selectedCube.cubeFace == null)
                yield break;

            // R -> Up
            if (probableScrollDirection == MouseScrollDir.Up && selectedCube.cubeFace.Contains(Side.Right))
            {
                yield return StartCoroutine(RotateFace(Side.Right, RotateDir.CW));
                OnCubeReleased(null);
                break;
            }
            
            // R -> Down
            if (probableScrollDirection == MouseScrollDir.Down && selectedCube.cubeFace.Contains(Side.Right))
            {               
                yield return StartCoroutine(RotateFace(Side.Right, RotateDir.CCW));
                OnCubeReleased(null);
                break;
            }

            // L -> Up
            if (probableScrollDirection == MouseScrollDir.Up && selectedCube.cubeFace.Contains(Side.Left))
            {
                yield return StartCoroutine(RotateFace(Side.Left, RotateDir.CCW));
                OnCubeReleased(null);
                break;
            }
            
            // L -> Down
            if (probableScrollDirection == MouseScrollDir.Down && selectedCube.cubeFace.Contains(Side.Left))
            {               
                yield return StartCoroutine(RotateFace(Side.Left, RotateDir.CW));
                OnCubeReleased(null);
                break;
            }

            if (selectedCube.cubeFace.Contains(Side.Top) && selectedCube.cubeFace.Contains(Side.Front))
            {
                // Scroll the Top.
                if (CameraManager.Instance.cameraVert == CameraVert.Up ? 
                    selectedCube.cubieFace.transform.position.y < 1.4f : 
                    selectedCube.cubieFace.transform.position.y > -1.4f)
                {
                    // U -> Left
                    if (probableScrollDirection == MouseScrollDir.Left)
                    {              
                        yield return StartCoroutine(RotateFace(Side.Top, RotateDir.CW));
                        OnCubeReleased(null);
                        break;
                    }

                    // U -> Right
                    if (probableScrollDirection == MouseScrollDir.Right)
                    {             
                        yield return StartCoroutine(RotateFace(Side.Top, RotateDir.CCW));
                        OnCubeReleased(null);
                        break;
                    }
                }
                // Scroll the front.
                else
                {
                    // F -> Left
                    if (probableScrollDirection == MouseScrollDir.Left)
                    {              
                        yield return StartCoroutine(RotateFace(Side.Front, RotateDir.CCW));
                        OnCubeReleased(null);
                        break;
                    }
    
                    // F -> Right
                    if (probableScrollDirection == MouseScrollDir.Right)
                    {             
                        yield return StartCoroutine(RotateFace(Side.Front, RotateDir.CW));
                        OnCubeReleased(null);
                        break;
                    }
                }
            }

            // B -> Left
            if (probableScrollDirection == MouseScrollDir.Left && selectedCube.cubeFace.Contains(Side.Back))
            {              
                yield return StartCoroutine(RotateFace(Side.Back, RotateDir.CW));
                OnCubeReleased(null);
                break;
            }

            // B -> Right
            if (probableScrollDirection == MouseScrollDir.Right && selectedCube.cubeFace.Contains(Side.Back))
            {             
                yield return StartCoroutine(RotateFace(Side.Back, RotateDir.CCW));
                OnCubeReleased(null);
                break;
            }

            // D -> Left
            if (probableScrollDirection == MouseScrollDir.Left && selectedCube.cubeFace.Contains(Side.Bottom))
            {              
                yield return StartCoroutine(RotateFace(Side.Bottom, RotateDir.CCW));
                OnCubeReleased(null);
                break;
            }

            // D -> Right
            if (probableScrollDirection == MouseScrollDir.Right && selectedCube.cubeFace.Contains(Side.Bottom))
            {             
                yield return StartCoroutine(RotateFace(Side.Bottom, RotateDir.CW));
                OnCubeReleased(null);
                break;
            }
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        if (inputLocked)
            return;


        if (Input.GetKeyUp(KeyCode.R))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                StartCoroutine(RotateFace(Side.Right, RotateDir.CCW));
            else
                StartCoroutine(RotateFace(Side.Right, RotateDir.CW));
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            if (Input.GetKey(KeyCode.LeftShift))            
                StartCoroutine(RotateFace(Side.Left, RotateDir.CCW));
            else
                StartCoroutine(RotateFace(Side.Left, RotateDir.CW));
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                StartCoroutine(RotateFace(Side.Front, RotateDir.CCW));
            else
                StartCoroutine(RotateFace(Side.Front, RotateDir.CW));
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                StartCoroutine(RotateFace(Side.Back, RotateDir.CCW));
            else
                StartCoroutine(RotateFace(Side.Back, RotateDir.CW));

        }
        if (Input.GetKeyUp(KeyCode.U))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                StartCoroutine(RotateFace(Side.Top, RotateDir.CCW));
            else
                StartCoroutine(RotateFace(Side.Top, RotateDir.CW));

        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                StartCoroutine(RotateFace(Side.Bottom, RotateDir.CCW));
            else
                StartCoroutine(RotateFace(Side.Bottom, RotateDir.CW));
        }
    }
    public void OnCubeReleased(Cubie cubie)
    {
        //selectedSides.Clear();
        selectedCube = null;
        //ReCallibrateCube();
    }
    public void CheckClickedSide(Cubie cubie)
    {
        string faces = null;
        foreach(Side s in cubie.cubeFace)
        {
            faces += s.ToString() + ", ";
        }

        Debug.Log("Pressing: " + cubie.name + " with faces: " + faces);
        selectedSides = cubie.cubeFace;
        
        selectedCube = cubie;
        
        selectedCube.GetComponent<MeshRenderer>().material.color = Color.cyan;

        selectedCubeText.text = selectedCube.name;
        string str = null;
        foreach (Side s in cubie.cubeFace)
            str += s + ", ";
        
        selectedFaceText.text = str;

        StartCoroutine(MouseScrollBehaviour());
    }

    IEnumerator RotateFace(Side s, RotateDir rotateDir, float time_max = 0.25f)
    {
        if (ready)
        {
            ready = false;
            float time = 0f;
            
            foreach (var c in cube_faces[(int)s].sideList)
                c.GetComponent<MeshRenderer>().material.color = Color.magenta;

            while (time < time_max)
            {   
                float angle = Mathf.Lerp(0f, 90f, time / time_max);
                Quaternion rotation = Quaternion.AngleAxis(rotateDir == RotateDir.CW ? angle : -angle, cube_faces[(int)s].originPoint);

                foreach (var c in cube_faces[(int)s].sideList)
                {
                    //                     Direction
                    // Origin Point      ------------->        Cube
                    Vector3 dir = c.GetComponent<Cubie>().startPos - cube_faces[(int)s].originPoint;

                    Vector3 rotatedDir = rotation * dir;

                    c.transform.localPosition = cube_faces[(int)s].originPoint + rotatedDir;

                    c.transform.rotation = rotation * c.GetComponent<Cubie>().currentRot;

                }

                time += Time.deltaTime;
                yield return null;
            }
            // Snap it back in place to avoid rounding issues:
            Quaternion snapped_rot = Quaternion.AngleAxis(rotateDir == RotateDir.CW ? 90f : -90f, cube_faces[(int)s].originPoint);

            foreach (var c in cube_faces[(int)s].sideList)
            {
                //                     Direction
                // Origin Point      ------------->        Cube
                Vector3 dir = c.GetComponent<Cubie>().startPos - cube_faces[(int)s].originPoint;

                Vector3 rotatedDir = snapped_rot * dir;

                c.transform.localPosition = cube_faces[(int)s].originPoint + rotatedDir;

                c.transform.rotation = snapped_rot * c.GetComponent<Cubie>().currentRot;

            }

            foreach (var c in cube_faces[(int)s].sideList)
            {
                
                c.GetComponent<MeshRenderer>().material.color = Color.black;
                c.GetComponent<Cubie>().startPos = c.transform.position;
                c.GetComponent<Cubie>().currentRot = c.transform.rotation;
            }
            ReCallibrateCube();
        }

        
    }
}
