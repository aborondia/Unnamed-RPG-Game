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
    [SerializeField] private Sprite gameOverSprite;
    [SerializeField] private Sprite[] shopSprites;
    [SerializeField] private Sprite[] enemySprites;
    private Queue<VisualElement> consoleLines = new Queue<VisualElement>();
    private VisualElement root;
    private VisualElement mainContentContainer;
    private ScrollView contentScrollView;
    private VisualElement enemyModelContainer;
    private VisualElement enemyModelImage;
    private Label enemyModelBuffLabel;
    private VisualElement shopModelContainer;
    private VisualElement shopModelImage;
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
        this.mainContentContainer = this.root.Q<VisualElement>("content-container");
        this.contentScrollView = this.root.Q<ScrollView>();
        this.enemyModelContainer = this.root.Q<VisualElement>("enemy-model-container");
        this.enemyModelImage = this.enemyModelContainer.Q<VisualElement>("image");
        this.enemyModelBuffLabel = this.enemyModelContainer.Q<Label>();
        this.enemyModelBuffLabel.RemoveFromClassList("label-white");
        this.enemyModelBuffLabel.AddToClassList(GetFontColorSelector(ConsoleColor.Cyan));
        this.shopModelContainer = this.root.Q<VisualElement>("shop-model-container");
        this.shopModelImage = this.shopModelContainer.Q<VisualElement>("image");
        this.userInputFieldParent = this.contentScrollView.contentContainer.Q<TemplateContainer>("UserInputField");
        this.userInputField = this.userInputFieldParent.Q<TextField>();
        this.UserInputField.RegisterCallback<FocusOutEvent>(evt => this.UserInputField.Focus());
        Clear();
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

    public void WriteEnemyBuff(string value)
    {
        this.enemyModelBuffLabel.text += $"{value} ";
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

    public void DrawEnemyModel(Difficulty difficulty)
    {
        this.enemyModelImage.style.backgroundImage = new StyleBackground(this.enemySprites[(int)difficulty]);
        this.enemyModelContainer.style.display = DisplayStyle.Flex;
        this.mainContentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
    }

    public void DrawShopModel(Shops shop)
    {
        this.shopModelImage.style.backgroundImage = new StyleBackground(this.shopSprites[(int)shop]);
        this.shopModelContainer.style.display = DisplayStyle.Flex;
    }

    public void DrawGameOverModel()
    {
        this.shopModelImage.style.backgroundImage = new StyleBackground(this.gameOverSprite);
        this.shopModelContainer.style.display = DisplayStyle.Flex;
    }

    public void Clear()
    {
        ClearText();
        ClearImage();
    }

    public void ClearText()
    {
        foreach (VisualElement consoleLine in this.contentScrollView.contentContainer.Children())
        {
            this.consoleLines.Enqueue(consoleLine);
        }

        this.enemyModelBuffLabel.text = String.Empty;
        this.contentScrollView.contentContainer.Clear();
        this.lastLineModified = null;
    }

    public void ClearImage()
    {
        this.enemyModelContainer.style.display = DisplayStyle.None;
        this.shopModelContainer.style.display = DisplayStyle.None;
        this.mainContentContainer.style.flexDirection = StyleKeyword.None;
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
}
