using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Buttons : MonoBehaviour
{
    public GameObject ObjectButton;
    public GameObject Object;
    private Animator anim;

    public AudioMixer mixer;
    const string MIXER_MUSIC = "MusicVolume";
    const string MIXER_SFX = "SFXVolume";
    public float musicVolume = 50f;
    public float sfxVolume = 50f;

    void Start()
    {
        anim = GetComponent<Animator>();
        mixer.SetFloat(MIXER_MUSIC, musicVolume);
        mixer.SetFloat(MIXER_SFX, sfxVolume);
    }

    #region Buttons
    private void OnMouseUpAsButton() //Function that turns 3D objects into a buttons
    {
        anim = Object.GetComponent<Animator>();

        #region Main Menu Buttons
        if (ObjectButton == GameObject.Find("PlayButton")) //If PlayButton is clicked
        {
            PlayButton();
        }

        if (ObjectButton == GameObject.Find("QuitButton")) //If QuitButton is clicked
        {
            QuitButton();
        }

        if (ObjectButton == GameObject.Find("OptionsButton")) //If OptionsButton is clicked
        {
            OptionsButton();
        }
        #endregion

        #region Options Menu Buttons
        if (ObjectButton == GameObject.Find("VolumeDownButton"))
        {
            VolumeDownButton();
        }

        if (ObjectButton == GameObject.Find("VolumeUpButton"))
        {
            VolumeUpButton();
        }

        if (ObjectButton == GameObject.Find("BackToMainMenuButton")) //If BackButton is clicked
        {
            BackToMainMenuButton();
        }
        #endregion
    }
    #endregion

    #region ButtonsFunctions
    public void PlayButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitButton()
    {
        Debug.Log("Quitted");
        Application.Quit();
    }

    public void OptionsButton()
    {
        anim.Play("OptionsMenuShow");
    }

    public void BackToMainMenuButton()
    {
        anim.Play("MainMenuShow");
    }

    public void VolumeDownButton()
    {
        // Music Volume -5 on click
        //mixer.SetFloat("MusicVolume", -5f);

    }

    public void VolumeUpButton()
    {
        // Music Volume +5 on click
    }
    #endregion
}
