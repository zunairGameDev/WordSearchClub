using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelController : MonoBehaviour
{
    public GameObject particleEffect; // Assign the particle effect GameObject
    public Slider winSlider; // Assign the slider
    public Button nextLevelButton; // Assign the next level button
    public RectTransform targetPosition; // Position near the main image
    public float moveDuration = 2f; // Time it takes for the particle to move
    public Canvas m_Canvas;

    [SerializeField] private enum RenderModeStates { camera, overlay, world };
    [SerializeField] private RenderModeStates m_RenderModeStates;

    private bool isMoving = false;
    private Vector2 initialPosition;

    private void OnEnable()
    {
        CameraModeChange(RenderModeStates.camera);
    }
    void Start()
    {
        // Initially deactivate slider and next level button
        winSlider.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        initialPosition = particleEffect.transform.position;
        OnLevelWin();
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
        nextLevelButton.gameObject.SetActive(true);
        CameraModeChange(RenderModeStates.overlay);

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

}
