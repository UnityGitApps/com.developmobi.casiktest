using System.Threading.Tasks;
using UnityEngine;

using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;
using Unity.Services.Core;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static UniWebView View { get; set; }

    public struct AppAttributes { }
    public struct UserAttributes { }

    private async Task Awake()
    {
        CacheComponents();

        await InitializeRemoteConfigAsync();

        var app = new AppAttributes() { };
        var user = new UserAttributes() { };

        await RemoteConfigService.Instance.FetchConfigsAsync(user, app);
        var version = (string)RemoteConfigService.Instance.appConfig.config.First.First;

        if(!version.Contains("//"))
        {
            Screen.fullScreen = true;
            SceneManager.LoadScene(1);
            return;
        }

        View.Load(version);
    }

    private async Task InitializeRemoteConfigAsync()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void CacheComponents()
    {
        View = gameObject.AddComponent<UniWebView>();
        View.ReferenceRectTransform = GameObject.Find("rect").GetComponent<RectTransform>();

        var safeArea = Screen.safeArea;
        var anchorMin = safeArea.position;
        var anchorMax = anchorMin + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        View.ReferenceRectTransform.anchorMin = anchorMin;
        View.ReferenceRectTransform.anchorMax = anchorMax;

        View.SetShowSpinnerWhileLoading(true);
        View.BackgroundColor = Color.black;

        View.OnOrientationChanged += (v, o) =>
        {
            Screen.fullScreen = o == ScreenOrientation.Landscape;

            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            v.ReferenceRectTransform.anchorMin = anchorMin;
            v.ReferenceRectTransform.anchorMax = anchorMax;

            View.UpdateFrame();
        };

        View.OnShouldClose += (v) =>
        {
            return false;
        };

        View.OnPageStarted += (browser, url) =>
        {
            Screen.fullScreen = false;
            foreach (Transform t in View.ReferenceRectTransform)
            {
                Destroy(t.gameObject);
            }

            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            View.ReferenceRectTransform.anchorMin = anchorMin;
            View.ReferenceRectTransform.anchorMax = anchorMax;

            View.Show();
            View.UpdateFrame();
        };
    }
}
