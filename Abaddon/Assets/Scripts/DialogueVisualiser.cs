using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ImageComponent = UnityEngine.UI.Image; //To make the Image class be the correct image component and not some weird microsoft stuff

[RequireComponent(typeof(TextMeshProUGUI))]
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
    private TextMeshProUGUI textbox;

    [SerializeField]
    private ImageComponent profileImage;

    [Header("Key Binds for Traversing Messages")]
    [SerializeField]
    private KeyCode nextMessage;

    [SerializeField]
    private KeyCode previousMessage;

    [SerializeField]
    private KeyCode skipTyping;

    private List<Message> messageQueue = new List<Message>();
    private string currentMessage = "";
    private float timeLeftToType = 0;
    private float startAmount = 0;
    private int qIndex = 0;
    public Action onDoneTalking;

    public bool CurrentlyTyping() => timeLeftToType != 0;

    public string CurrentMessage() => currentMessage;

    void Awake()
    {
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
        if (Input.GetKeyDown(nextMessage))
            PlayNextMessage();
        if (Input.GetKeyDown(previousMessage))
            PlayPreviousMessage();
        if (Input.GetKeyDown(skipTyping))
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

    public void SetQueueAndPlayFirst(System.Action onFinish = null, params Message[] messages)
    {
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
