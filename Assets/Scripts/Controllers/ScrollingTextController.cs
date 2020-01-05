using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the scrolling text lines on the load screen.
/// </summary>
public class ScrollingTextController : MonoBehaviour {
    /// <summary>
    /// The text object that displays the current line in the center.
    /// </summary>
    public RectTransform currentText;

    /// <summary>
    /// The text object that is used to display the next line.
    /// </summary>
    public RectTransform nextText;

    /// <summary>
    /// The speed to scroll the text with, as percentage of the screen width per second.
    /// </summary>
    public float scrollSpeed = 0.8f;

    /// <summary>
    /// How long a line should hang in the center before continuing scrolling.
    /// </summary>
    public float scrollHangDelay = 2f;

    /// <summary>
    /// The lines to scroll through. Loaded from scrolling_lines.txt.
    /// </summary>
    private string[] lines;

    /// <summary>
    /// Whether the lines are currently scrolling.
    /// </summary>
    private bool scrolling ;

    /// <summary>
    /// The next scrolling sequence starts when the scrollHangTimer reaches the scrollHangDelay value.
    /// </summary>
    private float scrollHangTimer;

    /// <summary>
    /// The index of the currently displayed line.
    /// </summary>
    private int lineIndex;

    private void Start() {
        TextAsset data = Resources.Load<TextAsset>("scrolling_lines");
        lines = data.text.Split('\n').OrderBy(it => Random.value).ToArray();
        StartScrolling();
    }

    private void Update() {
        if (!scrolling) {
            scrollHangTimer += Time.deltaTime;
            if (scrollHangTimer >= scrollHangDelay) {
                StartScrolling();
            }
        } else {
            HandleScrolling();
        }
    }

    /// <summary>
    /// Start the new scrolling sequence.
    /// </summary>
    private void StartScrolling() {
        scrolling = true;

        // The game will probably never run out of load lines, but if it does, reset the index to prevent an exception.
        if (lineIndex >= lines.Length)
            lineIndex = 0;
        nextText.GetComponent<Text>().text = lines[lineIndex++];
        
        Vector3 nextTextPosition = nextText.position;
        nextTextPosition.x = Screen.width + nextText.rect.width / 2f;
        nextText.position = nextTextPosition;
    }
    
    /// <summary>
    /// Called every frame while a scrolling sequence is active, to move the texts.
    /// </summary>
    private void HandleScrolling() {
        currentText.Translate(-scrollSpeed * Screen.width * Time.deltaTime, 0, 0);
        nextText.Translate(-scrollSpeed * Screen.width * Time.deltaTime, 0, 0);
        if (nextText.position.x <= Screen.width / 2f) {
            Vector3 nextTextPosition = nextText.position;
            nextTextPosition.x = Screen.width / 2f;
            nextText.position = nextTextPosition;
            FinishScrolling();
        }
    }

    /// <summary>
    /// Finish the current scrolling sequence.
    /// </summary>
    private void FinishScrolling() {
        scrolling = false;
        scrollHangTimer = 0f;

        // Switch the positions of the two texts, so the previous text can be used for the next text. 
        RectTransform previousText = currentText;
        currentText = nextText;
        nextText = previousText;
    }
}