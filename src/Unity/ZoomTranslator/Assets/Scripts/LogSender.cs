using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class LogSender
{
    const string domain = "18.183.180.160";
    string root_url = "http://" + domain + "/api/log/";
    string zoomid = null;
    string username = null;

    // Start is called before the first frame update
    public void Setup(string name, string id)
    {
        zoomid = id;
        username = name;
    }

    // Update is called once per frame
    void Update()
    {
    }

    string CreateURL(string zoomid)
    {
        return root_url + zoomid;
    }


    public IEnumerator SendLog(string[] logkeys, string[] logvalues, Action<bool,string> cb)
    {
        if (zoomid == null) cb(false, "");


        string url = CreateURL(zoomid);
        yield return SendLogRow(url, logkeys, logvalues, cb);
    }


    private List<IMultipartFormSection> CreateForm()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(string.Format("name={0}", username)));

        return formData;
    }

    private IEnumerator SendLogRow(string url, string[] logkeys,  string[] logvalues, Action<bool, string> cb)
    {
        var fd = CreateForm();

        for (int i = 0; i < logkeys.Length; ++i)
        {
            fd.Add(new MultipartFormDataSection(string.Format("{0}={1}", logkeys[i], logvalues[i])));
        }

        WWWForm form = new WWWForm();
        form.AddField("name", "koichi");
        for (int i = 0; i < logkeys.Length; ++i)
        {
            form.AddField(logkeys[i], logvalues[i]);
        }

        Debug.Log("url ; " + url);

        //UnityWebRequest uwr = UnityWebRequest.Post(url, fd);
        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        Debug.Log("1");
        yield return uwr.SendWebRequest();
        Debug.Log("2");
        if (uwr.isHttpError || uwr.isNetworkError)
        {
            Debug.Log(uwr.error);
            cb(false, uwr.error);
        }
        else
        {
            Debug.Log(uwr.downloadHandler.text);
            cb(true, uwr.downloadHandler.text);
        }


    }
}
