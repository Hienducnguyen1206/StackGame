using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{    
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI HighScoreText;
    public Button reStartBtn;
    public Button MenuBtn;
    public  bool OpenMenu = false;
    public Image Menu;

    private void Awake()
    {
        UIManager gameobject = FindAnyObjectByType<UIManager>();
        MenuBtn.onClick.AddListener(RotateBtn);


        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
       
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
            
    }

    public void UpdateScore(int Score)
    {
        ScoreText.text = Score.ToString();
    }

    public void UpdateHighestScore(int HighScore) { 
        HighScoreText.text = HighScore.ToString();
    }


    public void OpenControlMenu()
    {   
        Sequence sequence = DOTween.Sequence();

        
        sequence.Append(Menu.transform.DOScaleY(1, 0.25f).SetEase(Ease.InOutSine));

        
        sequence.Append(Menu.transform.DOScaleX(1, 0.25f).SetEase(Ease.InOutSine));
    }

    public void CloseControlMenu()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(Menu.transform.DOScaleX(0.1f, 0.25f).SetEase(Ease.InOutSine));
        sequence.Append(Menu.transform.DOScaleY(0, 0.25f).SetEase(Ease.InOutSine));


       
    }

    public void RotateBtn()
    {   if (!OpenMenu)
        {
            OpenMenu = true;
            MenuBtn.transform.DORotate(new Vector3(0, 0, 180), 0.25f, RotateMode.FastBeyond360)
        .SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            OpenControlMenu();
           
        });
        }
        else {
           
            MenuBtn.transform.DORotate(new Vector3(0, 0, 0), 0.25f, RotateMode.FastBeyond360)
          .SetEase(Ease.InOutSine).OnComplete(() =>
          {
              CloseControlMenu();
              OpenMenu = false;
          });
        
        }
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // In the build, quit the application
        Application.Quit();
    #endif
    }
}
