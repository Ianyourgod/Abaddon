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

    void OnValidate() { if (!singleton) singleton = this; }

    [Header("References To Important Objects")]
    [SerializeField] private TextMeshProUGUI textbox;
    [SerializeField] private ImageComponent profileImage;

    [Header("Key Binds for Traversing Messages")]
    [SerializeField] private KeyCode nextMessage;
    [SerializeField] private KeyCode previousMessage;
    [SerializeField] private KeyCode skipTyping;

    private List<Message> messageQueue = new List<Message>();
    private string currentMessage = "";
    private float timeLeftToType = 0;
    private float startAmmount = 0;
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
        Controller.main.enabled = false;
    }

    void OnDisable()
    {
        Controller.main.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(nextMessage)) PlayNextMessage();
        if (Input.GetKeyDown(previousMessage)) PlayPreviousMessage();
        if (Input.GetKeyDown(skipTyping)) SkipTyping();

        //Typewriter Effect
        if (timeLeftToType > 0)
        {
            textbox.text = currentMessage.Substring(0, 1 + (int)(currentMessage.Length * (1 - (timeLeftToType / startAmmount))));
            timeLeftToType -= Time.deltaTime;

            // Could be an event but have no reason for it yet
            if (timeLeftToType <= 0) timeLeftToType = 0;
        }
    }

    public void WriteMessage(Message msg) => WriteMessage(msg.message, msg.time, msg.usingCPS, msg.profileImage);
    public void WriteMessage(string message, float time = 8f, bool usingCPS = true, Sprite img = null)
    {
        print("A");
        startAmmount = usingCPS ? message.Length * time : time;
        print("B");
        timeLeftToType = startAmmount;
        print("C");
        currentMessage = message;
        print("D");
        if (img)
        {
            print("D1");
            if (profileImage) profileImage.sprite = img;
            print("D2");
        }
        print("E");
        print($"Writing message: {message} with time: {time} and usingCPS: {usingCPS}");
    }


    public void AddToQueue(params Message[] messages) => messageQueue.AddRange(messages);
    public void AddToQueue(float timeForAll, bool usingCharacterTime, params string[] strings)
    {
        Message[] messages = new Message[strings.Length];
        for (int i = 0; i < strings.Length; i++)
        {
            messages[i] = new Message(strings[i], timeForAll, usingCharacterTime);
        }
        messageQueue.AddRange(messages);
    }

    public void AddToQueue(float timeForAll, bool usingCharacterTime, Sprite img, params string[] strings)
    {
        Message[] messages = new Message[strings.Length];
        for (int i = 0; i < strings.Length; i++)
        {
            messages[i] = new Message(strings[i], timeForAll, usingCharacterTime, img);
        }
        messageQueue.AddRange(messages);
    }

    public void ClearQueue() => messageQueue.Clear();
    public void SetQueue(params Message[] messages) { messageQueue.Clear(); messageQueue.AddRange(messages); }
    public void SetQueueAndPlayFirst(params Message[] messages) { messageQueue.Clear(); messageQueue.AddRange(messages); PlayCurrentMessage(); }
    public void SetQueueAndPlayFirst(params string[] messages) { messageQueue.Clear(); messageQueue.AddRange(messages.Select((m) => new Message(m, 8, true, null))); PlayCurrentMessage(); }
    public void SetQueue(float timeForAll, bool usingCharacterTime, params string[] strings)
    {
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
        if (messageQueue.Count > 0 && qIndex > 0) WriteMessage(messageQueue[--qIndex]);
    }

    public void PlayCurrentMessage() { print($"Playing current message: {messageQueue[qIndex].message}"); WriteMessage(messageQueue[qIndex]); }
    public void ClosePage()
    {
        print("No more messages in queue, closing dialogue UI");
        UIStateManager.singleton.CloseUIPage(UIState.Dialogue);
        qIndex = 0;
        messageQueue.Clear();
        currentMessage = "";
        textbox.text = "";
        timeLeftToType = 0;
    }

    public void PlayNextMessage()
    {
        if (CurrentlyTyping())
        {
            SkipTyping();
            return;
        }
        if (messageQueue.Count > 0 && qIndex < messageQueue.Count - 1) WriteMessage(messageQueue[++qIndex]);
        else ClosePage();
    }

    public void SkipTyping()
    {
        textbox.text = currentMessage;
        timeLeftToType = 0;
    }
}

[Serializable]
public struct Message
{
    public Sprite profileImage;
    public string message;
    public float time;

    //Whether the message should use time as a measure of characters per second or how long the whole message should take
    public bool usingCPS;

    public Message(string message, float time, bool usingCPS, Sprite profileImage)
    {
        this.time = time;
        this.message = message;
        this.usingCPS = usingCPS;
        this.profileImage = profileImage;
    }

    //THIS IS GOING TO BE DEPRECATED SOON
    public Message(string message, float time, bool usingCPS)
    {
        this.time = time;
        this.message = message;
        this.usingCPS = usingCPS;
        profileImage = null;
    }
}