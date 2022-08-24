using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Core core;
    [SerializeField] private LevelManager levelManager;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private Button buttonContinue;

    [SerializeField] private Text lifes;
    [SerializeField] private Text score;

    private void Start()
    {
        levelManager.ChangeLifesHandlerEvent += ChangeUILifes;
        levelManager.ChangeScoreCountHandlerEvent += ChangeUIScore;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && levelManager.IsGame)
        {
            ToggleMenu();
        }
    }
    public void ToggleControllerKeyboard()
    {
        core.ChangePlayerControl(PlayerControl.Keyboard);
    }

    public void ToggleControllerMouseAndKeyboard()
    {
        core.ChangePlayerControl(PlayerControl.MouseAndKeyboard);
    }

    public void HideButton(GameObject go)
    {
        go.SetActive(false);
    }

    public void ShowButton(GameObject go)
    {
        go.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ShowGameOver()
    {
        gameOver.SetActive(true);
    }

    public void HideGameOver()
    {
        gameOver.SetActive(false);
    }

    public void ToggleMenu()
    {
        if (levelManager.IsGame == false)
        {
            buttonContinue.GetComponent<Button>().interactable = false;
        }
        else 
        {
            buttonContinue.GetComponent<Button>().interactable = true;
        }

        if (pauseMenu.activeInHierarchy)
        {
            core.PauseGame(false);
            pauseMenu.SetActive(false);
            return;
        }
        core.PauseGame(true);
        pauseMenu.SetActive(true);
    }

    private void ChangeUILifes(int count)
    {
        if (count < 0)
        {
            ShowGameOver();
            return;
        }

        SetText(lifes, count.ToString());
    }

    private void ChangeUIScore(int count)
    {
        SetText(score, count.ToString());
    }

    private void SetText(Text text, string str)
    {
        text.text = str;
    }
}

