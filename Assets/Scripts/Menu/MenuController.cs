using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
public class MenuController : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] GameObject MainMenuContainer;
    [SerializeField] GameObject OptionMenuContainer;
    [SerializeField] GameObject CostumPopoutDialogContainer;
    [SerializeField] GameObject NewGameDialog;
    [SerializeField] GameObject SettingsSound;
    [SerializeField] GameObject SettingsGameplay;
    [SerializeField] GameObject SettingsGraphic;
    [SerializeField] GameObject Credits;

    [Header("Volume setting")]
    [SerializeField] Slider VolumeSlider;
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 0.5f;
    
    [FormerlySerializedAs("conformationPrompt")] [SerializeField] private GameObject conformationPrompt = null;
 
    [Header("Levels to load")] public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSaveGameDialog = null;
    
    [Header ("Audio")]
    [SerializeField] AudioSource MenuBaseLineAudioSource;

    private void Start()
    {
        MainMenuContainer.SetActive(true);
        OptionMenuContainer.SetActive(false);
        CostumPopoutDialogContainer.SetActive(true);

        NewGameDialog.SetActive(false);
        SettingsSound.SetActive(false);
        SettingsGameplay.SetActive(false);
        SettingsGraphic.SetActive(false);
        Credits.SetActive(false);
        
        VolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");
        if(VolumeSlider.value == 0)
        {
            VolumeSlider.value = 1;
            PlayerPrefs.SetFloat("masterVolume", 1);
        }
        AudioListener.volume = VolumeSlider.value;
        amount = PlayerPrefs.GetInt("WinConPoints") + PlayerPrefs.GetInt("WinConHex") + PlayerPrefs.GetInt("WinConMissions");
        ManageHextileAmount(ProgressLv2, 3, Level2Image);
        ManageHextileAmount(ProgressLv3, 6, Level3Image);
        MenuBaseLineAudioSource.volume = 1;
    }
    public void NewGameDialogYes() => SceneManager.LoadScene(_newGameLevel);
    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene((levelToLoad));
        }
        else noSaveGameDialog.SetActive(true);
    }
    public void ExitButton() => Application.Quit();
    #region LevelAvailability
    [Header("Choose Level")] [SerializeField]
    TMP_Text ProgressLv2;
    [SerializeField] TMP_Text ProgressLv3;
    [SerializeField] Image Level2Image;
    [SerializeField] Image Level3Image;
    private int amount;
    void ManageHextileAmount(TMPro.TMP_Text lv, int goalAmount, Image levelimage)
    {
        if (amount <= goalAmount - 1) lv.text = amount + "/" + goalAmount;
        else if (amount >= goalAmount)
        {
            levelimage.color = Color.white;
            //Color tempColor = levelimage.color;
            // tempColor.a = 1f;
            // lv.text = "Not available yet";
            lv.gameObject.SetActive(false);
        }
    }

    [SerializeField] TMPro.TMP_Text Message;
    public void Level2AvailabilityMessage()
    {
        Message.gameObject.SetActive(true);
        if (amount <= 2) Message.text = "Collect Hextiles to unlock this Level";
        else Message.text = "Currently only Level 1 is available";
    }
    public void Level3AvailabilityMessage()
    {
        Message.gameObject.SetActive(true);
        if (amount <= 6) Message.text = "Collect Hextiles to unlock this Level";
        else Message.text = "Currently only Level 1 is available";
    }
    #endregion
    #region volume
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
        AudioListener.volume = VolumeSlider.value;
    }
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConformationBox());
    }
    public void ResetButton(string MenuType)
    {
        if(MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }
    }
    public IEnumerator ConformationBox()
    {
        conformationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        conformationPrompt.SetActive(false);
    }
    #endregion
}