using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    private bool gamePaused = false;
    private int highScore;
    [SerializeField] private AudioMixerSnapshot unpaused;
    [SerializeField] private AudioMixerSnapshot paused;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private PlayerController player;
    [SerializeField] private TextMeshProUGUI highScoreUI;
    [SerializeField] private TextMeshProUGUI scoreUI;

    public bool won = false;

    // Start is called before the first frame update
    void Start()
    {
        unpaused.TransitionTo(1f);
        highScore = PlayerPrefs.GetInt("HighScore");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gamePaused = !gamePaused;
            
            if (gamePaused)
            {
                paused.TransitionTo(1f);
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                unpaused.TransitionTo(1f);
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Win()
    {
        if (player.score > highScore)
        {
            highScore = player.score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        won = true;

        highScoreUI.text = "HIGHSCORE: " + highScore;
        scoreUI.text = "SCORE: " + player.score;

        winMenu.SetActive(true);
    }
}
