using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using Ink.Runtime; 
using System.Collections; 

public class IntroNarrator : MonoBehaviour
{
    [SerializeField] private TextAsset _inkJSON;
    [SerializeField] private string _sceneToLoad = "MainMenu";
    [SerializeField] private TextMeshProUGUI _narrativeText;
    [SerializeField] private GameObject _continuePrompt; 
    [SerializeField] private float _typingSpeed = 0.04f;
    private Story _story;
    private Coroutine _typingCoroutine;
    private bool _isTyping = false;

    void Start()
    {
        _story = new Story(_inkJSON.text);
        if (_continuePrompt != null) _continuePrompt.SetActive(false);
        DisplayNextLine();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DisplayNextLine();
        }
    }

    public void DisplayNextLine()
    {
        if (_isTyping)
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            _narrativeText.text = _story.currentText; 
            _isTyping = false;
            if (_continuePrompt != null) _continuePrompt.SetActive(true);
        }
        else if (_story.canContinue)
        {
            if (_continuePrompt != null) _continuePrompt.SetActive(false);
            string nextLine = _story.Continue();

            _typingCoroutine = StartCoroutine(TypewriterEffect(nextLine));
        }
        else 
        {
            SceneManager.LoadScene(_sceneToLoad);
        }
    }
    
    private IEnumerator TypewriterEffect(string line)
    {
        _isTyping = true;
        _narrativeText.text = ""; 

        foreach (char letter in line.ToCharArray())
        {
            _narrativeText.text += letter;
            yield return new WaitForSeconds(_typingSpeed);
        }

        _isTyping = false;
        if (_continuePrompt != null) _continuePrompt.SetActive(true); 
    }
}