using UnityEngine;
using System.Collections;

public class DropdownClickHandler : MonoBehaviour {

    public OptionsPanel panel;
    public string functionName;

    void OnClick() {
        panel.Invoke(functionName, 0);
    }
}
