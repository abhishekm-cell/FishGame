using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button musicOnBtn;
    [SerializeField] private Button musicOffBtn;
    [SerializeField] private Button SFXOnBtn;
    [SerializeField] private Button SFXOffBtn; 

    [SerializeField] private GameObject musicIconOn,musicIconOff, SFXIconOn, SFXIconOff;

    void OnEnable()
    {
        ButtonListeners();
    }

    void OnDisable()
    {
        startButton.onClick.RemoveListener(StartButtonClicked);
        infoButton.onClick.RemoveListener(InfoButtonClicked);
        quitButton.onClick.RemoveListener(QuitButtonClicked);
        musicOnBtn.onClick.RemoveListener(MusicOn);
        musicOffBtn.onClick.RemoveListener(MusicOff);
        SFXOnBtn.onClick.RemoveListener(SFXOn);
        SFXOffBtn.onClick.RemoveListener(SFXOff);
    }

    private void ButtonListeners()
    {
        startButton.onClick.AddListener(StartButtonClicked);
        infoButton.onClick.AddListener(InfoButtonClicked);
        quitButton.onClick.AddListener(QuitButtonClicked);
        musicOnBtn.onClick.AddListener(MusicOn);
        musicOffBtn.onClick.AddListener(MusicOff);
        SFXOnBtn.onClick.AddListener(SFXOn);
        SFXOffBtn.onClick.AddListener(SFXOff);
    }
    
    void StartButtonClicked()//playButton
    {
        Debug.Log("Start Button Clicked");
        AudioManager.Instance.PlaySFX(SoundType.Button);
        Events.ResetGameRequest();
        Events.GameStartInvoke();
        AudioManager.Instance.PlayMusic(SoundType.BGM2);
        Events.ShowInGameInvoke();
    }

    void InfoButtonClicked()
    {
        Debug.Log("Info Button Clicked");
        Events.InfoPanelRequest();
        AudioManager.Instance.PlaySFX(SoundType.Button);
    }

    void QuitButtonClicked()
    {
        Application.Quit();
        Debug.Log("Quit Button Clicked");
    }

    private void MusicOn()
    {
        Debug.Log("Music on");
        AudioManager.Instance.SetMusic(false);
        musicIconOff.SetActive(true);
        musicIconOn.SetActive(false);
    }

    private void MusicOff()
    {
        Debug.Log("Music off");
        AudioManager.Instance.SetMusic(true);
        musicIconOn.SetActive(true);
        musicIconOff.SetActive(false);
    }
    private void SFXOn()
    {
        Debug.Log("SFX on");
        AudioManager.Instance.StopSfx();
        SFXIconOff.gameObject.SetActive(true);
        SFXIconOn.gameObject.SetActive(false);
    }

    private void SFXOff()
    {
        Debug.Log("SFX off");
        AudioManager.Instance.PlaySFX();
        SFXIconOn.gameObject.SetActive(true);
        SFXIconOff.gameObject.SetActive(false);
    }
    
    
    
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


}