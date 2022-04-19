using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class AttributePanelControl : MonoBehaviour
{
    List<TMP_InputField> inputFields = new List<TMP_InputField>();
    List<TMP_Text> textFields = new List<TMP_Text>();
    List<Slider> sliders = new List<Slider>();

    List<GameObject> gameObjects = new List<GameObject>();

    public GameObject panelComponent;
    public namePair[] panelObjectNames;

    private GameObject currentCreature = null;
    private Attributes att = null;

    private bool editingInputField = false;
    private int InputFieldIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        // instantiates all attribute ui components to control stats
        for (int i = 0; i < panelObjectNames.Length; i++) {
            var obj = Instantiate(panelComponent);
            obj.transform.SetParent(transform, false);

            var inputField = obj.GetComponentInChildren<TMP_InputField>();
            var textField = obj.GetComponentInChildren<TMP_Text>();
            var slider = obj.GetComponentInChildren<Slider>();

            inputField.contentType = panelObjectNames[i].contentType;
            inputField.characterLimit = 10;
            textField.text = panelObjectNames[i].name;

            // Add listeners for inputs (integer necessary to avoid referencing)
            int listenIndex = i;
            inputField.onEndEdit.AddListener(delegate { UpdateSliderValue(listenIndex); });
            inputField.onSelect.AddListener(delegate { EditingInputField(listenIndex); });
            slider.onValueChanged.AddListener(delegate { UpdateDisplayValue(listenIndex); });

            // Add objects to lists
            gameObjects.Add(obj);
            inputFields.Add(inputField);
            textFields.Add(textField);
            sliders.Add(slider);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (att != null) {
            for (int i = 0; i < panelObjectNames.Length; i++) {
                float[] statValues = att.GetIndexStatLink(i);
                sliders[i].maxValue = statValues[1];
                sliders[i].SetValueWithoutNotify(statValues[0]);

                if (!editingInputField || i != InputFieldIndex) {
                    inputFields[i].text = statValues[0].ToString(); 
                }
                
            }
        }
    }

    public void SetCreature(GameObject creature) {
        currentCreature = creature;
    }

    public void SetAttributes(Attributes attributes) {
        att = attributes;
    }

    // update the text and starting values in the ui
    public void DisplayAttributeValues(int index, ref float value, float maxValue) {
        if (index >= sliders.Count) return;

        inputFields[index].text = value.ToString();
        sliders[index].minValue = 0f;
        sliders[index].maxValue = maxValue;
        sliders[index].SetValueWithoutNotify(value);
    }

    // update the slider value and attribute on input field enter
    void UpdateSliderValue(int index) {
        editingInputField = false;
        InputFieldIndex = -1;

        float input = float.Parse(inputFields[index].text);
        input = Mathf.Clamp(input, sliders[index].minValue, sliders[index].maxValue);
        sliders[index].SetValueWithoutNotify(input);
        inputFields[index].text = input.ToString();
        att.UpdateCurrentStats(index, input);

    }
    // Update the input field value and attribute when slider changes
    void UpdateDisplayValue(int index) {
        inputFields[index].text = sliders[index].value.ToString();
        att.UpdateCurrentStats(index, sliders[index].value);
    }

    void EditingInputField(int index) {
        editingInputField = true;
        InputFieldIndex = index;
    }

}

[System.Serializable]
public struct namePair { public string name; public TMP_InputField.ContentType contentType; }
