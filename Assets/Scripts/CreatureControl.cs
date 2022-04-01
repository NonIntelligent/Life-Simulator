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
    public CreatureFamily family { get; set; }

    // Reuse same ui menu but change the values
    static GameObject contextMenu;
    static GameObject contextCanvas;

    // Start is called before the first frame update
    void Start()
    {
        family = CreatureFamily.NATURE;
        genetics = new Genetics();
        attributes = new Attributes(genetics);
        transform.localScale = new Vector3(1f, 1f, 1f) * attributes.size;
        coll = GetComponent<Collider>() as BoxCollider;
        agent = GetComponent<NavMeshAgent>();
        if (contextMenu == null)
        {
        contextMenu = GameObject.FindGameObjectWithTag("CreatureContext");
        contextCanvas = contextMenu.transform.Find("Canvas").gameObject;
        }
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
