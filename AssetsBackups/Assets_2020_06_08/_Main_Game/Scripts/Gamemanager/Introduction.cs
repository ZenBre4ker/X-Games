using UnityEngine;
public class Introduction : MonoBehaviour
{
    public GameObject IntroductionScreen;
    public GameObject NextScreen;

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        IntroductionScreen.SetActive(true);
    }
    public void onClickCloseIntroduction()
    {
        IntroductionScreen.SetActive(false);
        NextScreen.SetActive(true);
    }
}
