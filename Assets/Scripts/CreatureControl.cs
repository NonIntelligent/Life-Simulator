using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureControl : MonoBehaviour
{
    Genetics genetics;
    public Attributes attributes;
    BoxCollider coll;
    NavMeshAgent agent;

    Genetics[] grandParents = new Genetics[4];
    Genetics[] parents = new Genetics[2];

    GameObject contextMenu;
    GameObject contextCanvas;

    // Start is called before the first frame update
    void Start()
    {
        genetics = new Genetics();
        attributes = new Attributes(genetics);
        coll = GetComponent<Collider>() as BoxCollider;
        agent = GetComponent<NavMeshAgent>();
        contextMenu = GameObject.FindGameObjectWithTag("CreatureContext");
        contextCanvas = contextMenu.transform.Find("Canvas").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        attributes.health -= 10f * Time.deltaTime;
    }

    public void UpdateSpeed(float speed)
    {
        agent.speed = speed;
        agent.acceleration = speed / 2f;
    }

    void SetupParentsAndGrandParents()
    {

    }

    void OnMouseDown() {
        contextCanvas.SetActive(true);
    }

    void OnDrawGizmos() {

    }
}
