using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ImageComponent = UnityEngine.UI.Image; //To make the Image class be the correct image component and not some weird microsoft stuff

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(AudioSource))]
public class DialogueVisualiser : MonoBehaviour
{
    public static DialogueVisualiser singleton;

    void OnValidate()
    {
        if (!singleton)
            singleton = this;
    }

    [Header("References To Important Objects")]
    [SerializeField]
    private GameObject flashingArrow;

    [SerializeField]
    private TextMeshProUGUI textbox;

    [SerializeField]
    private ImageComponent profileImage;

    private List<Message> messageQueue = new List<Message>();
    private string currentMessage = "";
    private float timeLeftToType = 0;
    private float startAmount = 0;
    private int qIndex = 0;
    public Action onDoneTalking;
    private float sample_rate = 44100f;

    private float current_npc_freq = 440f;

    private AudioSource audioSource;

    public bool CurrentlyTyping() => timeLeftToType != 0;

    public string CurrentMessage() => currentMessage;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.45f;
        if (!singleton)
            singleton = this;
        print($"{profileImage.gameObject.name} {textbox.gameObject.name}");
    }

    void OnEnable()
    {
        if (Controller.main == null)
            return;

        Controller.main.enabled = false;
    }

    void OnDisable()
    {
        if (Controller.main == null)
            return;

        Controller.main.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(SettingsMenu.singleton.nextDialogueKeybind.key))
            PlayNextMessage();
        if (Input.GetKeyDown(SettingsMenu.singleton.previousDialogueKeybind.key))
            PlayPreviousMessage();
        if (Input.GetKeyDown(SettingsMenu.singleton.skipDialogueKeybind.key))
            SkipTyping();

        //Typewriter Effect
        if (timeLeftToType > 0)
        {
            textbox.text = currentMessage.Substring(
                0,
                1 + (int)(currentMessage.Length * (1 - (timeLeftToType / startAmount)))
            );
            timeLeftToType -= Time.deltaTime;

            // Could be an event but have no reason for it yet
            if (timeLeftToType <= 0)
                timeLeftToType = 0;
        }
        else
        {
            flashingArrow.SetActive(true);
        }
    }

    AudioClip CreateToneAudioClip(float frequency, float length_seconds, float pulse_rate)
    {
        int sampleLength = (int)(sample_rate * length_seconds);
        float[] samples = new float[sampleLength];

        float pulsePeriod = 1f / pulse_rate;
        float halfPulseSamples = pulsePeriod * sample_rate / 2f;

        float current_sample_diff = UnityEngine.Random.Range(-50f, 50f);
        bool on_last = false;

        float fadeTime = 0.01f; // 10 ms fade time
        float fadeSamples = fadeTime * sample_rate;

        for (int i = 0; i < sampleLength; i++)
        {
            float t = i / sample_rate;
            bool isOn = (i / halfPulseSamples % 2) < 1;

            if (isOn)
            {
                if (!on_last)
                {
                    current_sample_diff = UnityEngine.Random.Range(-50f, 50f);
                }

                // Determine sample position within the current pulse
                float pulseSample = i % (int)(pulsePeriod * sample_rate);
                float fadeMultiplier = 1f;

                // Fade in
                if (pulseSample < fadeSamples)
                {
                    fadeMultiplier = pulseSample / fadeSamples;
                }
                // Fade out
                else if (pulseSample > (pulsePeriod * sample_rate / 2f - fadeSamples))
                {
                    float samplesIntoFade =
                        pulseSample - (pulsePeriod * sample_rate / 2f - fadeSamples);
                    fadeMultiplier = 1f - (samplesIntoFade / fadeSamples);
                }

                float wave = Mathf.Sin(2 * Mathf.PI * (frequency + current_sample_diff) * t);
                samples[i] = wave * fadeMultiplier;
            }
            else
            {
                samples[i] = 0f;
            }

            on_last = isOn;
        }

        AudioClip clip = AudioClip.Create("SineWave", sampleLength, 1, (int)sample_rate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    public void WriteMessage(Message msg) =>
        WriteMessage(msg.message, msg.time, msg.usingCPS, msg.profileImage);

    public void WriteMessage(
        string message,
        float time = 8f,
        TimeSettings usingCPS = TimeSettings.SecondsPerChar,
        Sprite img = null
    )
    {
        flashingArrow.SetActive(false);
        switch (usingCPS)
        {
            case TimeSettings.TotalTime:
            {
                startAmount = time;
                break;
            }
            case TimeSettings.CharsPerSecond:
            {
                startAmount = message.Length / time;
                break;
            }
            case TimeSettings.SecondsPerChar:
            {
                startAmount = message.Length * time;
                break;
            }
        }
        timeLeftToType = startAmount;

        AudioClip clip = CreateToneAudioClip(current_npc_freq, startAmount, 10);
        audioSource.clip = clip;
        audioSource.Play();

        currentMessage = message;
        if (img && profileImage)
            profileImage.sprite = img;
        print($"Writing message: {message} with time: {time} and usingCPS: {usingCPS}");
    }

    public void AddToQueue(params Message[] messages) => messageQueue.AddRange(messages);

    public void AddToQueue(
        float timeForAll,
        TimeSettings usingCharacterTime,
        params string[] strings
    )
    {
        Message[] messages = new Message[strings.Length];
        for (int i = 0; i < strings.Length; i++)
        {
            messages[i] = new Message(strings[i], timeForAll, usingCharacterTime);
        }
        messageQueue.AddRange(messages);
    }

    public void AddToQueue(
        float timeForAll,
        TimeSettings usingCharacterTime,
        Sprite img,
        params string[] strings
    )
    {
        Message[] messages = new Message[strings.Length];
        for (int i = 0; i < strings.Length; i++)
        {
            messages[i] = new Message(strings[i], timeForAll, usingCharacterTime, img);
        }
        messageQueue.AddRange(messages);
    }

    public void ClearQueue() => messageQueue.Clear();

    public void SetQueue(System.Action onFinish = null, params Message[] messages)
    {
        onDoneTalking = onFinish;
        messageQueue.Clear();
        messageQueue.AddRange(messages);
    }

    public void SetQueueAndPlayFirst(
        float voiceFrequency,
        System.Action onFinish = null,
        params Message[] messages
    )
    {
        current_npc_freq = voiceFrequency;
        onDoneTalking = onFinish;
        messageQueue.Clear();
        messageQueue.AddRange(messages);
        PlayCurrentMessage();
    }

    public void SetQueueAndPlayFirst(System.Action onFinish = null, params string[] messages)
    {
        onDoneTalking = onFinish;
        messageQueue.Clear();
        messageQueue.AddRange(
            messages.Select((m) => new Message(m, 8, TimeSettings.CharsPerSecond, null))
        );
        PlayCurrentMessage();
    }

    public void SetQueue(
        float timeForAll,
        TimeSettings usingCharacterTime,
        System.Action onFinish = null,
        params string[] strings
    )
    {
        onDoneTalking = onFinish;
        messageQueue.Clear();
        Message[] messages = new Message[strings.Length];
        for (int i = 0; i < strings.Length; i++)
        {
            messages[i] = new Message(strings[i], timeForAll, usingCharacterTime);
        }
        messageQueue.AddRange(messages);
    }

    public void PlayPreviousMessage()
    {
        if (messageQueue.Count > 0 && qIndex > 0)
            WriteMessage(messageQueue[--qIndex]);
    }

    public void PlayCurrentMessage()
    {
        print($"Playing current message: {messageQueue[qIndex].message}");
        WriteMessage(messageQueue[qIndex]);
    }

    public void ClosePage()
    {
        print("No more messages in queue, closing dialogue UI");
        UIStateManager.singleton.CloseUIPage(UIState.Dialogue);
        qIndex = 0;
        messageQueue.Clear();
        currentMessage = "";
        textbox.text = "";
        timeLeftToType = 0;
        onDoneTalking?.Invoke();
    }

    public void PlayNextMessage()
    {
        if (CurrentlyTyping())
        {
            SkipTyping();
            return;
        }
        if (messageQueue.Count > 0 && qIndex < messageQueue.Count - 1)
            WriteMessage(messageQueue[++qIndex]);
        else
            ClosePage();
    }

    public void SkipTyping()
    {
        textbox.text = currentMessage;
        timeLeftToType = 0;
        audioSource.Stop();
    }
}

[Serializable]
public enum TimeSettings
{
    TotalTime,
    CharsPerSecond,
    SecondsPerChar,
};

[Serializable]
public struct Message
{
    public Sprite profileImage;
    public string message;
    public float time;

    //Whether the message should use time as a measure of characters per second or how long the whole message should take

    public TimeSettings usingCPS;

    public Message(string message, float time, TimeSettings usingCPS, Sprite profileImage)
    {
        this.time = time;
        this.message = message;
        this.usingCPS = usingCPS;
        this.profileImage = profileImage;
    }

    //THIS IS GOING TO BE DEPRECATED SOON
    public Message(string message, float time, TimeSettings usingCPS)
    {
        this.time = time;
        this.message = message;
        this.usingCPS = usingCPS;
        profileImage = null;
    }
}
