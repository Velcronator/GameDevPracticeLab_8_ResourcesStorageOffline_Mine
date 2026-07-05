using CodeMonkey.Utils;
using System;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeMonkey.CSharpCourse.Interactive {

    public class LiveChat : EditorWindow {


        [SerializeField] private VisualTreeAsset visualTreeAsset = default;



        private TextField inputTextField;
        private Label onlineLabel;
        private Label chatLabel;
        private FunctionTimer.FunctionTimerObject inputFocusFunctionTimerObject;
        private long lastTimestamp;
        private double nextGetMessagesTime;



        [MenuItem("Code Monkey/Live Chat", priority = 200)]
        public static void ShowWindow() {
            LiveChat liveChat = GetWindow<LiveChat>();
            liveChat.titleContent = new GUIContent("Code Monkey Live Chat");
        }

        public void CreateGUI() {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement rootVisualTreeAsset = visualTreeAsset.Instantiate();
            root.Add(rootVisualTreeAsset);

            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = CodeMonkeyInteractiveSO.GetCodeMonkeyInteractiveSO();

            VisualElement onlineContainer = rootVisualTreeAsset.Q<VisualElement>("onlineContainer");

            TextField emailTextField = rootVisualTreeAsset.Q<TextField>("emailTextField");
            Button loginButton = rootVisualTreeAsset.Q<Button>("loginButton");
            loginButton.RegisterCallback((ClickEvent clickEvent) => {
                string studentEmail = emailTextField.value.Trim();
                if (studentEmail != "") {
                    CodeMonkeyInteractiveSO.SetStudentEmail(studentEmail);
                    UpdateLoginState();
                }
            });

            UpdateLoginState();

            onlineLabel = rootVisualTreeAsset.Q<Label>("onlineLabel");
            onlineLabel.text = "---";

            inputTextField = rootVisualTreeAsset.Q<TextField>("inputTextField");
            inputTextField.Focus();

            chatLabel = rootVisualTreeAsset.Q<Label>("chatLabel");

            Button sendMessageButton = rootVisualTreeAsset.Q<Button>("sendMessageButton");
            sendMessageButton.RegisterCallback((ClickEvent clickEvent) => {
                SendMessage();
            });

            inputTextField.RegisterCallback((KeyDownEvent keyDownEvent) => {
                if (keyDownEvent.keyCode == KeyCode.Return || keyDownEvent.keyCode == KeyCode.KeypadEnter) {
                    SendMessage();

                    inputFocusFunctionTimerObject =
                        FunctionTimer.CreateObject(() => {
                            inputTextField.Focus(); 
                        }, .1f);
                }
            }, TrickleDown.TrickleDown);
        }

        private void UpdateLoginState() {
            VisualElement loginContainer = rootVisualElement.Q<VisualElement>("loginContainer");
            VisualElement chatContainer = rootVisualElement.Q<VisualElement>("chatContainer");

            if (string.IsNullOrEmpty(CodeMonkeyInteractiveSO.GetStudentEmail())) {
                // No email, show login
                loginContainer.style.display = DisplayStyle.Flex;
                chatContainer.style.display = DisplayStyle.None;
            } else {
                // Has email, show chat
                loginContainer.style.display = DisplayStyle.None;
                chatContainer.style.display = DisplayStyle.Flex;

                rootVisualElement.Q<Label>("usedEmailLabel").text = CodeMonkeyInteractiveSO.GetStudentEmail();
            }
        }

        private void Update() {
            if (inputFocusFunctionTimerObject != null) {
                if (inputFocusFunctionTimerObject.Update()) {
                    inputFocusFunctionTimerObject = null;
                }
            }

            if (EditorApplication.timeSinceStartup > nextGetMessagesTime) {
                double nextGetMessagesTimeAdd = 1f;
                nextGetMessagesTime = EditorApplication.timeSinceStartup + nextGetMessagesTimeAdd;

                GetMessages();
                RefreshOnlineState();
            }
        }

        private void RefreshOnlineState() {
            if (!CodeMonkeyInteractiveSO.HasInternetConnection()) {
                return;
            }

            CodeMonkeyInteractiveSO.ContactWebsiteLiveChatGetCodeMonkeyState((bool isOnline, string offlineReason) => {
                // Success!
                if (isOnline) {
                    onlineLabel.text = "I'm <u>ONLINE</u> right now!\r\nNeed help with anything? Ask me!";
                    onlineLabel.style.color = Color.green;
                } else {
                    onlineLabel.text = "I'm <u>OFFLINE</u> right now! Be back when I can!\nAlso remember how you can post questions in the Lecture comments.\n(" + offlineReason + ")";
                    onlineLabel.style.color = Color.grey;
                }
            },
                (string other) => {
                    Debug.Log("OTHER: " + other);
                    AddText("OTHER: " + other);
                },
                (string error) => {
                    Debug.Log("ERROR: " + error);
                    AddText("ERROR: " + error);
                }
            );
        }

        private void SendMessage() {
            if (!CodeMonkeyInteractiveSO.HasInternetConnection()) {
                return;
            }

            string message = inputTextField.value;
            if (!string.IsNullOrEmpty(message)) {
                inputTextField.value = "";

                Debug.Log("Sending Chat Message: " + message);
                //AddText("<b>" + CodeMonkeyInteractiveSO.GetStudentEmail() + ":</b> " + message);

                CodeMonkeyInteractiveSO.ContactWebsiteLiveChatSendMessage(message, () => {
                    // Success!
                    },
                    (string other) => {
                        Debug.Log("OTHER: " + other);
                        AddText("OTHER: " + other);
                    },
                    (string error) => {
                        Debug.Log("ERROR: " + error);
                        AddText("ERROR: " + error);
                    }
                );
            }
        }

        private void GetMessages() {
            if (!CodeMonkeyInteractiveSO.HasInternetConnection()) {
                return;
            }

            CodeMonkeyInteractiveSO.ContactWebsiteLiveChatGetMessages(lastTimestamp, 
                (CodeMonkeyInteractiveSO.LiveChatGetMessagesResponse liveChatGetMessagesResponse) => {
                    // Success!
                    for (int i = liveChatGetMessagesResponse.messageArray.Length - 1; i >= 0; i--) {
                        CodeMonkeyInteractiveSO.LiveChatGetMessagesSingle liveChatGetMessagesSingle = liveChatGetMessagesResponse.messageArray[i];
                        string messageComplete = liveChatGetMessagesSingle.message;
                        if (liveChatGetMessagesSingle.isReply) {
                            messageComplete = "<b>Code Monkey:</b> " + messageComplete;
                        } else {
                            messageComplete = "<b>" + CodeMonkeyInteractiveSO.GetStudentEmail() + ":</b> " + messageComplete;
                        }
                        AddText(messageComplete);

                        if (liveChatGetMessagesSingle.timestamp > lastTimestamp) {
                            lastTimestamp = liveChatGetMessagesSingle.timestamp;
                        }
                    }
                },
                (string other) => {
                    Debug.Log("OTHER: " + other);
                    AddText("OTHER: " + other);
                },
                (string error) => {
                    Debug.Log("ERROR: " + error);
                    AddText("ERROR: " + error);
                }
            );
        }


        private void AddText(string text) {
            chatLabel.text = text + "\n" + chatLabel.text;
        }

    }

}