using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public int seedNumber;
    public string seed;
    public TMP_InputField seedTextField;
    public Button scrambleButton;
    public Button resetButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        seedTextField.onValueChanged.AddListener((string str) => OnUpdatedText(str) );
        resetButton.onClick.AddListener(() => SceneManager.LoadScene("SampleScene"));

        scrambleButton.onClick.AddListener(
            () => StartCoroutine(RubiksCubeManager.Instance.ScrambleCube(seedNumber)) 
        );
        
        OnUpdatedText(seedTextField.text);

        seedTextField.onSelect.AddListener((meta) => RubiksCubeManager.Instance.inputLocked = true );
        seedTextField.onDeselect.AddListener((meta) => RubiksCubeManager.Instance.inputLocked = false );
    }

    void OnUpdatedText(string seed)
    {
        seedNumber = 4;
        foreach(char c in seed)
        {
            seedNumber = seedNumber + c;
        }
        this.seed = seed;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
