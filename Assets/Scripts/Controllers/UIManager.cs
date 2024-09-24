using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region editor references
    [SerializeField] private LevelCompleteScreen levelCompleteScreen;
    [SerializeField] private MainMenuScreen mainMenuScreen;
    [SerializeField] private GameplayScreen gameplayScreen;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private float rotationDuration = 1f; // Duration of the rotation animation
    [SerializeField] private float scaleDuration = 0.5f; // Duration of the scaling animations
    [SerializeField] private Vector3 initialScale = new Vector3(0.8f, 0.8f, 0.8f); // Target scale when shrinking
    [SerializeField] private Vector3 finalScale = Vector3.one; // Target scale after rotations complete 
    private Vector3 currentRotation = Vector3.zero; // To keep track of the current rotation

    #endregion

    private void Start()
    {
        HideLevelCompleteScreen();
        ShowMainMenuScreen();
        AssignOnClickListeners();
        ObserveEvents();
    }
    public GridLayoutGroup GetGridLayoutGroup()
    {
        return gridLayoutGroup;
    }
    public Transform GetGridTransform()
    {
        return gridLayoutGroup.GetComponent<Transform>();
    }
    public RectTransform GetGridRectTransform()
    {
        return gridLayoutGroup.GetComponent<RectTransform>();
    }
    public void SetStartButtonText(string startButtonText)
    {
        mainMenuScreen.SetStartButtonText(startButtonText);
    }
    private void ObserveEvents()
    {
        GameManager.Instance.onLevelCompleted += ShowLevelCompleteScreen;
    }
    private void AssignOnClickListeners()
    {
        mainMenuScreen.GetStartButton().onClick.RemoveAllListeners();
        mainMenuScreen.GetStartButton().onClick.AddListener(
            () =>
            {
                GameManager.Instance.StartGame();
                mainMenuScreen.Hide();
                gameplayScreen.Show();
            }
            );

        levelCompleteScreen.GetNextLevelButton().onClick.RemoveAllListeners();
        levelCompleteScreen.GetNextLevelButton().onClick.AddListener(
            () =>
            {
                GameManager.Instance.ReturnToMainMenu();
            }
            );
    }
    private void ShowMainMenuScreen()
    {
        mainMenuScreen.Show();
    }
    private void HideMainMenuScreen()
    {
        mainMenuScreen.Hide();
    }
    private void ShowLevelCompleteScreen(int levelNumber)
    {
        levelCompleteScreen.Show();
    }
    private void HideLevelCompleteScreen()
    {
        levelCompleteScreen.Hide();
    }
    public void GridScaleRotateAndScale()
    {
        Transform gridRotation = GetGridTransform();
        // Check the current rotation of the grid (parent object)
        float currentYRotation = Mathf.Round(gridRotation.localEulerAngles.z);

        // Determine the next rotation based on the current rotation state
        float rotationAngle = (currentYRotation == 0f) ? -180f : 0f;

        // Calculate the new target rotation for the parent and its children
        currentRotation = new Vector3(0, 0, rotationAngle); // Update current rotation
        // Create a sequence to chain animations
        Sequence sequence = DOTween.Sequence();

        // Step 1: Scale the parent down to 0.8
        sequence.Append(gridRotation.DOScale(initialScale, scaleDuration)
                            .SetEase(Ease.OutQuad)); // Optional: Set easing for the scaling

        // Step 2: Simultaneously rotate parent and children
        sequence.AppendCallback(() =>
        {
            // Rotate parent locally
            gridRotation.DOLocalRotate(currentRotation, rotationDuration, RotateMode.FastBeyond360)
                     .SetEase(Ease.InOutQuad); // Set the easing for rotation

            // Rotate each child locally
            foreach (Transform child in gridRotation)
            {
                if (child.gameObject.CompareTag("LineRender"))
                {
                    // Skip the current child if it has the tag "LineRender"
                    continue;
                }
                child.DOLocalRotate(currentRotation, rotationDuration, RotateMode.FastBeyond360)
                     .SetEase(Ease.InOutQuad); // Set the easing for rotation
            }
        });

        // Step 3: Scale the parent back to 1 after rotation completes
        sequence.Append(gridRotation.DOScale(finalScale, scaleDuration)
                            .SetEase(Ease.OutBounce)); // Optional: Set easing for scaling back

        // Start the sequence
        sequence.Play();
    }
}
