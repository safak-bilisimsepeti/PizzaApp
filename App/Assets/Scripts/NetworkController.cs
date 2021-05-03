using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;

public class NetworkController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(FindAddress("DA17 5NR"));
    }

    // Update is called once per frame
    void Update()
    {
        ReadBuffer();
    }

    void ReadBuffer()
    {
        if (Network.buffer.Count > 0)
        {
            foreach (string json in Network.buffer)
            {
                Local.user = JsonUtility.FromJson<User>(json);
            }
        }
    }

    public IEnumerator FindAddress(string zipcode)
    {
        string uri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}",
            zipcode, APIKey.Geocoding);
        WWWForm form = new WWWForm();

        using (UnityWebRequest request = UnityWebRequest.Post(uri, form))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log("WebRequest Error");
            }
            else
            {
                string json = request.downloadHandler.text;
                dynamic data = JObject.Parse(json);
                string formatted_address = data.results[0].formatted_address;
                Local.user.address = formatted_address;
                Debug.Log(formatted_address);
            }
        }
    }
}


public static class Network
{
    public static string address = "127.0.0.1";
    public static int port = 25565;
    public static List<string> buffer = new List<string>();

    static TcpClient client;
    static NetworkStream stream;
    static StreamReader reader;
    static StreamWriter writer;
    static Thread thread;

    public static void Connect()
    {
        client = new TcpClient(address, port);
        stream = client.GetStream();
        reader = new StreamReader(stream);
        writer = new StreamWriter(stream);
        thread = new Thread(() => Listen());
        thread.Start();
    }

    static void Listen()
    {
        while (client.Connected)
        {
            string json = reader.ReadLine();
            buffer.Add(json);
        }
    }

    public static void Send(string json)
    {
        writer.WriteLine(json);
        writer.Flush();
    }
}

public static class HTTP
{
    public static string address = "";

    public static IEnumerator Post(WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(address, form);
        yield return www.SendWebRequest();
    }
}