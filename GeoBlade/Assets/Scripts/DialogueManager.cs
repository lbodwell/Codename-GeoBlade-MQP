using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogueLine {
    public readonly string Text;
    public readonly string Speaker;
    public readonly float Duration;
    public readonly float PostDelay;
    public readonly string NextLine;

    public DialogueLine(string text, string speaker, float duration, float postDelay, string nextLine) {
        Text = text;
        Speaker = speaker;
        Duration = duration;
        PostDelay = postDelay;
        NextLine = nextLine;
    }
}

public class DialogueManager : MonoBehaviour {
    public GameObject subtitlesTextBox;
    public string locale = "en_US";
    public bool subtitlesEnabled = true;
    public static DialogueManager Instance;
    private SortedDictionary<string, DialogueLine> _dialogueLines;
    private bool _lineActive;
    private bool _interruptRequested;

    private void Awake() {
        Instance = this;
        _dialogueLines = new SortedDictionary<string, DialogueLine>();

        print("Loading dialogue lines...");
        if (!LoadDialogLines()) {
            print("Failed to load dialogue lines.");
            return;
        }
        print("Successfully loaded dialogue lines.");
    }

    private bool LoadDialogLines() {
        // Throw exception if an unsupported locale is used (currently only en_US is supported)
        if (locale != "en_US" && locale != "test") {
            throw new NotSupportedException();
        }
        
        var data = CsvParser.Read("dialogue_lines");
        if (data == null) {
            return false;
        }
        
        foreach (var row in data) {
            var duration = 5.0f;
            var durationStr = (string) row["line_duration"];
            if (float.TryParse(durationStr, out var dur)) {
                duration = dur;
            }
            
            var postDelay = 0.0f;
            var postDelayStr = (string) row["post_delay"];
            if (float.TryParse(postDelayStr, out var delay)) {
                postDelay = delay;
            }
            
            _dialogueLines[(string) row["line_id"]] = new DialogueLine((string) row["line_" + locale], (string) row["line_speaker"], duration, postDelay, (string) row["next_line"]);
        }

        return true;
    }
    
    public async Task PlayDialogueSequence(string firstLineId) {
        // TODO: Use cancellation tokens to end task early if dialogue is interrupted by new line
        
        var nextLine = firstLineId;

        while (true) {
            if (nextLine == "END") {
                break;
            }
            
            nextLine = await PlayLine(nextLine);
            await Task.Delay(10);
        }
    }

    public async Task<string> PlayLine(string lineId) {
        while (_lineActive) {
            await Task.Delay(10);
        }

        var line = _dialogueLines[lineId];

        if (subtitlesEnabled && subtitlesTextBox != null) {
            var textBox = subtitlesTextBox.GetComponent<TextMeshProUGUI>();

            var subtitleText = line.Speaker + ": " + line.Text;
            if (textBox != null) {
                textBox.SetText(subtitleText);
                _lineActive = true;
            }
        }
        
        AkSoundEngine.SetState("Dialogue_Line", lineId);
        
        // TODO: Fix this garbage
        while (PlayerManager.Instance == null) {
            await Task.Delay(10);
        }
        while (PlayerManager.Instance.player == null) {
            await Task.Delay(10);
        }
        AkSoundEngine.PostEvent("Dialogue_Trigger", PlayerManager.Instance.player);
        
        await Task.Delay((int) (line.Duration * 1000)).ContinueWith(t => {
            _lineActive = false;
        });
        
        if (subtitlesTextBox != null) {
            var textBox = subtitlesTextBox.GetComponent<TextMeshProUGUI>();
            while (_lineActive) {
                await Task.Delay(10);
            }
            
            if (textBox != null) {
                textBox.SetText("");
            }
            
            if (line.PostDelay > 0) {
                await Task.Delay((int) (line.PostDelay * 1000));
            }
        }
        
        return line.NextLine;
    }
}
