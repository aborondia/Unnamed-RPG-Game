using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public static UIController Active;
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset consoleLineTemplate;
    private Queue<VisualElement> consoleLines;
    private VisualElement root;
    private ScrollView contentScrollView;
    private VisualElement lastLineModified;

    private void Awake()
    {
        if (Active != null)
        {
            Destroy(Active);
        }

        Active = this;

        SetupUI();
    }

    private void SetupUI()
    {
        this.root = this.uiDocument.rootVisualElement;
        this.contentScrollView = this.root.Q<ScrollView>();
    }

    public Label WriteLine(string value)
    {
        Label label = GetConsoleLabel(true);

        label.text = value;

        return label;
    }

    public Label Write(string value)
    {
        Label label = GetConsoleLabel(false);

        label.text = value;

        return label;
    }

    public void WriteColorText(ConsoleColor consoleColor, string value, bool newLine = true)
    {
        Label label = newLine ? WriteLine(value) : Write(value);

        ColorText(label, consoleColor);
    }

    private void ColorText(Label label, ConsoleColor consoleColor)
    {
        label.AddToClassList(GetFontColorSelector(consoleColor));
    }

    private Label GetConsoleLabel(bool newLine)
    {
        bool createNewElement = newLine || this.lastLineModified == null;
        VisualElement consoleLine;
        VisualElement labelContainer;
        Label newLabel = new Label();

        if (createNewElement)
        {
            if (this.consoleLines.Count <= 0)
            {
                consoleLine = this.consoleLineTemplate.Instantiate();
                this.consoleLines.Enqueue(consoleLine);
            }
            else
            {
                consoleLine = this.consoleLines.Dequeue();
            }
        }
        else
        {
            consoleLine = this.lastLineModified;
        }

        labelContainer = consoleLine.Q<VisualElement>("label-container");

        if (createNewElement)
        {
            labelContainer.Clear();
        }

        newLabel.AddToClassList("console-line-entry-label");
        labelContainer.Add(newLabel);
        this.contentScrollView.contentContainer.Add(consoleLine);
        this.lastLineModified = consoleLine;

        return newLabel;
    }

    public void Clear()
    {
        this.contentScrollView.contentContainer.Clear();
        this.lastLineModified = null;
    }

    private string GetFontColorSelector(ConsoleColor consoleColor)
    {
        switch (consoleColor)
        {
            case ConsoleColor.Black:
                return "label-black";
            case ConsoleColor.DarkBlue:
                return "label-dark-blue";
            case ConsoleColor.DarkGreen:
                return "label-dark-green";
            case ConsoleColor.DarkCyan:
                return "label-dark-cyan";
            case ConsoleColor.DarkRed:
                return "label-dark-red";
            case ConsoleColor.DarkMagenta:
                return "label-dark-magenta";
            case ConsoleColor.DarkYellow:
                return "label-dark-yellow";
            case ConsoleColor.Gray:
                return "label-gray";
            case ConsoleColor.DarkGray:
                return "label-dark-gray";
            case ConsoleColor.Blue:
                return "label-blue";
            case ConsoleColor.Green:
                return "label-green";
            case ConsoleColor.Cyan:
                return "label-cyan";
            case ConsoleColor.Red:
                return "label-red";
            case ConsoleColor.Magenta:
                return "label-magenta";
            case ConsoleColor.Yellow:
                return "label-yellow";
            case ConsoleColor.White:
                return "label-white";
            default:
                return "label-white";
        }
    }
}
