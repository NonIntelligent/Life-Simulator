using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureControl : MonoBehaviour
{
    Genetics genetics;
    Attributes attributes;
    BoxCollider coll;

    public GameObject contextMenu;

    // Start is called before the first frame update
    void Start()
    {
        genetics = new Genetics();
        attributes = new Attributes(genetics);
        coll = GetComponent<Collider>() as BoxCollider;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown() {
        contextMenu.SetActive(true);
    }

    void OnDrawGizmos() {

    }
}
