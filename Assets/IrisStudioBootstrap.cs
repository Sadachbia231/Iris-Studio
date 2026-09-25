using UnityEngine;
using UnityEngine.UIElements;

public class IrisStudioBootstrap : MonoBehaviour
{
    [SerializeField]
    private UIDocument uiDocument;

    private void OnEnable()
    {
        var root =
            uiDocument.rootVisualElement;

        var pianoRoll =
            new IrisPianoRoll();

        pianoRoll.style.flexGrow = 1;

        root.Add(pianoRoll);
    }
}