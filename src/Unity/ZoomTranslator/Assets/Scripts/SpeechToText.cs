//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// <code>
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
//using UnityEditor.VersionControl;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class SpeechToText : MonoBehaviour
{
    // Hook up the two properties below with a Text and Button object in your UI.
    //public Text outputText;
    //public Button startRecoButton;

    public event System.Action<ResultReason, string> onEndRecognition = delegate { };

    private object threadLocker = new object();
    private bool micPermissionGranted = false;

#if PLATFORM_ANDROID
    // Required to manifest microphone permission, cf.
    // https://docs.unity3d.com/Manual/android-manifest.html
    private Microphone mic;
#endif

    void onRecognized(object sender, SpeechRecognitionEventArgs a)
    {
        string newMessage = string.Empty;
        if (a.Result.Reason == ResultReason.RecognizedSpeech)
        {
            newMessage = a.Result.Text;
        }
        else if (a.Result.Reason == ResultReason.NoMatch)
        {
            newMessage = "NOMATCH: Speech could not be recognized.";
        }
        else if (a.Result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(a.Result);
            newMessage = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
        }

        onEndRecognition(a.Result.Reason, a.Result.Text);
    }
    void onCanceled(object sender, SpeechRecognitionCanceledEventArgs a)
    {
        string newMessage = string.Empty;
        newMessage = a.Result.Reason.ToString() + " ";
        onEndRecognition(a.Result.Reason, a.Result.Text);

    }

    public async Task StartRecognition()
    {
        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        //var config = SpeechConfig.FromSubscription("006be514f548436ea52d8add25eb43b1", "japaneast");//"japaneast", "eastasia"
        var config = SpeechConfig.FromSubscription("1cc152501f59408cb0a6494f2b1a8b26", "japaneast");//"japaneast", "eastasia"
        config.SpeechRecognitionLanguage = "ja-jp";
        


        // Make sure to dispose the recognizer after use!
        Debug.Log("Start recognition.");
#if true
        while(true)
        {
            using (var recognizer = new SpeechRecognizer(config))
            {
                lock (threadLocker)
                {
                    //waitingForReco = true;
                }

                // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result.
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query.
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);                

                // Checks result.
                string newMessage = string.Empty;
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    newMessage = result.Text;
                    Debug.Log("" + newMessage);
                    //if (result.Text.Contains("‚±‚ñ‚É‚¿‚í"))
                    {
                        onEndRecognition(result.Reason, newMessage);
                    }
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    newMessage = "NOMATCH: Speech could not be recognized.";
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    newMessage = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
                }

                Debug.Log("next " + newMessage);
#if false
            lock (threadLocker)
            {
                message = newMessage;
                waitingForReco = false;
            }
#endif
            }

        }
#else
        var recognizer = new SpeechRecognizer(config);
        recognizer.Recognized += onRecognized;
        await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(true);
#endif
    }

    void Start()
    {
        {
            // Continue with normal initialization, Text and Button objects are present.

#if PLATFORM_ANDROID
            // Request to use the microphone, cf.
            // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#else
            micPermissionGranted = true;
            //message = "Click button to recognize speech";
#endif
        }
    }

    void Update()
    {
#if PLATFORM_ANDROID
        if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            micPermissionGranted = true;
        }
#endif
#if false
        lock (threadLocker)
        {
            if (startRecoButton != null)
            {
                startRecoButton.interactable = !waitingForReco && micPermissionGranted;
            }
            if (outputText != null)
            {
                outputText.text = message;
            }
        }
#endif
    }
}
// </code>
