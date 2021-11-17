using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogueLine {
    public readonly string Text;
    public readonly string Speaker;
    public readonly float Duration;

    public DialogueLine(string text, string speaker, int duration) {
        Text = text;
        Speaker = speaker;
        Duration = duration;
    }
}

public class SubtitleManager : MonoBehaviour {
    public GameObject subtitlesTextBox;
    public string locale = "en_US";
    private static SubtitleManager _instance;
    private readonly SortedDictionary<string, DialogueLine> _dialogueLines;
    private bool _subtitleActive;

    public SubtitleManager() {
        _dialogueLines = new SortedDictionary<string, DialogueLine>();
    }

    private async void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
        print("Loading dialogue lines...");
        if (await LoadDialogLines()) {
            print("Successfully loaded dialogue lines.");
        } else {
            print("Failed to load dialogue lines.");
        }
    }

    private async Task<bool> LoadDialogLines() {
        // Throw exception if an unsupported locale is used (currently only en_US is supported)
        if (locale != "en_US") {
            throw new NotSupportedException();
        }
        
        var data = CsvParser.Read("dialogue_lines");
        if (data == null) {
            return false;
        }
        
        foreach (var row in data) {
            _dialogueLines[(string) row["line_id"]] = new DialogueLine((string) row["line_" + locale], (string) row["line_speaker"], (int) row["line_duration"]);
        }

        foreach (var lineId in _dialogueLines.Keys) {
            print(lineId);
            await RenderSubtitle(lineId);
            await Task.Delay(25);
            print("ready for next sub");
        }

        return true;
    }

    private async Task RenderSubtitle(string lineId) {
        while (_subtitleActive) {
            await Task.Delay(25);
        }
        var line = _dialogueLines[lineId];
        print(lineId);
        print(line.Text);
        print(line.Speaker);
        print(line.Duration);
        
        var subtitleText = line.Speaker + ": " + line.Text;
        print(subtitleText);
        var textBox = subtitlesTextBox.GetComponent<TextMeshProUGUI>();
        textBox.SetText(subtitleText);
        _subtitleActive = true;
        
        await Task.Delay((int) line.Duration * 1000).ContinueWith(t => {
            _subtitleActive = false;
        });
        print("time up");
        
        while (_subtitleActive) {
            await Task.Delay(25);
        }
        
        textBox.SetText("");
        
        // TODO: Use id from dict key to send event to Wwise
    }
}
