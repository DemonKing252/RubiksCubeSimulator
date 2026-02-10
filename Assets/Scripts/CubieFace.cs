using UnityEngine;


public class CubieFace : MonoBehaviour
{
    void OnMouseDown()
    {
        transform.parent.GetComponent<Cubie>()._OnMouseDown(this);
    }

    void OnMouseUp()
    {
        transform.parent.GetComponent<Cubie>()._OnMouseUp(this);
    }
}
