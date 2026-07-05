using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static CodeMonkey.CSharpCourse.Interactive.CodeMonkeyInteractiveSO;

namespace CodeMonkey.CSharpCourse.Interactive {

    public class MainWindow : EditorWindow {


        private const string UPDATE_COMPANION_PROJECT_URL = "https://unitycodemonkey.teachable.com/courses/problem-solving/lectures/63536137";


        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private VisualTreeAsset lectureSingleVisualTreeAsset;
        [SerializeField] private VisualTreeAsset lectureHeaderVisualTreeAsset;
        [SerializeField] private VisualTreeAsset textTemplateVisualTreeAsset;
        [SerializeField] private VisualTreeAsset codeTemplateVisualTreeAsset;
        [SerializeField] private VisualTreeAsset videoTemplateVisualTreeAsset;
        [SerializeField] private VisualTreeAsset objectiveTogglePrefabVisualTreeAsset;


        private enum SubWindow {
            MainMenu,
            LectureList,
            Lecture,
        }

        private VisualElement mainMenuVisualElement;
        private VisualElement liveChatContainer;
        private double nextCheckLiveChatStateTime;


        [MenuItem("Code Monkey/Game Dev Practice Lab", priority = 0)]
        public static void ShowWindow() {
            MainWindow window = GetWindow<MainWindow>();
            window.titleContent = new GUIContent("Game Dev Practice Lab");
        }

        public static void DestroyChildren(VisualElement containerVisualElement) {
            foreach (VisualElement child in containerVisualElement.Children().ToList()) {
                containerVisualElement.Remove(child);
            }
        }

        public static void AddComplexText(
            VisualTreeAsset textTemplateVisualTreeAsset,
            VisualTreeAsset codeTemplateVisualTreeAsset,
            VisualTreeAsset videoTemplateVisualTreeAsset,
            VisualElement containerVisualElement,
            string text) {
            // Break down complex text and add all components

            // ##REF##video_small, KGFAnwkO0Pk, What are Value Types and Reference Types in C#? (Class vs Struct)##REF##
            // ##REF##code, Console.WriteLine("Qwerty");##REF##

            // Parse HTML
            text = text.Replace("<h1>", "<size=20>");
            text = text.Replace("</h1>", "</size>");
            text = text.Replace("<strong>", "<b>");
            text = text.Replace("</strong>", "</b>");
            text = text.Replace("<p>", "<br>");
            text = text.Replace("</p>", "");

            string refTag = "##REF##";
            string textRemaining = text;
            int safety = 0;
            while (textRemaining.IndexOf(refTag) != -1 && safety < 100) {
                // Found Ref Tag
                int refTagIndex = textRemaining.IndexOf(refTag);

                // Add before text
                string textBefore = textRemaining.Substring(0, refTagIndex);
                AddText(textTemplateVisualTreeAsset, containerVisualElement, textBefore);

                string refData = textRemaining.Substring(refTagIndex + refTag.Length);
                refData = refData.Substring(0, refData.IndexOf(refTag));

                textRemaining = textRemaining.Substring(refTagIndex + refTag.Length);
                textRemaining = textRemaining.Substring(textRemaining.IndexOf(refTag) + refTag.Length);

                string[] refDataArray = refData.Split(',');
                string refType = refDataArray[0].Trim();
                switch (refType) {
                    case "video_small":
                        string youTubeId = refDataArray[1].Trim();
                        string youTubeTitle = refDataArray[2].Trim();
                        string thumbnailUrl = $"https://img.youtube.com/vi/{youTubeId}/mqdefault.jpg";
                        AddVideoReference(videoTemplateVisualTreeAsset, containerVisualElement, thumbnailUrl, youTubeTitle, "https://www.youtube.com/watch?v=" + youTubeId);
                        break;
                    case "code":
                        AddCode(codeTemplateVisualTreeAsset, containerVisualElement, refData.Substring(refType.Length + 1).Trim());
                        break;
                }
                safety++;
            }
            // No more Ref tags found
            AddText(textTemplateVisualTreeAsset, containerVisualElement, textRemaining);
        }

        public static void AddText(VisualTreeAsset textTemplateVisualTreeAsset, VisualElement containerVisualElement, string text) {
            VisualElement textVisualElement = textTemplateVisualTreeAsset.Instantiate();

            Label textLabel = textVisualElement.Q<Label>("textLabel");
            textLabel.text = text;

            containerVisualElement.Add(textVisualElement);
        }

        public static void AddCode(VisualTreeAsset codeTemplateVisualTreeAsset, VisualElement containerVisualElement, string codeString) {
            VisualElement codeVisualElement = codeTemplateVisualTreeAsset.Instantiate();

            Label textLabel = codeVisualElement.Q<Label>("codeLabel");
            textLabel.text = codeString;

            containerVisualElement.Add(codeVisualElement);
        }

        public static void AddVideoReference(VisualTreeAsset videoTemplateVisualTreeAsset, VisualElement containerVisualElement, string imageUrl, string title, string url, VideoReferenceSettings videoReferenceSettings = null) {
            Sprite waitingSprite = null;
            VisualElement videoVisualElement = AddVideoReference(videoTemplateVisualTreeAsset, containerVisualElement, waitingSprite, title, url, videoReferenceSettings);

            UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(imageUrl);
            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                    } else {
                        DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                        VisualElement imageVisualElement = videoVisualElement.Q<VisualElement>("image");
                        imageVisualElement.style.backgroundImage = new StyleBackground(downloadHandlerTexture.texture);
                    }
                } catch (Exception) {
                }
                unityWebRequest.Dispose();
            };
        }

        public static VisualElement AddVideoReference(VisualTreeAsset videoTemplateVisualTreeAsset, VisualElement containerVisualElement, Sprite sprite, string title, string url, VideoReferenceSettings videoReferenceSettings = null) {
            VisualElement videoVisualElement = videoTemplateVisualTreeAsset.Instantiate();

            VisualElement videoContainer = videoVisualElement.Q<VisualElement>("videoContainer");
            videoContainer.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                Debug.Log("Clicked: " + url);
                Application.OpenURL(url);
            });

            VisualElement imageVisualElement = videoContainer.Q<VisualElement>("image");
            imageVisualElement.style.backgroundImage = new StyleBackground(sprite);

            Label textLabel = videoContainer.Q<Label>("titleLabel");
            textLabel.text = title;

            if (videoReferenceSettings != null) {
                if (videoReferenceSettings.height != null) {
                    imageVisualElement.style.height = new StyleLength(videoReferenceSettings.height.Value);
                }
                if (videoReferenceSettings.fontSize != null) {
                    textLabel.style.fontSize = new StyleLength(videoReferenceSettings.fontSize.Value);
                }
            }

            containerVisualElement.Add(videoVisualElement);

            return videoVisualElement;
        }

        public class VideoReferenceSettings {
            public float? height;
            public float? fontSize;
        }

        private SubWindow GetActiveSubWindow() {
            return SubWindow.MainMenu;
        }

        public void OnDestroy() {
            CodeMonkeyInteractiveSO.GetCodeMonkeyInteractiveSO().OnStateChanged -= CodeMonkeyInteractiveSO_OnStateChanged;
        }

        private void CodeMonkeyInteractiveSO_OnStateChanged(object sender, EventArgs e) {
            switch (GetActiveSubWindow()) {
                case SubWindow.MainMenu:
                    ShowMainMenu();
                    break;
            }
        }

        public void CreateGUI() {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            CodeMonkeyInteractiveSO.GetCodeMonkeyInteractiveSO().OnStateChanged -= CodeMonkeyInteractiveSO_OnStateChanged;
            CodeMonkeyInteractiveSO.GetCodeMonkeyInteractiveSO().OnStateChanged += CodeMonkeyInteractiveSO_OnStateChanged;

            // Instantiate UXML
            VisualElement rootVisualTreeAsset = visualTreeAsset.Instantiate();
            rootVisualTreeAsset.style.flexGrow = 1f;
            root.Add(rootVisualTreeAsset);

            mainMenuVisualElement = root.Q<VisualElement>("mainMenu");

            liveChatContainer = root.Q<VisualElement>("liveChatContainer");
            liveChatContainer.style.display = DisplayStyle.None;
            liveChatContainer.RegisterCallback((ClickEvent clickEvent) => {
                LiveChat.ShowWindow();
            });

            Toggle objective1 = root.Q<VisualElement>("objectivesContainer").Children().ToList()[0].Q<Toggle>();
            Toggle objective2 = root.Q<VisualElement>("objectivesContainer").Children().ToList()[1].Q<Toggle>();
            Toggle objective3 = root.Q<VisualElement>("objectivesContainer").Children().ToList()[2].Q<Toggle>();
            Toggle objective4 = root.Q<VisualElement>("objectivesContainer").Children().ToList()[3].Q<Toggle>();
            Toggle objective5 = root.Q<VisualElement>("objectivesContainer").Children().ToList()[4].Q<Toggle>();

            Toggle objectiveBonus1 = root.Q<VisualElement>("bonusObjectivesContainer").Children().ToList()[0].Q<Toggle>();
            Toggle objectiveBonus2 = root.Q<VisualElement>("bonusObjectivesContainer").Children().ToList()[1].Q<Toggle>();
            //Toggle objectiveBonus3 = root.Q<VisualElement>("bonusObjectivesContainer").Children().ToList()[2].Q<Toggle>();

            objective1.value = CodeMonkeyInteractiveSO.GetObjectiveCheckboxState().objective1;
            objective2.value = CodeMonkeyInteractiveSO.GetObjectiveCheckboxState().objective2;
            objective3.value = CodeMonkeyInteractiveSO.GetObjectiveCheckboxState().objective3;
            objective4.value = CodeMonkeyInteractiveSO.GetObjectiveCheckboxState().objective4;
            objective5.value = CodeMonkeyInteractiveSO.GetObjectiveCheckboxState().objective5;
            objectiveBonus1.value = CodeMonkeyInteractiveSO.GetObjectiveCheckboxState().bonusObjective1;
            objectiveBonus2.value = CodeMonkeyInteractiveSO.GetObjectiveCheckboxState().bonusObjective2;
            //objectiveBonus3.value = CodeMonkeyInteractiveSO.GetObjectiveCheckboxState().bonusObjective3;

            objective1.RegisterCallback<ChangeEvent<bool>>((changeEvent) => {
                GetObjectiveCheckboxState().objective1 = changeEvent.newValue;
                SetObjectiveCheckboxState(GetObjectiveCheckboxState());
            });
            objective2.RegisterCallback<ChangeEvent<bool>>((changeEvent) => {
                GetObjectiveCheckboxState().objective2 = changeEvent.newValue;
                SetObjectiveCheckboxState(GetObjectiveCheckboxState());
            });
            objective3.RegisterCallback<ChangeEvent<bool>>((changeEvent) => {
                GetObjectiveCheckboxState().objective3 = changeEvent.newValue;
                SetObjectiveCheckboxState(GetObjectiveCheckboxState());
            });
            objective4.RegisterCallback<ChangeEvent<bool>>((changeEvent) => {
                GetObjectiveCheckboxState().objective4 = changeEvent.newValue;
                SetObjectiveCheckboxState(GetObjectiveCheckboxState());
            });
            objective5.RegisterCallback<ChangeEvent<bool>>((changeEvent) => {
                GetObjectiveCheckboxState().objective5 = changeEvent.newValue;
                SetObjectiveCheckboxState(GetObjectiveCheckboxState());
            });
            objectiveBonus1.RegisterCallback<ChangeEvent<bool>>((changeEvent) => {
                GetObjectiveCheckboxState().bonusObjective1 = changeEvent.newValue;
                SetObjectiveCheckboxState(GetObjectiveCheckboxState());
            });
            objectiveBonus2.RegisterCallback<ChangeEvent<bool>>((changeEvent) => {
                GetObjectiveCheckboxState().bonusObjective2 = changeEvent.newValue;
                SetObjectiveCheckboxState(GetObjectiveCheckboxState());
            });

            root.Q<Label>("versionLabel").text = CodeMonkeyInteractiveSO.GetCodeMonkeyInteractiveSO().currentVersion;

            ShowMainMenu();
        }

        private void ShowMainMenu() {
            mainMenuVisualElement.style.display = DisplayStyle.Flex;

            // Always hide version tab, not checking for updates on Game Mechanic Challenge
            mainMenuVisualElement.Q<VisualElement>("checkingForUpdates").style.display = DisplayStyle.None;

            // Check for updates
            CodeMonkeyInteractiveSO.CheckForUpdates((string currentVersion, string newVersion) => {
                /*
                if (currentVersion == newVersion) {
                    mainMenuVisualElement.Q<VisualElement>("checkingForUpdates").style.display = DisplayStyle.None;
                    return;
                }

                VisualElement checkingForUpdatesVisualElement =
                    mainMenuVisualElement.Q<VisualElement>("checkingForUpdates");
                checkingForUpdatesVisualElement.style.display = DisplayStyle.Flex;
                Label textLabel = checkingForUpdatesVisualElement.Q<Label>();
                textLabel.text = "New version available!\n" +
                    currentVersion + " -> " + newVersion + "\n" +
                    "<u>Click here!</u>";

                textLabel.RegisterCallback((ClickEvent clickEvent) => {
                    string url = UPDATE_COMPANION_PROJECT_URL;
                    Application.OpenURL(url);
                });
                */
            });


            // Dynamic Message
            VisualElement dynamicMessageVisualElement =
                mainMenuVisualElement.Q<VisualElement>("dynamicMessage");

            Func<string> getDynamicMessageURL = () => "https://unitycodemonkey.com/";
            dynamicMessageVisualElement.RegisterCallback((ClickEvent clickEvent) => {
                Application.OpenURL(getDynamicMessageURL());
            });

            CodeMonkeyInteractiveSO.GetWebsiteDynamicMessage((WebsiteDynamicMessage websiteDynamicMessage) => {
                dynamicMessageVisualElement.Q<Label>("messageLabel").text = websiteDynamicMessage.text;
                getDynamicMessageURL = () => websiteDynamicMessage.url;
            });


            // Latest Videos
            VisualElement latestVideosVisualElement =
                mainMenuVisualElement.Q<VisualElement>("latestVideos");

            latestVideosVisualElement.Q<VisualElement>("_1Container").Clear();
            latestVideosVisualElement.Q<VisualElement>("_2Container").Clear();
            latestVideosVisualElement.Q<VisualElement>("_3Container").Clear();
            latestVideosVisualElement.Q<VisualElement>("_4Container").Clear();



            CodeMonkeyInteractiveSO.GetWebsiteLatestVideos((LatestVideos latestVideos) => {
                AddLatestVideoReference(latestVideos.videos[0], latestVideosVisualElement.Q<VisualElement>("_1Container"));
                AddLatestVideoReference(latestVideos.videos[1], latestVideosVisualElement.Q<VisualElement>("_2Container"));
                AddLatestVideoReference(latestVideos.videos[2], latestVideosVisualElement.Q<VisualElement>("_3Container"));
                AddLatestVideoReference(latestVideos.videos[3], latestVideosVisualElement.Q<VisualElement>("_4Container"));
            });

            void AddLatestVideoReference(LatestVideoSingle latestVideoSingle, VisualElement containerVisualElement) {
                string thumbnailUrl = $"https://img.youtube.com/vi/{latestVideoSingle.youTubeId}/mqdefault.jpg";
                string url = $"https://unitycodemonkey.com/video.php?v={latestVideoSingle.youTubeId}";
                AddVideoReference(
                    videoTemplateVisualTreeAsset,
                    containerVisualElement,
                    thumbnailUrl,
                    latestVideoSingle.title,
                    url,
                    new VideoReferenceSettings {
                        height = 80,
                        fontSize = 9,
                    }
                );
            }
            /*
            if (CodeMonkeyInteractiveSO.TryLoad()) {
                ShowMainMenu();
            }*/

            CheckLiveChatOnline();
        }

        private void CheckLiveChatOnline() {
            if (!CodeMonkeyInteractiveSO.HasInternetConnection()) {
                return;
            }
            CodeMonkeyInteractiveSO.ContactWebsiteLiveChatGetCodeMonkeyState((bool isOnline, string offlineReason) => {
                // Success!
                if (isOnline) {
                    liveChatContainer.style.display = DisplayStyle.Flex;
                } else {
                    liveChatContainer.style.display = DisplayStyle.None;
                }
            },
                (string other) => {
                    Debug.Log("OTHER: " + other);
                },
                (string error) => {
                    Debug.Log("ERROR: " + error);
                }
            );
        }

        private void Update() {
            if (EditorApplication.timeSinceStartup > nextCheckLiveChatStateTime) {
                double nextGetMessagesTimeAdd = 3f;
                nextCheckLiveChatStateTime = EditorApplication.timeSinceStartup + nextGetMessagesTimeAdd;

                CheckLiveChatOnline();
            }
        }




        private void TESTING_OnLectureListButtonClick() {
            //Debug.Log("Lecture List Button");

            //PrintAllTitles();
            //WriteFileFAQData();
            //WriteFileQuizData();

            //ParseFAQJson();
            //ParseQuizJson();
        }


    }





}