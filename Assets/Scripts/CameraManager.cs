using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public enum CameraVert
{
    Up,
    Down
}

public enum CameraHoriz
{
    Minus_X,
    Minus_Z,
    Plus_X,
    Plus_Z,
    Count
}

public class CameraManager : MonoBehaviour
{
    private static CameraManager sInstance;
    public static CameraManager Instance
    {
        get { return sInstance; }
    }
    
    public CameraVert cameraVert;
    public CameraHoriz cameraHoriz;
    public Dictionary<CameraHoriz, Tuple<Vector3, Vector3>> cameraParams = new Dictionary<CameraHoriz, Tuple<Vector3, Vector3>>()
    {
        { CameraHoriz.Minus_X,  new Tuple<Vector3,Vector3>(new Vector3(-6f, 3f, 0f), new Vector3(20f, 90f, 0f))},
        { CameraHoriz.Minus_Z, new Tuple<Vector3,Vector3>(new Vector3(0f, 3f, -6f), new Vector3(20f, 0f, 0f))},
        { CameraHoriz.Plus_X, new Tuple<Vector3,Vector3>(new Vector3(+6f, 3f, 0f), new Vector3(20f, 270f, 0f))},
        { CameraHoriz.Plus_Z, new Tuple<Vector3,Vector3>(new Vector3(0f, 3f, +6f), new Vector3(20f, 180f, 0f))},
    };
    
    public GameObject cameraPlane;

    public Camera rightCamera;
    public Camera leftCamera;

    void Awake()
    {
        sInstance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AlignSideCameras(0, CameraVert.Up);
    }
    void AlignSideCameras(int camera_face_index, CameraVert cameraVert)
    {
        int right_camera_index = (camera_face_index + 1) % (int)CameraHoriz.Count;
        int left_camera_axis_index = (camera_face_index - 1 + (int)CameraHoriz.Count) % (int)CameraHoriz.Count;

        if (cameraVert == CameraVert.Up)
        {            
            int temp = right_camera_index;
            right_camera_index = left_camera_axis_index;
            left_camera_axis_index = temp;
        }

        // -----------------------------------------------------
        // Left camera
        rightCamera.transform.position = new Vector3(cameraParams[(CameraHoriz)left_camera_axis_index].Item1.x,
                                        transform.position.y,
                                        cameraParams[(CameraHoriz)left_camera_axis_index].Item1.z);

        rightCamera.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 
                                              cameraParams[(CameraHoriz)left_camera_axis_index].Item2.y, 
                                              transform.rotation.eulerAngles.z);

        // -----------------------------------------------------
        // Right camera
        leftCamera.transform.position = new Vector3(cameraParams[(CameraHoriz)right_camera_index].Item1.x,
                                        transform.position.y,
                                        cameraParams[(CameraHoriz)right_camera_index].Item1.z);

        leftCamera.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 
                                              cameraParams[(CameraHoriz)right_camera_index].Item2.y, 
                                              transform.rotation.eulerAngles.z);
    }


    void OnRight(CameraVert cameraVert)
    {
        int camera_axis_index = (int)cameraHoriz;
        camera_axis_index = (camera_axis_index + 1) % (int)CameraHoriz.Count;

        
        transform.position = new Vector3(cameraParams[(CameraHoriz)camera_axis_index].Item1.x,
                                        transform.position.y,
                                        cameraParams[(CameraHoriz)camera_axis_index].Item1.z);

        cameraPlane.transform.position = new Vector3(cameraParams[(CameraHoriz)camera_axis_index].Item1.x,
                                        transform.position.y,
                                        cameraParams[(CameraHoriz)camera_axis_index].Item1.z);

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 
                                              cameraParams[(CameraHoriz)camera_axis_index].Item2.y, 
                                              transform.rotation.eulerAngles.z);

        cameraPlane.transform.rotation = Quaternion.Euler(0f, 
                                              cameraParams[(CameraHoriz)camera_axis_index].Item2.y, 
                                              cameraPlane.transform.rotation.eulerAngles.z);
        
        
        AlignSideCameras(camera_axis_index, cameraVert);

        cameraHoriz = (CameraHoriz)camera_axis_index;
        RubiksCubeManager.Instance.ReCallibrateCube();
    }
    void OnLeft(CameraVert cameraVert)
    {
        int camera_axis_index = (int)cameraHoriz;
        camera_axis_index = (camera_axis_index - 1 + (int)CameraHoriz.Count) % (int)CameraHoriz.Count;

        transform.position = new Vector3(cameraParams[(CameraHoriz)camera_axis_index].Item1.x,
                                        transform.position.y,
                                        cameraParams[(CameraHoriz)camera_axis_index].Item1.z);

        cameraPlane.transform.position = new Vector3(cameraParams[(CameraHoriz)camera_axis_index].Item1.x,
                                        transform.position.y,
                                        cameraParams[(CameraHoriz)camera_axis_index].Item1.z);

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 
                                              cameraParams[(CameraHoriz)camera_axis_index].Item2.y, 
                                              transform.rotation.eulerAngles.z);

        cameraPlane.transform.rotation = Quaternion.Euler(0f, 
                                              cameraParams[(CameraHoriz)camera_axis_index].Item2.y, 
                                              cameraPlane.transform.rotation.eulerAngles.z);

        AlignSideCameras(camera_axis_index, cameraVert);

        cameraHoriz = (CameraHoriz)camera_axis_index;
        RubiksCubeManager.Instance.ReCallibrateCube();
    }

    // Update is called once per frame
    void Update()
    {
        if (!RubiksCubeManager.Instance.Ready || RubiksCubeManager.Instance.inputLocked)
            return;

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            cameraVert = CameraVert.Up;
            transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
            cameraPlane.transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
            transform.rotation = Quaternion.Euler(+20f, transform.rotation.eulerAngles.y, 0f);
            cameraPlane.transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            RubiksCubeManager.Instance.ReCallibrateCube();
            
            AlignSideCameras((int)cameraHoriz, cameraVert);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            cameraVert = CameraVert.Down;
            transform.position = new Vector3(transform.position.x, -3f, transform.position.z);
            cameraPlane.transform.position = new Vector3(transform.position.x, -3f, transform.position.z);
            transform.rotation = Quaternion.Euler(-20f, transform.rotation.eulerAngles.y, 180f);
            cameraPlane.transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 180f);
            RubiksCubeManager.Instance.ReCallibrateCube();
            
            AlignSideCameras((int)cameraHoriz, cameraVert);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (cameraVert == CameraVert.Up)
                OnRight(CameraVert.Up);
            else
                OnLeft(CameraVert.Down);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (cameraVert == CameraVert.Up)
                OnLeft(CameraVert.Up);
            else
                OnRight(CameraVert.Down);
        }

    }
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.right);
        
    }
}
