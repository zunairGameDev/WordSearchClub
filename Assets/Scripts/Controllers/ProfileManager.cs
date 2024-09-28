using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProfileManager : MonoBehaviour
{
    #region singleton
    public static ProfileManager profile_Instance;
    #endregion

    public Image profileImage;  // UI Image component for displaying the profile picture
    public Image[] spriteImages;  // UI Images for displaying available sprites in the UI
    public Sprite[] availableSprites;  // Array of available sprites for profile pictures
    private Sprite currentProfileSprite;  // Track current profile sprite
    public InputField playerNameInputField;

    private void Awake()
    {
        profile_Instance = this;
    }

    private void OnEnable()
    {
        playerNameInputField.text = PlayerPrefs.GetString("PlayerName", "Evening, Guest_16dan1!");
        profileImage.sprite = availableSprites[PlayerPrefs.GetInt("SpriteIndex")];
    }
    void Start()
    {
        // Initially set the profile picture
        currentProfileSprite = profileImage.sprite;
        // Initialize sprite images with available sprites
        for (int i = 0; i < availableSprites.Length; i++)
        {
            spriteImages[i].sprite = availableSprites[i];
        }
    }
    // This function will be called when a sprite is clicked
    public void OnSpriteClick(int spriteIndex)
    {
        // Get the clicked sprite
        Sprite clickedSprite = availableSprites[spriteIndex];
        // Swap current profile sprite with clicked sprite

        currentProfileSprite = clickedSprite;
        profileImage.sprite = clickedSprite;
        // Update the sprite in the array and in the UI Image at the clicked index
        //availableSprites[spriteIndex] = temp;
        //spriteImages[spriteIndex].sprite = temp;
        PlayerPrefs.SetInt("SpriteIndex", spriteIndex);
    }
}
