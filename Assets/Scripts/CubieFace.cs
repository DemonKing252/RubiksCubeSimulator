using UnityEngine;

public enum LayerID
{
    // We are treating White/Yellow as the same layer for a specific reason. 
    // When the camera flips to Yellow side, the math and cube notation is all the same.
    TopLayer,   
    MiddleLayer,
    None,
}
public class CubieFace : MonoBehaviour
{
    void OnMouseDown()
    {
        transform.parent.GetComponent<Cubie>()._OnMouseDown(this, gameObject.tag == "CentreLayer" ? LayerID.MiddleLayer : LayerID.TopLayer);
    }

    void OnMouseUp()
    {
        transform.parent.GetComponent<Cubie>()._OnMouseUp(this, gameObject.tag == "CentreLayer" ? LayerID.MiddleLayer : LayerID.TopLayer);
    }
}
