using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    private UIDocument document;

    private Button button;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        button = document.rootVisualElement.Q("StartGame") as Button;
        button.RegisterCallback<ClickEvent>(StartGame);
    }

	private void OnDisable()
	{
		button.UnregisterCallback<ClickEvent>(StartGame);
	}

	private void StartGame(ClickEvent clickEvent)
    {
        Debug.Log("StartGame");
        SceneManager.LoadScene("ActionScene");
    }
}

