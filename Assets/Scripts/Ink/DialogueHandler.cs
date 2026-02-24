using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Dialogue UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private Button choiceButtonPrefab;
    [SerializeField] private Image overlayScreen;
    [SerializeField] private List<Canvas> choiceCanvasGroup;
    [SerializeField] private Canvas continuationCanvasGroup;

    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    // This is intential and should be left as null.
    // In the event we do some fun UI stuff in a 3-dimensional space this will be useful.
    private Camera eventCamera;

    [Header("Audio (Typewriter)")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip typeBlip;
    [SerializeField] private bool randomizeStartTime = true;
    private bool typingSfxPlaying = false;

    [Header("Audio (Buttons)")]
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip pressedClip;

    [SerializeField] private float spaceDelayFactor = 0.2f;
    [SerializeField] private float commaPause = 0.15f;
    [SerializeField] private float periodPause = 0.35f;
    [SerializeField] private float questionPause = 0.35f;
    [SerializeField] private float exclaimPause = 0.30f;
    [SerializeField] private float ellipsisPause = 0.50f;
    [SerializeField] private float newlinePause = 0.25f;

    [SerializeField] private Vector2 letterPitchJitter = new Vector2(0.98f, 1.04f);
    [SerializeField] private Vector2 punctuationPitchJitter = new Vector2(0.92f, 0.98f);


    public Story story;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool choicesShownThisLine = false;

    private AudioClip originalTypeBlip;

    private Coroutine continueFade = null;

    public string heldStory = "";

    public static DialogueHandler Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        story = new Story(inkJSON.text);

        // Hold original for when voices change.
        originalTypeBlip = typeBlip;

        story.BindExternalFunction("ChangeVoice", (string character) => ChangeVoice(character));

        ContinueStory();
    }

    public void OnButtonPress()
    {
        if (story.currentChoices.Count == 0 && story.canContinue && !isTyping)
        {
            ContinueStory();
        }
        else if (isTyping)
        {
            dialogueText.text = heldStory;
            StopCoroutine(typingCoroutine);
            isTyping = false;
            StopTypingSfx();

            if (!choicesShownThisLine && story.currentChoices.Count > 0)
            {
                choicesShownThisLine = true;
                DisplayChoices();
                FadeContinuation(0.5f, 0f);
                
            }
            else
            {
                if (story.canContinue)
                {
                    FadeContinuation(2f, 1f);
                }
            }
        }
    }

    private void FadeContinuation(float length, float alpha) 
    {
        if (continueFade != null)
        {
            StopCoroutine(continueFade);
        }
        continueFade = StartCoroutine(FadeLogic.Instance.FadeCanvasGroup(continuationCanvasGroup, length, alpha));
    }

    private void ContinueStory()
    {
        FadeContinuation(0.5f, 0f);

        foreach (Transform child in buttonContainer.transform)
            Destroy(child.gameObject);

        choicesShownThisLine = false;

        if (story.canContinue)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            heldStory = story.Continue();

            typingCoroutine = StartCoroutine(DisplayLine(heldStory, dialogueText));
        }
        else
        {
            StopTypingSfx();
            // No choices = end of story
            Debug.Log("Made it here.");
            if (overlayScreen != null) overlayScreen.gameObject.SetActive(false);
            Button endButton = Instantiate(choiceButtonPrefab, buttonContainer.transform);
            endButton.GetComponentInChildren<TextMeshProUGUI>().text = "End";
            endButton.onClick.AddListener(() => gameObject.SetActive(false));
        }
    }

    private void DisplayChoices()
    {
        foreach (Choice choice in story.currentChoices)
        {
            Button button = Instantiate(choiceButtonPrefab, buttonContainer.transform);

            button.gameObject.AddComponent<ButtonNoises>();
            var grabbedButtonNoise = button.GetComponentInChildren<ButtonNoises>();
            grabbedButtonNoise.AddAudioSource(sfxSource);
            grabbedButtonNoise.AddHoverClip(hoverClip);
            grabbedButtonNoise.AddPressedClip(pressedClip);
            grabbedButtonNoise.AssignSelf(button);


            string formattedChoice = choice.text.Trim();
            button.GetComponentInChildren<TextMeshProUGUI>().text = formattedChoice;

            button.onClick.AddListener(() =>
            {
                story.ChooseChoiceIndex(choice.index);
                story.Continue();
                ContinueStory();
                StartCoroutine(FadeLogic.Instance.Fade(choiceCanvasGroup, 3f, 0f));
            });
        }
        StartCoroutine(FadeLogic.Instance.Fade(choiceCanvasGroup, 3f, 1f));
    }

    IEnumerator DisplayLine(string line, TextMeshProUGUI textBox)
    {
        isTyping = true;
        textBox.text = "";

        bool insideTag = false;
        bool audioSuppressedThisLine = false;
        StartTypingSfx();

        int i = 0;
        while (i < line.Length)
        {
            char c = line[i];
            char cNext = ' ';

            if (i < line.Length - 1)
            {
                cNext = line[i + 1];
            }

            if (c == '<') insideTag = true;

            float wait = typingSpeed;
            string toAppend = c.ToString();

            bool isPunctPause = false;

            if (!insideTag)
            {
                // Whitespace quick
                if (char.IsWhiteSpace(c))
                {
                    if (c == '\n' || c == '\r')
                    {
                        wait = newlinePause;
                        StopTypingSfx();
                        audioSuppressedThisLine = true;
                    }
                    else
                    {
                        wait = typingSpeed * spaceDelayFactor;
                        if (sfxSource) sfxSource.pitch = Random.Range(letterPitchJitter.x, letterPitchJitter.y);
                    }
                }
                else
                {
                    // Ellipsis as single unit
                    if (c == '.' && i + 2 < line.Length && line[i + 1] == '.' && line[i + 2] == '.')
                    {
                        toAppend = "...";
                        i += 2; // consume extra dots
                        wait = ellipsisPause;
                        isPunctPause = true;
                    }
                    else if (c == '.')
                    {
                        wait = periodPause;
                        isPunctPause = true;
                    }
                    else if (c == ',')
                    {
                        wait = commaPause;
                        isPunctPause = true;
                    }
                    else if (c == '?')
                    {
                        wait = questionPause;
                        isPunctPause = true;
                    }
                    else if (c == '!')
                    {
                        wait = exclaimPause;
                        isPunctPause = true;
                    }
                    else
                    {
                        // normal visible character
                        wait = typingSpeed;
                    }

                    // Pitch gesture
                    if (sfxSource)
                    {
                        if (isPunctPause)
                            sfxSource.pitch = Random.Range(punctuationPitchJitter.x, punctuationPitchJitter.y);
                        else
                            sfxSource.pitch = Random.Range(letterPitchJitter.x, letterPitchJitter.y);
                    }
                }
            }

            if (c == '>') insideTag = false;

            // Used to hadle ellipsis as one step
            if (cNext != ' ' && IsQuote(cNext))
            {
                i++;
                toAppend += cNext;
            }

            textBox.text += toAppend;

            // If user skipped mid-typing, stop immediately
            if (!isTyping) break;

            // For comma/period/ellipsis it pauses the looping SFX during the stall
            if (isPunctPause) PauseTypingSfx();

            yield return new WaitForSeconds(wait);

            if (!isTyping) break;

            if (isPunctPause && !audioSuppressedThisLine)
                ResumeTypingSfx();

            i++;
        }

        isTyping = false;
        StopTypingSfx();

        if (!choicesShownThisLine && story.currentChoices.Count > 0)
        {
            choicesShownThisLine = true;
            DisplayChoices();
            FadeContinuation(0.5f, 0f);
        } else
        {
            if (story.canContinue)
            {
                FadeContinuation(2f, 1f);
            }
        }
    }

    private void StartTypingSfx()
    {
        if (sfxSource == null || typeBlip == null || typingSfxPlaying) return;
        sfxSource.clip = typeBlip;
        if (randomizeStartTime && typeBlip.length > 0f)
            sfxSource.time = Random.Range(0f, typeBlip.length);
        sfxSource.loop = true;
        sfxSource.Play();
        typingSfxPlaying = true;
    }

    private void StopTypingSfx()
    {
        if (sfxSource == null || !typingSfxPlaying) return;
        sfxSource.Stop();
        typingSfxPlaying = false;
    }

    private void PauseTypingSfx()
    {
        if (sfxSource != null && typingSfxPlaying && sfxSource.isPlaying)
            sfxSource.Pause();
    }

    private void ResumeTypingSfx()
    {
        // If we were playing and got paused, continue from where we left off
        if (sfxSource != null && typingSfxPlaying && !sfxSource.isPlaying)
            sfxSource.UnPause();
    }

    private static bool IsQuote(char c)
    {
        return c == '"' || c == '“' || c == '”' || c == '\'';
    }


    // == PUBLIC GRABBABLE FUNCTIONS ==
    public void StoryJump(string location)
    {
        StopTypingSfx();
        story.ChoosePathString(location);
        ContinueStory();
        if (overlayScreen != null) overlayScreen.gameObject.SetActive(true);
    }

    public void ChangeVoice(string character)
    {
        List<Character> characters = CharacterManager.Instance.grabCharacters();
        foreach (var c in characters)
        {
            if (c.characterName == character)
            {
                typeBlip = c.voiceBank;
                return;
            }
        }

        typeBlip = originalTypeBlip;
    }
}
