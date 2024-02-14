using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using UnityEngine.Android;

public class CamPermission : MonoBehaviour
{
//#if UNITY_IOS
//    [DllImport("__Internal")]
//    private static extern void _CamPermission();
//#endif

    public string nextScene;

    TextAsset activityAsset;
    TextAsset loadingAsset;
    string activity;
    string loading;

    void Awake()
    {
        if (PlayerPrefs.HasKey("HasLoaded"))
        {
            PlayerPrefs.DeleteKey("HasLoaded");
        }
    }

    void Start()
    {
        Go();
    }

    public void Go()
    {
#if UNITY_EDITOR
        ChangeScene();
#endif

#if UNITY_ANDROID
        StartCoroutine(CheckForCameraPermission());
#endif

#if UNITY_IOS
		//_CamPermission();
#endif
    }
    
    IEnumerator CheckForCameraPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }

        yield return new WaitForSeconds(1);

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            //PlayerPrefsX.SetBool("AllowCamera", false);
        }
        else
        {
            //PlayerPrefsX.SetBool("AllowCamera", true);
        }

        ChangeScene();
    }
    
    public void SetPref(string permission)
    {
        if (permission == "true")
        {
            //PlayerPrefsX.SetBool("AllowCamera", true);
        }
        else
        {
            //PlayerPrefsX.SetBool("AllowCamera", false);
        }
    }
    
    public void ChangeScene()
         {
             SceneManager.LoadSceneAsync(nextScene);
         }
}