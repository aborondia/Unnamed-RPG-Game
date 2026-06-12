using System;
using System.Collections.Generic;
using RPGGame;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
public class UIController : MonoBehaviour
{
    [Inject] private IObjectResolver resolver;
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset consoleLineTemplate;
    private Queue<VisualElement> consoleLines = new Queue<VisualElement>();
    private VisualElement root;
    private ScrollView contentScrollView;
    private VisualElement userInputFieldParent;
    private TextField userInputField;
    public TextField UserInputField => userInputField;
    private VisualElement lastLineModified;

    private void Awake()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        this.root = this.uiDocument.rootVisualElement;
        this.contentScrollView = this.root.Q<ScrollView>();
        this.userInputFieldParent = this.contentScrollView.contentContainer.Q<TemplateContainer>("UserInputField");
        this.userInputField = this.userInputFieldParent.Q<TextField>();
        this.UserInputField.RegisterCallback<FocusOutEvent>(evt => this.UserInputField.Focus());
        this.contentScrollView.contentContainer.Clear();
    }

    public Label WriteLine(int value)
    {
        return WriteLine(value.ToString());
    }

    public Label WriteLine(string value = "")
    {
        Label label = GetConsoleLabel(true);
        label.AddToClassList(GetFontColorSelector(ConsoleColor.White));

        label.text = value;
        this.resolver.Resolve<GameEngine>().PerformActionAfterPause(() => ScrollToEnd(), 50);
        return label;
    }

    public Label Write(string value)
    {
        Label label = GetConsoleLabel(false);
        label.AddToClassList(GetFontColorSelector(ConsoleColor.White));

        label.text = value;

        return label;
    }

    public void WriteColorText(ConsoleColor consoleColor, string value, bool newLine = true)
    {
        Label label = newLine ? WriteLine(value) : Write(value);

        ColorText(label, consoleColor);
    }

    public void ShowUserInputField()
    {
        this.userInputField.value = String.Empty;
        this.userInputField.focusable = true;
        this.userInputField.style.display = DisplayStyle.Flex;
        this.contentScrollView.contentContainer.Add(this.userInputFieldParent);
        this.userInputField.Focus();
    }

    public void HideUserInputField()
    {
        this.userInputField.focusable = false;
        this.userInputField.style.display = DisplayStyle.None;
        // this.contentScrollView.contentContainer.Remove(this.userInputFieldParent);
    }

    private void ColorText(Label label, ConsoleColor consoleColor)
    {
        label.RemoveFromClassList("label-white");
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

        if (labelContainer == null)
        {
            return GetConsoleLabel(newLine);
        }
        
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
        foreach (VisualElement consoleLine in this.contentScrollView.contentContainer.Children())
        {
            this.consoleLines.Enqueue(consoleLine);
        }

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

    public void ScrollToEnd()
    {
        this.contentScrollView.verticalScroller.value = this.contentScrollView.verticalScroller.highValue;
    }
    // TODO Implement for mobile
    // public void SimulateKeyPressed()
    // {
    //     InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(Key.W));
    // }
}
