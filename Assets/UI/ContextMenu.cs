using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContextMenu : MonoBehaviour
{
    GameObject canvasChild;

    GraphicRaycaster graphicRayCast;
    public GraphicRaycaster[] otherCanvases;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    Vector3 Collision = Vector3.zero;
    Vector3 Origin = Vector3.zero;
    // Raycast works on every layer except for Ignore Raycast and UI
    int layerMask = ~((1 << 2) | (1 << 5));

    // Start is called before the first frame update
    void Start()
    {
        canvasChild = transform.GetChild(0).gameObject;
        graphicRayCast = canvasChild.GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
    }

    void FixedUpdate() {
        // use a raycast to determine to detect creature and populate the panel with data
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 300f, layerMask)) {
                Origin = ray.origin;
                Collision = hit.point;

                // Hit a creature
                GameObject obj = hit.transform.gameObject;
                if (hit.collider.isTrigger && hit.transform.gameObject.CompareTag("Creature")) {
                    canvasChild.SetActive(true);
                    var control = canvasChild.GetComponentInChildren<AttributePanelControl>();
                    control.SetCreature(obj);
                    var att = obj.GetComponent<CreatureControl>().attributes;
                    control.SetAttributes(att);
                    control.DisplayAttributeValues(0, ref att.health, att.maxHealth);
                    control.DisplayAttributeValues(1, ref att.saturation, att.maxSaturation);
                    control.DisplayAttributeValues(2, ref att.energy, att.maxEnergy);
                    return;
                }
            }

            // Graphics raycast for UI to disable it
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;
            int totalCount = 0;

            List<RaycastResult> results = new List<RaycastResult>();

            graphicRayCast.Raycast(pointerEventData, results);
            totalCount += results.Count;

            // Iterate through other graphic raycasters 
            for (int g = 0; g < otherCanvases.Length; g++) {
                results.Clear();
                otherCanvases[g].Raycast(pointerEventData, results);
                totalCount+= results.Count;
            }

            // Hit nothing related to UI so disable canvas
            if (totalCount == 0) { 
                canvasChild.SetActive(false);
            }


        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Collision, 0.3f);
        Gizmos.DrawRay(Origin, Collision - Origin);
    }
}
