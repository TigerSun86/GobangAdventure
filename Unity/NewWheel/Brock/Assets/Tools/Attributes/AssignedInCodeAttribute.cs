using UnityEngine;

/// <summary>
/// Marks a field that is assigned in code, not in the editor.
/// Adds a label icon to indicate that.
/// </summary>
public class AssignedInCodeAttribute : PropertyAttribute
{
    public string icon;

    /// <param name="icon">Optional custom icon (e.g., "🔧", "🧠")</param>
    public AssignedInCodeAttribute(string icon = "🛠️")
    {
        this.icon = icon;
    }
}
