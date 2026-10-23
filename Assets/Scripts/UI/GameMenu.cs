using System.Collections.Generic;
using MyUnityGame.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace MyUnityGame.UI
{
    public class GameMenu : MonoBehaviour
    {
        private const string LayoutResourcePath = "UI/GameMenuLayout";
        private const string ThemeResourcePath = "UI/GameMenuTheme";
        private const string SelectedEntryClass = "menu-button--selected";

        private readonly List<IGameMenuPage> pages = new List<IGameMenuPage>
        {
            new PlaceholderMenuPage("Journal"),
            new PartyMenuPage(),
            new PlaceholderMenuPage("Character"),
            new PlaceholderMenuPage("Bag"),
            new PlaceholderMenuPage("Save"),
        };

        private readonly Dictionary<IGameMenuPage, Button> entryButtons = new Dictionary<IGameMenuPage, Button>();

        private PanelSettings panelSettings;
        private VisualElement menuRoot;
        private VisualElement pagePanel;
        private VisualElement pageContent;
        private Label pageTitle;
        private Button firstEntry;
        private Button backButton;
        private IGameMenuPage openPage;
        private bool isOpen;

        private void Awake()
        {
            var layout = Resources.Load<VisualTreeAsset>(LayoutResourcePath);
            var theme = Resources.Load<ThemeStyleSheet>(ThemeResourcePath);
            if (layout == null || theme == null)
            {
                Debug.LogError(
                    $"Game menu assets are missing from Resources/{LayoutResourcePath} or Resources/{ThemeResourcePath}.",
                    this);
                enabled = false;
                return;
            }

            panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            panelSettings.name = "GameMenuPanelSettings";
            panelSettings.themeStyleSheet = theme;
            panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            panelSettings.referenceResolution = new Vector2Int(1280, 720);
            panelSettings.match = 0.5f;

            var document = gameObject.AddComponent<UIDocument>();
            document.panelSettings = panelSettings;
            document.visualTreeAsset = layout;

            var root = document.rootVisualElement;
            menuRoot = root.Q<VisualElement>("menu-root");
            pagePanel = root.Q<VisualElement>("page");
            pageContent = root.Q<VisualElement>("page-content");
            pageTitle = root.Q<Label>("page-title");
            backButton = root.Q<Button>("page-back");
            backButton.clicked += ShowEntries;

            BuildEntries(root.Q<VisualElement>("entry-list"));
            SetVisible(menuRoot, false);
        }

        private void BuildEntries(VisualElement entryList)
        {
            foreach (var page in pages)
            {
                var entry = CreateEntry(page.Title, () => ShowPage(page));
                entryButtons.Add(page, entry);
                entryList.Add(entry);
                firstEntry ??= entry;
            }

            entryList.Add(CreateEntry("Close", Close));
        }

        private static Button CreateEntry(string label, System.Action onClick)
        {
            var entry = new Button(onClick) { text = label };
            entry.AddToClassList("menu-button");
            return entry;
        }

        private void Update()
        {
            var gameManager = GameManager.Instance;
            if (isOpen && gameManager != null && !gameManager.IsMenuOpen)
            {
                HideMenu();
            }

            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.tabKey.wasPressedThisFrame)
            {
                if (isOpen)
                {
                    Close();
                }
                else
                {
                    Open();
                }
            }
        }

        public void Open()
        {
            var gameManager = GameManager.Instance;
            if (isOpen || (gameManager != null && !gameManager.SetMenuOpen(true)))
            {
                return;
            }

            isOpen = true;
            SetVisible(menuRoot, true);
            ShowEntries();
        }

        public void Close()
        {
            if (!isOpen)
            {
                return;
            }

            GameManager.Instance?.SetMenuOpen(false);
            HideMenu();
        }

        private void HideMenu()
        {
            isOpen = false;
            openPage = null;
            SetVisible(menuRoot, false);
        }

        private void ShowEntries()
        {
            SelectEntry(null);
            SetVisible(pagePanel, false);
            FocusLater(firstEntry);
        }

        private void ShowPage(IGameMenuPage page)
        {
            SelectEntry(page);
            pageTitle.text = page.Title;
            pageContent.Clear();
            page.Build(pageContent);
            SetVisible(pagePanel, true);
            FocusLater(backButton);
        }

        private void SelectEntry(IGameMenuPage page)
        {
            if (openPage != null && entryButtons.TryGetValue(openPage, out var previous))
            {
                previous.RemoveFromClassList(SelectedEntryClass);
            }

            openPage = page;
            if (page != null && entryButtons.TryGetValue(page, out var current))
            {
                current.AddToClassList(SelectedEntryClass);
            }
        }

        // Focus has to wait a frame for an element that was just made visible.
        private static void FocusLater(VisualElement element)
        {
            element?.schedule.Execute(element.Focus);
        }

        private static void SetVisible(VisualElement element, bool visible)
        {
            element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnDestroy()
        {
            if (isOpen)
            {
                GameManager.Instance?.SetMenuOpen(false);
            }

            if (panelSettings != null)
            {
                Destroy(panelSettings);
            }
        }
    }
}
