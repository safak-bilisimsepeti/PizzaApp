using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoonSharp;
using MoonSharp.Interpreter;

public class Core : MonoBehaviour
{
    void Awake()
    {
        if (Language.Init())
        {
            Language.TranslateAll();
        }
        else
        {
            Debug.Log("LANG_INIT_ERR");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        Debug.Log(Language.Translate("#test"));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot(Random.Range(int.MinValue, int.MaxValue).ToString() + ".png", 1);
        }
    }

    public string TakeScreenshot()
    {
        Texture2D screen = ScreenCapture.CaptureScreenshotAsTexture();
        return Base64Utility.ToBase64(screen);
    }

    public void LoadUserData()
    {
        if (PlayerPrefs.HasKey("USER"))
        {
            string json = PlayerPrefs.GetString("USER");
            Local.user = JsonUtility.FromJson<User>(json);
        }
    }

    public void SaveUserData()
    {
        string json = JsonUtility.ToJson(Local.user);
        PlayerPrefs.SetString("USER", json);

    }

    public void Register(string phone, string password, string name, string surname, string address, string mail, Country country)
    {
        Local.user.name = name;
        Local.user.surname = surname;
        Local.user.address = address;
        Local.user.phone = phone;
        Local.user.password = password;
        Local.user.mail = mail;
        Local.user.country = country;

        Local.user.gameData[0].GenerateTokens(GameStats.spinTokens);
        Local.user.gameData[1].GenerateTokens(GameStats.soccerTokens);
        Local.user.gameData[2].GenerateTokens(GameStats.soccerTokens);

        WWWForm form = new WWWForm();
        form.AddField("name", Local.user.name);
        form.AddField("surname", Local.user.surname);
        form.AddField("address", Local.user.address);
        form.AddField("phone", Local.user.phone);
        form.AddField("password", Local.user.password);
        form.AddField("mail", Local.user.mail);
        form.AddField("country", (byte)Local.user.country);

        HTTP.Post(form);
    }

    public string GenerateSHA256()
    {
        string keys = "0123456789abcdef";
        string value = "";

        for (int i = 0; i < 64; i++)
        {
            value += keys[Random.Range(0, 16)];
        }

        return value;
    }

    public void CheckVersion()
    {
        
    }

    public void ClearContent(UnityEngine.UI.ScrollRect scrollrect)
    {
        while (true)
        {
            if (scrollrect.content.childCount > 0)
            {
                Destroy(scrollrect.content.GetChild(0).gameObject);
            }
            else
            {
                break;
            }
        }
    }

    void OnApplicationQuit()
    {
        SaveUserData();
        Debug.Log("EXIT");
    }
}
