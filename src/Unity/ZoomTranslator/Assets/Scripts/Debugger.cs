#pragma warning disable 0162
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Debugger : Singleton<Debugger>
{
    const bool WRITE_LOG_FILE = false;
    const bool WRITE_TO_CANVAS = true;

    [SerializeField]
    GameObject _logDisplay;

    [SerializeField]
    Text _logText;

    [SerializeField]
    GameObject _windowDisplay;

    List<LogInfo> logList;

    RectTransform parentRect;

    public void Init()
    {
        logList = new List<LogInfo>();

        if (WRITE_TO_CANVAS)
        {
            Application.logMessageReceived += Instance.WriteToCanvasLog;
        }

        if (WRITE_LOG_FILE)
        {
            Instance.DebugFileInitialize();
            Application.logMessageReceived += Instance.WriteToLogFile;
        }

        parentRect = _logText.transform.parent.GetComponent<RectTransform>();

    }

    void Start() //Awakeだった
    {
        Init();
        Show(false);
        //_windowDisplay.SetActive(true);
    }
    private void Update()
    {
    }


    public void OnClickButton()
    {
        //if (false && Debug.isDebugBuild)
        {
            ShowTrigger();
        }
    }

    private void ShowTrigger()
    {
        if (_logDisplay != null)
        {
            _logDisplay.SetActive(!_logDisplay.activeInHierarchy);
        }
    }
    public void Show(bool show)
    {
        //_logDisplay.SetActive(show);
    }


    /******
     *　デバッグ表示
     */
    private void WriteToCanvasLog(string message, string stackTrace, LogType type)
    {
        logList.Add(new LogInfo(message, type));
        UpdateLog();
    }
    private void UpdateLog()
    {
        int ccc = 0;
        bool calc = true;
        while (calc)
        {

            string log = "";
            foreach (LogInfo info in logList)
            {
                switch (info.logType)
                {
                    case LogType.Error: { log += "<color=red>"; } break;
                }

                log += info.message;

                switch (info.logType)
                {
                    case LogType.Error: { log += "</color>"; } break;
                }

                log += "\n";
            }
            if (log.Length > 0 )  log = log.Substring(0, log.Length - 1); // 最後の改行いらない。
            _logText.text = log;

            //Debug.Log("p " + _text.preferredHeight);

            if (parentRect.rect.height < _logText.preferredHeight + 20 && logList.Count > 0 )
            {
                logList.RemoveAt(0);
            }
            else
            {
                calc = false;
            }
            if (ccc++ > 100)
            {
                break;
            }
        }

    }


    /******
     * デバッグファイル吐き出しシステム
     */
    private string DebugLogFileFormat = "yyyyMMddHHmmss";
    private string DebugLogFilePath;
    private void DebugFileInitialize()
    {
#if true
        string LogHeader = "Log_";
        DebugLogFilePath = LogHeader + DateTime.Now.ToString(DebugLogFileFormat) + ".txt";
        DebugLogFilePath = Application.persistentDataPath + "/" + DebugLogFilePath;

        int maxFiles = 9;
        {
            DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] fileInfo = info.GetFiles();
            List<FileInfo> fileList = new List<FileInfo>();

            //long tmp = 99999999999999;
            foreach (FileInfo file in fileInfo)
            {
                Debug.Log("file list : " + file.FullName);
                if (file.Name.StartsWith(LogHeader))
                {
                    fileList.Add(file);
                }
            }
            Comparison<FileInfo> func = new Comparison<FileInfo>((a, b) =>
            {
                string f0 = a.Name.Replace(LogHeader, "").Replace(".txt", "");
                string f1 = b.Name.Replace(LogHeader, "").Replace(".txt", "");
                long l0 = long.Parse(f0);
                long l1 = long.Parse(f1);
                if (l0 < l1) return -1;
                if (l0 > l1) return 1;
                return 0;
            }
            );
            fileList.Sort(func);
            int del = fileList.Count - maxFiles;
            del = Math.Max(0, del);
            for (int i = 0; i < del; i++)
            {
                File.Delete(fileList[i].FullName);
            }

        }


        FileInfo fi = null;
        StreamWriter sw = null;
        try
        {
            fi = new FileInfo(DebugLogFilePath);
            sw = fi.CreateText();
            sw.Flush();
            sw.Close();

            // iCloudの保存対象から外す
#if UNITY_IOS
            UnityEngine.iOS.Device.SetNoBackupFlag(DebugLogFilePath);
#endif

        }
        catch (Exception e)
        {
            Debug.LogError("Debug File Write Error " + e.ToString());
        }
#endif
    }
    private void WriteToLogFile(string message, string stackTrace, LogType type)
    {
#if true
        FileInfo fi = null;
        StreamWriter sw = null;
        try
        {
            fi = new FileInfo(DebugLogFilePath);

            //write                    
            sw = fi.AppendText();
            if (type == LogType.Log)
            {
                sw.WriteLine(message);
            }
            else
            {
                sw.WriteLine(message);
                sw.WriteLine("Previous Logs StackTrace:" + stackTrace);
            }
            sw.Flush();
            sw.Close();
            sw = null;

            // iCloudの保存対象から外す
#if UNITY_IOS
            UnityEngine.iOS.Device.SetNoBackupFlag(DebugLogFilePath);
#endif

        }
        catch (Exception )
        {

        }
#endif
    }


    class LogInfo
    {
        public string message;
        public LogType logType;
        public LogInfo(string m, LogType t)
        {
            message = m;
            logType = t;
        }
    }


    /****************************************************************
     * エラー表示
     * **********************/
#if false
DebugMessageWindowController debugwindow = null;
    public void CreateDebugWindow(string title, string text)
    {
        if (debugwindow == null)
        {
            var g = Instantiate(Resources.Load("DebugMessageWindow")) as GameObject;
            g.transform.SetParent(_windowDisplay.transform);
            HoneycombUtility.GameUtil.ResetTransformStretchXY(g);
            debugwindow = g.GetComponent<DebugMessageWindowController>();
            debugwindow.onDestroyCallback += () => { debugwindow = null; };
        }
        debugwindow.CreateMessage(title, text);
    }
#endif


}