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

public class DialogueManager : MonoBehaviour {
    public GameObject subtitlesTextBox;
    public string locale = "en_US";
    public bool subtitlesEnabled = true;
    public static DialogueManager Instance;
    private SortedDictionary<string, DialogueLine> _dialogueLines;
    private bool _lineActive;

    private async void Awake() {
        Instance = this;
        _dialogueLines = new SortedDictionary<string, DialogueLine>();

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
            
            nextLine = await PlayLine(nextLine);
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

    private async Task<string> PlayLine(string lineId) {
        while (_lineActive) {
            await Task.Delay(25);
        }

        var line = _dialogueLines[lineId];
        var textBox = subtitlesTextBox.GetComponent<TextMeshProUGUI>();

        if (subtitlesEnabled) {
            var subtitleText = line.Speaker + ": " + line.Text;
            textBox.SetText(subtitleText);
            _lineActive = true;
        }

        await Task.Delay((int)line.Duration * 1000).ContinueWith(t => {
            _lineActive = false;
        });
        
        if (subtitlesEnabled) {
            while (_lineActive) {
                await Task.Delay(25);
            }

            textBox.SetText("");
        }

        // TODO: Use id from dict key to send event to Wwise

        return line.NextLine;
    }
}
