using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Linq;

public static class APIKey
{
    public static string Geocoding { get { return "AIzaSyBDj0K4TLkQOJTLmmBBwYXsJakxxlTZ1BY"; } }
}

public static class Language
{
    static Dictionary dictionary = new Dictionary();

    public static Country GetCurrentLanguage()
    {
        return dictionary.language;
    }

    public static bool Init(Country language = Country.UK)
    {
        TextAsset[] assets = Resources.LoadAll<TextAsset>("Languages");

        foreach (TextAsset asset in assets)
        {
            if (asset.name == language.ToString())
            {
                DictJson dict = JsonUtility.FromJson<DictJson>(asset.text);
                dictionary.language = dict.language;
                List<DictionaryKey> keys = new List<DictionaryKey>();
                foreach (string json in dict.keys)
                {
                    DictionaryKey key = JsonUtility.FromJson<DictionaryKey>(json);
                    keys.Add(key);
                }
                dictionary.keys = keys.ToArray();
                break;
            }
        }
        
        return dictionary.keys.Length > 0;
    }

    public static string Translate(string Key)
    {
        try
        {
            return (from key in dictionary.keys where key.key == Key select key).First().value;
        }
        catch
        {
            return Key;
        }
    }

    public static void TranslateAll()
    {
        Object[] texts = GameObject.FindObjectsOfType(typeof(UnityEngine.UI.Text));
        
        foreach (Object text in texts)
        {
            UnityEngine.UI.Text txt = text as UnityEngine.UI.Text;
            txt.text = Translate(txt.text);
        }
    }
}

public class Dictionary
{
    public Country language = Country.UK;
    public DictionaryKey[] keys = new DictionaryKey[0];
}

public class DictJson
{
    public Country language;
    public string[] keys;
}

public class DictionaryKey
{
    public string key = "";
    public string value = "";

    public DictionaryKey(string Key, string Value)
    {
        key = Key;
        value = Value;
    }
}

public static class Base64Utility
{
    public static Texture2D FromBase64(string base64)
    {
        byte[] flagData = System.Convert.FromBase64String(base64);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(flagData);
        return texture;
    }

    public static string ToBase64(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        return System.Convert.ToBase64String(bytes);
    }
}

public static class Local
{
    public static string version = "1.0.0";
    public static User user = new User();
    public static Team team_SoccerMP = null;
    public static object buffer = null;
}

public class User
{
    public string phone = "";
    public string password = "";
    public string name = "";
    public string surname = "";
    public string mail = "";
    public string address = "";
    public string zipcode = "";
    public Country country = Country.UK;
    public string registerDate = "";
    public float ratingScore = 1.5f;
    public int freeSlices = 12;
    public List<FreePizza> freePizzas = new List<FreePizza>();
    public List<GameData> gameData = new List<GameData>()
    {
        new GameData("Spin"),
        new GameData("Soccer"),
        new GameData("SoccerMP")
    };

    public int CalculateStar(float score)
    {
        if (score < 0.01)
        {
            return 0;
        }
        else if (score >= 0.01f && score <= 0.61f)
        {
            return 1;
        }
        else if (score > 0.61f && score <= 1.19f)
        {
            return 2;
        }
        else if (score > 1.19f && score <= 2.5f)
        {
            return 3;
        }
        else if (score > 2.5f && score <= 3.5f)
        {
            return 4;
        }
        else if (score > 3.5f)
        {
            return 5;
        }

        return 0;
    }
}

public enum Country
{
    UK,
    USA,
    DE,
    FR
}

public static class GameStats
{
    public static GameToken[,] spinTokens = new GameToken[,]
    {
        { new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 1) },
        { new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 2) },
        { new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 2), new GameToken(Difficulty.Easy, 2), new GameToken(Difficulty.Easy, 3) },
        { new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 2), new GameToken(Difficulty.Easy, 3) },
        { new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 2), new GameToken(Difficulty.Easy, 3), new GameToken(Difficulty.Easy, 4) },
        { new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 2), new GameToken(Difficulty.Easy, 2), new GameToken(Difficulty.Easy, 3), new GameToken(Difficulty.Easy, 4) }
    };

    public static GameToken[,] soccerTokens = new GameToken[,]
    {
        { new GameToken(Difficulty.Medium, 0), new GameToken(Difficulty.Medium, 1), new GameToken(Difficulty.Hard, 0), new GameToken(Difficulty.Hard, 0), new GameToken(Difficulty.Hard, 1), new GameToken(Difficulty.Hard, 2), new GameToken(Difficulty.Crazy, 0), new GameToken(Difficulty.Crazy, 1), new GameToken(Difficulty.Crazy, 2), new GameToken(Difficulty.Crazy, 3) },
        { new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Medium, 0), new GameToken(Difficulty.Medium, 1), new GameToken(Difficulty.Medium, 2), new GameToken(Difficulty.Hard, 0), new GameToken(Difficulty.Hard, 1), new GameToken(Difficulty.Hard, 2), new GameToken(Difficulty.Hard, 3), new GameToken(Difficulty.Crazy, 2), new GameToken(Difficulty.Crazy, 3) },
        { new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 2), new GameToken(Difficulty.Medium, 2), new GameToken(Difficulty.Hard, 0), new GameToken(Difficulty.Hard, 1), new GameToken(Difficulty.Hard, 2), new GameToken(Difficulty.Hard, 3), new GameToken(Difficulty.Crazy, 2), new GameToken(Difficulty.Crazy, 3) },
        { new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 2), new GameToken(Difficulty.Medium, 0), new GameToken(Difficulty.Medium, 1), new GameToken(Difficulty.Medium, 2), new GameToken(Difficulty.Medium, 3), new GameToken(Difficulty.Hard, 2), new GameToken(Difficulty.Hard, 3), new GameToken(Difficulty.Crazy, 3) },
        { new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 2), new GameToken(Difficulty.Easy, 3), new GameToken(Difficulty.Medium, 0), new GameToken(Difficulty.Medium, 1), new GameToken(Difficulty.Medium, 2), new GameToken(Difficulty.Medium, 3), new GameToken(Difficulty.Hard, 2), new GameToken(Difficulty.Hard, 3) },
        { new GameToken(Difficulty.Easy, 0), new GameToken(Difficulty.Easy, 1), new GameToken(Difficulty.Easy, 2), new GameToken(Difficulty.Easy, 3), new GameToken(Difficulty.Medium, 0), new GameToken(Difficulty.Medium, 1), new GameToken(Difficulty.Medium, 2), new GameToken(Difficulty.Medium, 3), new GameToken(Difficulty.Medium, 3), new GameToken(Difficulty.Hard, 3) }
    };

    public static int[,] soccerScores = new int[,]
    {
        { 0, 0, 0, 0 },
        { 16, 12, 8, 2 },
        { 40, 30, 20, 5 },
        { 80, 60, 40, 10 },
        { 120, 90, 60, 15 }
    };
}

public class GameData
{
    public string id;
    public List<GameToken> tokens;
    public string lastUpdate;
    public int points;
    public string team;

    public GameData(string GameId)
    {
        id = GameId;
        tokens = new List<GameToken>();
        lastUpdate = System.DateTime.Now.ToString();
        points = 0;
        team = null;
    }

    void SetTokens(GameToken[] GameTokens)
    {
        
    }

    public void GenerateTokens(GameToken[,] GameStat)
    {
        int star = Local.user.CalculateStar(Local.user.ratingScore);
        List<GameToken> newTokens = new List<GameToken>();
        for (int i = 0; i < GameStat.GetLength(1); i++)
        {
            newTokens.Add(GameStat[star, i]);
        }

        tokens.Clear();

        int count = newTokens.Count;
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, newTokens.Count);
            GameToken token = newTokens[index];
            tokens.Add(token);
            newTokens.RemoveAt(index);
        }
    }

    public GameToken UseGameToken(GameToken[,] GameStat, bool Force = false)
    {
        GameToken token = null;
        bool refresh = (System.DateTime.Now - System.DateTime.Parse(lastUpdate)).Hours >= 12 || Force;

        if (tokens.Count == 0 && refresh)
            GenerateTokens(GameStat);

        if (tokens.Count > 0)
        {
            GameToken t = tokens[0];
            token = new GameToken(t.difficulty, t.level);
            tokens.RemoveAt(0);
        }

        return token;
    }
}

public class GameToken
{
    public Difficulty difficulty = Difficulty.Easy;
    public int level = 0;

    public GameToken (Difficulty Difficulty, int Level)
    {
        difficulty = Difficulty;
        level = Level;
    }
}

public class Team
{
    public string name = "";
    public int logoIndex = 0;
    public TeamMember[] members = new TeamMember[0];

    public int GetTotalScore()
    {
        int score = 0;
        foreach(TeamMember member in members)
        {
            score += member.score;
        }

        return score;
    }

    public int GetTotalSlices()
    {
        int slices = 0;
        foreach (TeamMember member in members)
        {
            slices += member.slices;
        }

        return slices;
    }
}

public class TeamMember
{
    public string phone = "";
    public string name = "";
    public int score = 0;
    public int slices = 0;

    public TeamMember(string Phone, string Name)
    {
        phone = Phone;
        name = Name;
    }
}

public class FreePizza
{
    public string expirationDate = (System.DateTime.Now.AddMonths(3)).ToString();
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Crazy
}