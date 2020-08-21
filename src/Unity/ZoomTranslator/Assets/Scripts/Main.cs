using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;

public class Main : MonoBehaviour
{
    [SerializeField]
    SpeechToText _stt;

    [SerializeField]
    Text _text;

    string sttMessage = "";
    object locker = new object();
    LogSender logSender = new LogSender();
    SynchronizationContext mainThreadContext;
    void Awake()
    {
        mainThreadContext = SynchronizationContext.Current;
    }

    // Start is called before the first frame update
    void Start()
    {
        logSender.Setup("koichi", "0123456789");

        StartCoroutine(StartDelay(1f));



    }

    // Update is called once per frame
    void Update()
    {
        lock (locker)
        {
            _text.text = sttMessage;
        }
    }
    IEnumerator StartDelay(float s)
    {
        Debug.Log("StartRecognition [0]");
        yield return new WaitForSeconds(s);
        Debug.Log("StartRecognition [1]");

        Task.Run(async () =>
        {
            _stt.onEndRecognition += onCompleteSTT;
            await _stt.StartRecognition();
            Debug.Log("end ");
        });
    }

    void onCompleteSTT(ResultReason r, string m)
    {
        Debug.Log("onCompleteSTT " + m);
        //_stt.onEndRecognition -= onSTT;
        lock (locker)
        {
            sttMessage += m + "\n";


            mainThreadContext.Post(__ =>
            {
                SendLog(m);
            }, null);

        }
    }

    void SendLog(string l)
    {
        Debug.Log("SendLog : " + l) ;

        TranslatePacker p = new TranslatePacker();
        p.Add(TranslateTarget.ja_JP, l);
        p.Add(TranslateTarget.en, "eigo");

        var d = p.GetPack();

        // コルーチン変えたい
        StartCoroutine(logSender.SendLog(d.Keys.ToArray(), d.Values.ToArray(), (t, m) =>
        {
            Debug.Log("return " + t + " " + m);

        }));
    }

}
