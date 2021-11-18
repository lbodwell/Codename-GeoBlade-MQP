using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogueLine {
    public readonly string Text;
    public readonly string Speaker;
    public readonly float Duration;
    public readonly string NextLine;

    public DialogueLine(string text, string speaker, int duration, string nextLine) {
        Text = text;
        Speaker = speaker;
        Duration = duration;
        NextLine = nextLine;
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
        if (!LoadDialogLines()) {
            print("Failed to load dialogue lines.");
            return;
        }
        print("Successfully loaded dialogue lines.");
        
        var nextLine = "dialogue_lvl1_001";
        while (true) {
            if (nextLine == "END") {
                break;
            }
            
            nextLine = await RenderSubtitles(nextLine);
            await Task.Delay(25);
        }
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
            _dialogueLines[(string) row["line_id"]] = new DialogueLine((string) row["line_" + locale], (string) row["line_speaker"], (int) row["line_duration"], (string) row["next_line"]);
        }

        return true;
    }

    private async Task<string> RenderSubtitles(string lineId) {
        while (_subtitleActive) {
            await Task.Delay(25);
        }

        var line = _dialogueLines[lineId];

        var subtitleText = line.Speaker + ": " + line.Text;
        var textBox = subtitlesTextBox.GetComponent<TextMeshProUGUI>();
        textBox.SetText(subtitleText);
        _subtitleActive = true;

        await Task.Delay((int) line.Duration * 1000).ContinueWith(t => {
            _subtitleActive = false;
        });

        while (_subtitleActive) {
            await Task.Delay(25);
        }

        textBox.SetText("");
        // TODO: Use id from dict key to send event to Wwise

        return line.NextLine;
    }
}
