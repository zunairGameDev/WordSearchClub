using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
using BBG.WordSearch;
using FunGames.Mediation;


public class WinPanelController : MonoBehaviour
{
    public static WinPanelController Instance;
    public GameObject treeImage;
    public Image countryLogo;
    //public GameObject particleEffect; // Assign the particle effect GameObject
    public Slider winSlider; // Assign the slider
    public Button collectButton;
    public Button nextLevelButton; // Assign the next level button
    public Button dailyChallengeButton; // Assign the next level button
    public RectTransform targetPosition; // Position near the main image
    public float moveDuration = 2f; // Time it takes for the particle to move
    public Canvas m_Canvas;
    public TextMeshProUGUI wisdomPoint;
    public TextMeshProUGUI winSlideText;
    public List<string> goodWord;
    public GameObject appreciationObject;
    public TextMeshProUGUI appreciationText;
    public TextMeshProUGUI appreciationShadowText;
    public bool toShowCollectButton;
    public GameObject sliderReward;
    public GameObject ticketPanel;
    public GameObject NewDestinationPanel;
    public RectTransform reachPoint;
    public Image backGround;
    public GameObject light;
    //public GameObject countryStampPanel;

    [SerializeField] private enum RenderModeStates { camera, overlay, world };
    [SerializeField] private RenderModeStates m_RenderModeStates;

    private bool isMoving = false;
    private Vector2 initialPosition;
    private void Awake()
    {
        Instance = this;
    }
    public void ToShowData()
    {
        treeImage.transform.localScale = Vector3.one;
        sliderReward.SetActive(true);
        ticketPanel.gameObject.SetActive(false);
        CameraModeChange(RenderModeStates.camera);
        appreciationObject.SetActive(false);
        winSlider.gameObject.SetActive(false);
        winSlider.transform.localScale = Vector3.zero;
        nextLevelButton.gameObject.SetActive(false);
        nextLevelButton.transform.localScale = Vector3.zero;
        dailyChallengeButton.gameObject.SetActive(false);
        dailyChallengeButton.transform.localScale = Vector3.zero;
        NewDestinationPanel.SetActive(false);
        //initialPosition = particleEffect.transform.position;
        wisdomPoint.text = PlayerPrefs.GetInt("WisdomPoints", 0).ToString();
        ToShowCountryComplete();
        OnLevelWin();
    }
    public void ToShowCountryComplete()
    {
        //if ()
        //{
            if ((MainMenuText.Instance.currentValue + 1) == MainMenuText.Instance.countryInfo.maxValue && !GameManager.Instance.toPlayDailyChallange)
            {
                appreciationText.text = "Country Complete";
                appreciationShadowText.text = appreciationText.text;
                toShowCollectButton = true;

            }
            else
            {
                appreciationText.text = goodWord[Random.Range(0, goodWord.Count - 1) /*PlayerPrefs.GetInt("SelectJasonLevel")*/];
                appreciationShadowText.text = appreciationText.text;
                toShowCollectButton = false;

            }
            appreciationObject.SetActive(true);

        //}

    }

    public void OnLevelWin()
    {
        
        // Start particle effect
        //particleEffect.SetActive(true);
        backGround.sprite = MainMenuText.Instance.countryInfo.BackGroundImage;
        // Begin moving the particle to the target
        isMoving = true;
        StartCoroutine(MoveParticle());
    }



    IEnumerator MoveParticle()
    {
        float elapsedTime = 0f;
        //Vector2 startPosition = particleEffect.transform.position;
        StartCoroutine(IncreasingWisedomPoint());
        while (elapsedTime < moveDuration)
        {
            // Move the particle towards the target position
            //particleEffect.transform.position = Vector3.Lerp(startPosition, targetPosition.position, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //particleEffect.transform.DOMove(targetPosition.position,1f);
        // Ensure the particle reaches the target position
        //particleEffect.transform.position = targetPosition.position;


        //// Deactivate the particle effect
        //particleEffect.SetActive(false);
        //CameraUI.SetActive(false);
        // Activate the slider and next level button
        countryLogo.sprite = MainMenuText.Instance.countryInfo.countryLogo;
        winSlider.gameObject.SetActive(true);
        winSlider.transform.DOScale(1, 0.5f).SetEase(Ease.Linear);
        if (!toShowCollectButton)
        {
            if (GameManager.Instance.toPlayDailyChallange)
            {
                dailyChallengeButton.gameObject.SetActive(true);
                dailyChallengeButton.transform.DOScale(1, 0.5f).SetEase(Ease.Linear);
            }
            else
            {
                nextLevelButton.gameObject.SetActive(true);
                nextLevelButton.transform.DOScale(1, 0.5f).SetEase(Ease.Linear);
            }
        }

        CameraModeChange(RenderModeStates.overlay);
        StartCoroutine(FillAmount());

    }
    private void CameraModeChange(RenderModeStates m_RenderModeStates)
    {
        switch (m_RenderModeStates)
        {
            case RenderModeStates.camera:
                m_Canvas.renderMode = RenderMode.ScreenSpaceCamera;
                m_Canvas.worldCamera = Camera.main;
                break;

            case RenderModeStates.overlay:
                m_Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                break;
            case RenderModeStates.world:
                m_Canvas.renderMode = RenderMode.WorldSpace;
                break;
        }
    }
    IEnumerator IncreasingWisedomPoint()
    {

        yield return new WaitForSeconds(0.5f);

        float currentValue = PlayerPrefs.GetInt("WisdomPoints", 0);
        float duration = 1;
        float endValue = PlayerPrefs.GetInt("WisdomPoints", 0) + GameManager.Instance.ActiveBoard.words.Count;
        float increment = (endValue - currentValue) / duration;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            currentValue += increment * Time.deltaTime;
            wisdomPoint.text = Mathf.FloorToInt(currentValue).ToString();
            yield return null;
        }

        // Ensure the final value is set
        currentValue = endValue;
        PlayerPrefs.SetInt("WisdomPoints", (int)endValue);
        wisdomPoint.text = Mathf.FloorToInt(currentValue).ToString();
    }
    IEnumerator FillAmount()
    {
        float fillAmount = (float)MainMenuText.Instance.currentValue / MainMenuText.Instance.countryInfo.maxValue;
        winSlider.value = fillAmount;
        winSlideText.text = MainMenuText.Instance.currentValue.ToString() + " / " + MainMenuText.Instance.countryInfo.maxValue.ToString();
        yield return new WaitForSeconds(0.5F);
        if (!GameManager.Instance.toPlayDailyChallange)
        {
            PlayerPrefs.SetInt("CurrentValue", PlayerPrefs.GetInt("CurrentValue") + 1);
            MainMenuText.Instance.currentValue = PlayerPrefs.GetInt("CurrentValue");
            fillAmount = (float)MainMenuText.Instance.currentValue / MainMenuText.Instance.countryInfo.maxValue;
            //winSlider.DOValue(fillAmount, 0.5f).SetEase(Ease.Linear);
            winSlider.value = fillAmount;
            winSlideText.text = MainMenuText.Instance.currentValue.ToString() + " / " + MainMenuText.Instance.countryInfo.maxValue.ToString();
            MainMenuText.Instance.FillAmount();
            StartCoroutine(StartAdPanel());
            if (toShowCollectButton)
            {
                StartCoroutine(ScalingDownTree());
            }
        }
        else
        {
            StartCoroutine(StartAdPanel());
        }

    }
    IEnumerator StartAdPanel()
    {
        yield return new WaitForSeconds(0.1f);
        if (PlayerPrefs.GetInt("SelectJasonLevel") >= 2)
        {
            //Applovin_Manager.instance.ShowInterstitial();
            //FGMediation.ShowInterstitial("MyInterstitialAd");
            FGMediation.ShowInterstitial();
        }

    }
    IEnumerator ScalingDownTree()
    {


        //yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(1f);
        ticketPanel.GetComponent<CountryCompletePanel>().ApplyingData();
        //ticketPanel.GetComponent<CountryCompletePanel>().nextButton = nextLevelButton;
        ticketPanel.SetActive(true); sliderReward.SetActive(false);
        treeImage.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.Linear);
        winSlider.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.Linear);

    }

}
