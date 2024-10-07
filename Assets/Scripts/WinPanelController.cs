using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
using BBG.WordSearch;

public class WinPanelController : MonoBehaviour
{
    public GameObject treeImage;
    public GameObject particleEffect; // Assign the particle effect GameObject
    public Slider winSlider; // Assign the slider
    public Button collectButton;
    public Button nextLevelButton; // Assign the next level button
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
    public RectTransform jumpingReward;
    public RectTransform reachPoint;

    public GameObject countryStampPanel;

    [SerializeField] private enum RenderModeStates { camera, overlay, world };
    [SerializeField] private RenderModeStates m_RenderModeStates;

    private bool isMoving = false;
    private Vector2 initialPosition;

    public void ToShowData()
    {
        treeImage.transform.localScale = Vector3.one;
        sliderReward.SetActive(true);
        jumpingReward.gameObject.SetActive(false);
        CameraModeChange(RenderModeStates.camera);
        appreciationObject.SetActive(false);
        winSlider.gameObject.SetActive(false);
        winSlider.transform.localScale = Vector3.zero;
        nextLevelButton.gameObject.SetActive(false);
        nextLevelButton.transform.localScale = Vector3.zero;
        initialPosition = particleEffect.transform.position;
        ToShowCountryComplete();
        OnLevelWin();
    }
    public void ToShowCountryComplete()
    {
        if ((MainMenuText.Instance.currentValue + 1) == MainMenuText.Instance.countryInfo.maxValue)
        {
            appreciationText.text = "Country Complete";
            toShowCollectButton = true;

        }
        else
        {
            appreciationText.text = goodWord[Random.Range(0, goodWord.Count - 1) /*PlayerPrefs.GetInt("SelectJasonLevel")*/];
            appreciationShadowText.text = appreciationText.text;
            toShowCollectButton = false;

        }
        appreciationObject.SetActive(true);
    }

    public void OnLevelWin()
    {
        // Start particle effect
        particleEffect.SetActive(true);

        // Begin moving the particle to the target
        isMoving = true;
        StartCoroutine(MoveParticle());
    }

    IEnumerator MoveParticle()
    {
        float elapsedTime = 0f;
        Vector2 startPosition = particleEffect.transform.position;

        while (elapsedTime < moveDuration)
        {
            // Move the particle towards the target position
            particleEffect.transform.position = Vector3.Lerp(startPosition, targetPosition.position, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the particle reaches the target position
        particleEffect.transform.position = targetPosition.position;

        // Deactivate the particle effect
        particleEffect.SetActive(false);
        //CameraUI.SetActive(false);
        // Activate the slider and next level button
        winSlider.gameObject.SetActive(true);
        winSlider.transform.DOScale(1, 0.5f).SetEase(Ease.Linear);
        if (!toShowCollectButton)
        {
            nextLevelButton.gameObject.SetActive(true);
            nextLevelButton.transform.DOScale(1, 0.5f).SetEase(Ease.Linear);
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
    IEnumerator FillAmount()
    {
        float fillAmount = (float)MainMenuText.Instance.currentValue / MainMenuText.Instance.countryInfo.maxValue;
        winSlider.value = fillAmount;
        winSlideText.text = MainMenuText.Instance.currentValue.ToString() + " / " + MainMenuText.Instance.countryInfo.maxValue.ToString();
        yield return new WaitForSeconds(0.5F);
        PlayerPrefs.SetInt("CurrentValue", PlayerPrefs.GetInt("CurrentValue") + 1);
        MainMenuText.Instance.currentValue = PlayerPrefs.GetInt("CurrentValue");
        fillAmount = (float)MainMenuText.Instance.currentValue / MainMenuText.Instance.countryInfo.maxValue;
        winSlider.DOValue(fillAmount, 0.5f).SetEase(Ease.Linear);
        winSlideText.text = MainMenuText.Instance.currentValue.ToString() + " / " + MainMenuText.Instance.countryInfo.maxValue.ToString();
        MainMenuText.Instance.FillAmount();
        if (toShowCollectButton)
        {
            StartCoroutine(ScalingDownTree());
        }
    }
    IEnumerator ScalingDownTree()
    {
        jumpingReward.GetChild(0).localPosition = Vector3.zero;
        sliderReward.SetActive(false);
        jumpingReward.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        treeImage.transform.DOScale(Vector3.zero, 1.5f).SetEase(Ease.Linear);
        winSlider.transform.DOScale(Vector3.zero, 1.5f).SetEase(Ease.Linear);
        jumpingReward.GetChild(0).DOLocalMoveX(-120, 1f).SetEase(Ease.Linear);
        jumpingReward.DOAnchorPosX(-310, 1f).SetEase(Ease.OutQuad);
        jumpingReward.transform.DOScale(3.1f, 1f).SetEase(Ease.Linear);
        jumpingReward.transform.DOPunchPosition(Vector3.up * 150f, 1f, 1, 0);
        yield return new WaitForSeconds(1f);

        countryStampPanel.GetComponent<CountryCompletePanel>().ApplyingData();

    }

}
