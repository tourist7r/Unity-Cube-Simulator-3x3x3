/*
 * This class is resposnible for the animation of dropdown mechanism used to select the rotation speed
 * */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropDown : MonoBehaviour {
    public RectTransform container;
    public bool isOpen;
    public CubeController cube;

	// Use this for initialization
	void Start () {
	    container = transform.FindChild("Container").GetComponent<RectTransform>();
        isOpen = false;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 scale = container.localScale;
        scale.y = Mathf.Lerp(scale.y, isOpen ? 1 : 0, Time.deltaTime * 12);
        container.localScale = scale;
	}

    public void toggleDropDown() {
        isOpen = !isOpen;
    }

    public void setSpeed(int index) {
        cube.selectedItemIndex = index;
    }

    public void setHighlighted(Button btn) {
        foreach (Button c in container.GetComponentsInChildren<Button>()) {
            c.image.color = Color.white;
        }
        btn.image.color = HexToColor("65B5EEFF");
        isOpen = false;
    }

    Color HexToColor(string hex) {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 230);
    }
}
