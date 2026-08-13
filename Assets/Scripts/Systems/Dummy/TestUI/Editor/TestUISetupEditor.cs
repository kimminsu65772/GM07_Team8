using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class TestUISetupEditor
{
    private const string ScenePath =
        "Assets/Scenes/FeatureTestScenes/Systems/BattleSystemTestScene.unity";
    private const string CanvasName = "SystemsTestCanvas";

    private static readonly Color PanelColor =
        new Color(0.08f, 0.09f, 0.11f, 0.94f);
    private static readonly Color ButtonColor =
        new Color(0.18f, 0.42f, 0.66f, 1f);
    private static readonly Color SecondaryButtonColor =
        new Color(0.25f, 0.29f, 0.34f, 1f);

    [MenuItem("Tools/Test UI/Build Battle System Test UI")]
    public static void BuildBattleSystemTestUI()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath)
        {
            scene = EditorSceneManager.OpenScene(
                ScenePath,
                OpenSceneMode.Single);
        }

        GameObject existingCanvas = GameObject.Find(CanvasName);
        if (existingCanvas != null && existingCanvas.scene == scene)
        {
            Object.DestroyImmediate(existingCanvas);
        }

        Canvas canvas = CreateCanvas();
        GameObject panelRoot = CreatePanelRoot(canvas.transform);
        CreatePanelToggle(canvas.transform, panelRoot);

        AirshipUpgradeController upgradeController =
            FindSceneComponent<AirshipUpgradeController>(scene);
        AirshipHeroPlacementPoints placementPoints =
            FindSceneComponent<AirshipHeroPlacementPoints>(scene);
        BattleManager battleManager =
            FindSceneComponent<BattleManager>(scene);

        CreateAirshipUpgradePanel(
            panelRoot.transform,
            upgradeController);
        CreateHeroLevelPanel(
            panelRoot.transform,
            placementPoints);
        CreateHeroFormationPanel(
            panelRoot.transform,
            battleManager);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Selection.activeGameObject = canvas.gameObject;
        Debug.Log(
            $"Created {CanvasName} in {ScenePath}. " +
            $"UpgradeController: {upgradeController != null}, " +
            $"PlacementPoints: {placementPoints != null}, " +
            $"BattleManager: {battleManager != null}");
    }

    private static Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject(
            CanvasName,
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    private static GameObject CreatePanelRoot(Transform parent)
    {
        GameObject root = CreateUIObject("TestPanels", parent);
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(24f, 24f);
        rect.offsetMax = new Vector2(-24f, -24f);

        HorizontalLayoutGroup layout =
            root.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 12, 12);
        layout.spacing = 16f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        return root;
    }

    private static void CreatePanelToggle(
        Transform parent,
        GameObject panelRoot)
    {
        Button toggleButton = AddButton(
            parent,
            "TestPanelToggleButton",
            "Close Tests",
            SecondaryButtonColor);

        RectTransform rect =
            toggleButton.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.one;
        rect.anchorMax = Vector2.one;
        rect.pivot = Vector2.one;
        rect.anchoredPosition = new Vector2(-24f, -24f);
        rect.sizeDelta = new Vector2(160f, 48f);

        TMP_Text buttonText =
            toggleButton.GetComponentInChildren<TMP_Text>(true);
        TestUIPanelToggle toggle =
            parent.gameObject.AddComponent<TestUIPanelToggle>();

        SerializedObject serialized = new SerializedObject(toggle);
        SetReference(serialized, "panelRoot", panelRoot);
        SetReference(serialized, "toggleButton", toggleButton);
        SetReference(serialized, "buttonText", buttonText);
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateAirshipUpgradePanel(
        Transform parent,
        AirshipUpgradeController upgradeController)
    {
        GameObject panel = CreatePanel("AirshipUpgradeTestUI", parent);
        AddText(panel.transform, "Title", "Airship Upgrade", 30f, 48f);

        Button attackButton = AddButton(panel.transform, "AttackButton", "Attack");
        Button defenseButton = AddButton(panel.transform, "DefenseButton", "Defense");
        Button maxHealthButton = AddButton(panel.transform, "MaxHealthButton", "Max Health");
        Button criticalButton = AddButton(panel.transform, "CriticalButton", "Critical");
        TMP_Text resultText = AddStatusText(panel.transform, "ResultText");

        AirshipUpgradeTestUI testUI =
            panel.AddComponent<AirshipUpgradeTestUI>();
        SerializedObject serialized = new SerializedObject(testUI);
        SetReference(serialized, "upgradeController", upgradeController);
        SetReference(serialized, "attackButton", attackButton);
        SetReference(serialized, "defenseButton", defenseButton);
        SetReference(serialized, "maxHealthButton", maxHealthButton);
        SetReference(serialized, "criticalButton", criticalButton);
        SetReference(serialized, "resultText", resultText);
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateHeroLevelPanel(
        Transform parent,
        AirshipHeroPlacementPoints placementPoints)
    {
        GameObject panel = CreatePanel("HeroLevelTestUI", parent);
        AddText(panel.transform, "Title", "Hero Level", 30f, 48f);
        AddText(panel.transform, "HeroListLabel", "Owned Heroes", 21f, 30f);

        Transform heroButtonRoot = AddListRoot(
            panel.transform,
            "HeroButtonRoot",
            250f);
        Button heroButtonTemplate = AddButton(
            heroButtonRoot,
            "HeroButtonTemplate",
            "Hero",
            SecondaryButtonColor);
        heroButtonTemplate.gameObject.SetActive(false);

        TMP_Text selectedHeroText = AddText(
            panel.transform,
            "SelectedHeroText",
            string.Empty,
            20f,
            38f);
        Button levelUpButton = AddButton(
            panel.transform,
            "LevelUpButton",
            "Level Up");
        TMP_Text resultText = AddStatusText(panel.transform, "ResultText");

        HeroLevelTestUI testUI = panel.AddComponent<HeroLevelTestUI>();
        SerializedObject serialized = new SerializedObject(testUI);
        SetReference(serialized, "heroButtonRoot", heroButtonRoot);
        SetReference(serialized, "heroButtonTemplate", heroButtonTemplate);
        SetReference(serialized, "levelUpButton", levelUpButton);
        SetReference(serialized, "selectedHeroText", selectedHeroText);
        SetReference(serialized, "resultText", resultText);
        SetReference(serialized, "placementPoints", placementPoints);
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateHeroFormationPanel(
        Transform parent,
        BattleManager battleManager)
    {
        GameObject panel = CreatePanel("HeroFormationTestUI", parent);
        AddText(panel.transform, "Title", "Hero Formation", 30f, 48f);
        AddText(panel.transform, "HeroListLabel", "Owned Heroes", 21f, 30f);

        Transform heroButtonRoot = AddListRoot(
            panel.transform,
            "HeroButtonRoot",
            190f);
        Button heroButtonTemplate = AddButton(
            heroButtonRoot,
            "HeroButtonTemplate",
            "Hero",
            SecondaryButtonColor);
        heroButtonTemplate.gameObject.SetActive(false);

        AddText(panel.transform, "SlotListLabel", "Formation Slots", 21f, 30f);
        Transform slotButtonRoot = AddListRoot(
            panel.transform,
            "SlotButtonRoot",
            250f);
        Button slotButtonTemplate = AddButton(
            slotButtonRoot,
            "SlotButtonTemplate",
            "Slot",
            SecondaryButtonColor);
        slotButtonTemplate.gameObject.SetActive(false);

        TMP_Text selectedHeroText = AddText(
            panel.transform,
            "SelectedHeroText",
            string.Empty,
            20f,
            38f);
        TMP_Text resultText = AddStatusText(panel.transform, "ResultText");

        HeroFormationTestUI testUI =
            panel.AddComponent<HeroFormationTestUI>();
        SerializedObject serialized = new SerializedObject(testUI);
        SetReference(serialized, "heroButtonRoot", heroButtonRoot);
        SetReference(serialized, "heroButtonTemplate", heroButtonTemplate);
        SetReference(serialized, "slotButtonRoot", slotButtonRoot);
        SetReference(serialized, "slotButtonTemplate", slotButtonTemplate);
        SetReference(serialized, "selectedHeroText", selectedHeroText);
        SetReference(serialized, "resultText", resultText);
        SetReference(serialized, "battleManager", battleManager);
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = CreateUIObject(name, parent);
        Image image = panel.AddComponent<Image>();
        image.color = PanelColor;

        VerticalLayoutGroup layout =
            panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(18, 18, 18, 18);
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        LayoutElement element = panel.AddComponent<LayoutElement>();
        element.minWidth = 360f;
        element.flexibleWidth = 1f;

        return panel;
    }

    private static Transform AddListRoot(
        Transform parent,
        string name,
        float preferredHeight)
    {
        GameObject root = CreateUIObject(name, parent);
        Image image = root.AddComponent<Image>();
        image.color = new Color(0.03f, 0.04f, 0.05f, 0.72f);

        VerticalLayoutGroup layout =
            root.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(8, 8, 8, 8);
        layout.spacing = 6f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        LayoutElement element = root.AddComponent<LayoutElement>();
        element.preferredHeight = preferredHeight;
        element.flexibleHeight = 0f;

        return root.transform;
    }

    private static Button AddButton(
        Transform parent,
        string name,
        string label,
        Color? color = null)
    {
        GameObject buttonObject = CreateUIObject(name, parent);
        Image image = buttonObject.AddComponent<Image>();
        image.color = color ?? ButtonColor;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.28f, 0.58f, 0.84f, 1f);
        colors.pressedColor = new Color(0.12f, 0.31f, 0.51f, 1f);
        colors.disabledColor = new Color(0.20f, 0.20f, 0.20f, 0.55f);
        button.colors = colors;

        LayoutElement element = buttonObject.AddComponent<LayoutElement>();
        element.minHeight = 42f;
        element.preferredHeight = 46f;

        TMP_Text text = AddText(
            buttonObject.transform,
            "Label",
            label,
            20f,
            46f);
        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(8f, 4f);
        textRect.offsetMax = new Vector2(-8f, -4f);

        return button;
    }

    private static TMP_Text AddStatusText(Transform parent, string name)
    {
        TMP_Text text = AddText(parent, name, string.Empty, 18f, 64f);
        text.color = new Color(0.75f, 0.84f, 0.91f, 1f);
        text.alignment = TextAlignmentOptions.TopLeft;
        return text;
    }

    private static TMP_Text AddText(
        Transform parent,
        string name,
        string value,
        float fontSize,
        float preferredHeight)
    {
        GameObject textObject = CreateUIObject(name, parent);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = true;
        text.raycastTarget = false;

        LayoutElement element = textObject.AddComponent<LayoutElement>();
        element.preferredHeight = preferredHeight;
        element.flexibleHeight = 0f;

        return text;
    }

    private static GameObject CreateUIObject(
        string name,
        Transform parent)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static T FindSceneComponent<T>(Scene scene)
        where T : Component
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            T component = root.GetComponentInChildren<T>(true);
            if (component != null)
            {
                return component;
            }
        }

        return null;
    }

    private static void SetReference(
        SerializedObject serialized,
        string propertyName,
        Object value)
    {
        SerializedProperty property =
            serialized.FindProperty(propertyName);
        property.objectReferenceValue = value;
    }
}
