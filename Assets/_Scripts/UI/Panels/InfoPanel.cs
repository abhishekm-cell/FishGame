using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private Button closeBtn;


    void Start()
    {
        
    }
    void OnEnable()
    {
        closeBtn.onClick.AddListener(OnCloseClick);
    }
    void OnDisable()
    {
        closeBtn.onClick.RemoveListener(OnCloseClick);
    }

    void OnCloseClick()
    {
        Hide();
        AudioManager.Instance.PlaySFX(SoundType.Button);
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