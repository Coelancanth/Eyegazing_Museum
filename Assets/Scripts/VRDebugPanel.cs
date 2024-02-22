using UnityEngine;
using TMPro; // Make sure to include this if you're using TextMeshPro

public class VRDebugPanel : MonoBehaviour
{
    public TextMeshProUGUI debugText; // Assign this in the inspector

    void Update()
    {
        // Example debug information
        debugText.text = $"FPS: {1 / Time.deltaTime:N2}\n";
        // Add any other debug information you need here, for example:
        // debugText.text += $"Player Pos: {Player.transform.position}\n";
    }
}
