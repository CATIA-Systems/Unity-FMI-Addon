using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenProjectButtonBehaviour : MonoBehaviour {

	private void Start()
	{
        var button = GetComponent<Button>();
        button.onClick.AddListener(OpenProject);
	}

	public void OpenProject() {
        System.Diagnostics.Process.Start("https://github.com/CATIA-Systems/Unity-FMI-Addon");
    }

}
