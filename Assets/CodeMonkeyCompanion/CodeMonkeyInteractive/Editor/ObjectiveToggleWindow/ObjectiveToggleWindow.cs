using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectiveToggleWindow : EditorWindow {

    [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;


    public static void ShowExample() {
        ObjectiveToggleWindow wnd = GetWindow<ObjectiveToggleWindow>();
        wnd.titleContent = new GUIContent("ObjectiveToggleWindow");
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
    }

}
