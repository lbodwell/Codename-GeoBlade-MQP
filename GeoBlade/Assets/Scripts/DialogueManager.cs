using System;
using System.Collections.Generic;
using System.Threading;
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
    private CancellationTokenSource _source;
    private CancellationToken _token;
    private bool _lineActive;
    private bool _sequenceActive;

    private void Awake() {
        Instance = this;
        _dialogueLines = new SortedDictionary<string, DialogueLine>();
        ResetCancellationToken();

        Debug.Log("Loading dialogue lines...");
        if (!LoadDialogLines()) {
            Debug.Log("Failed to load dialogue lines.");
            return;
        }
        Debug.Log("Successfully loaded dialogue lines.");
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
    
    
    //TODO: Verify that all async task delays are required and their timings are optimal for minimizing busy-waiting
    public async Task PlayDialogueSequence(string firstLineId) {
        if (_sequenceActive) {
            while (_lineActive) {
                await Task.Delay(100, _token);
                Debug.Log("Waiting for active line to finish to start next dialogue sequence...");
            }

            _source.Cancel();
        }

        var nextLine = firstLineId;

        while (true) {
            if (nextLine == "END") {
                break;
            }

            try {
                _sequenceActive = true;
                nextLine = await PlayLine(nextLine, _token);
                await Task.Delay(100, _token);
            }
            catch (OperationCanceledException) {
                Debug.Log("Current dialogue sequence cancelled");
                break;
            }
            finally {
                ResetCancellationToken();
            }
        }

        _sequenceActive = false;
    }

    private async Task<string> PlayLine(string lineId, CancellationToken token) {
        while (_lineActive) {
            await Task.Delay(250, token);
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
        
        // TODO: Find more elegant way to check if singletons and their member values are defined yet
        while (PlayerManager.Instance == null) {
            await Task.Delay(50, token);
        }
        while (PlayerManager.Instance.player == null) {
            await Task.Delay(50, token);
        }

        // TODO: Make this more robust
        if (line.Speaker == "Seru") {
            AkSoundEngine.PostEvent("Dialogue_Trigger", PlayerManager.Instance.player);
        } else if (line.Speaker == "Iris") {
            AkSoundEngine.PostEvent("Dialogue_Trigger", PlayerManager.Instance.iris);
        } else {
            Debug.Log("Invalid speaker for current dialogue line");
        }
        
        
        await Task.Delay((int) (line.Duration * 1000), token).ContinueWith(t => _lineActive = false, token);
        
        if (subtitlesTextBox != null) {
            var textBox = subtitlesTextBox.GetComponent<TextMeshProUGUI>();
            while (_lineActive) {
                await Task.Delay(250, token);
            }
            
            if (textBox != null) {
                textBox.SetText("");
            }
        }
        
        if (line.PostDelay > 0) {
            await Task.Delay((int) (line.PostDelay * 1000), token);
        }
        
        return line.NextLine;
    }
    
    private void ResetCancellationToken() {
        _source?.Dispose();
        _source = new CancellationTokenSource();
        _token = _source.Token;
    }
}
