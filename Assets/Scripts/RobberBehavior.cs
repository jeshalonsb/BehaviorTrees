using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehavior : MonoBehaviour
{
    BehaviorTree tree;

    public GameObject diamond;
    public GameObject van;
    public GameObject backDoor;
    public GameObject frontDoor;
    public GameObject monaLisa;
    NavMeshAgent agent;

    public enum ActionState { IDLE, WORKING};
    ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.RUNNING;

    [Range(0, 1000)]
    public int money = 800;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        
        
        tree = new BehaviorTree();
        Sequence steal = new Sequence("Steal Something");
        Leaf goToDiamond = new Leaf("Go to Diamond", GoToDiamond);
        Leaf hasGotMoney = new Leaf("Has got Money", HasMoney);
        Leaf goToMonaLisa = new Leaf("Go to Diamond", GoToMonaLisa);
        Leaf goTobackDoor = new Leaf("Go to Backdoor", GoToBackDoor);
        Leaf goTofrontDoor = new Leaf("Go to Frontdoor", GoToFrontDoor);
        Leaf goToVan = new Leaf("Go to Van", GoToVan);
        Selector opendoor = new Selector("Open Door");

        opendoor.AddChild(goTofrontDoor);
        opendoor.AddChild(goTobackDoor);

        steal.AddChild(hasGotMoney);
        steal.AddChild(opendoor);
        steal.AddChild(goToDiamond);
        steal.AddChild(goToMonaLisa);
        //steal.AddChild(goTobackDoor);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        tree.PrintTree();

        
        
    }
    public Node.Status HasMoney()
    {
        if (money >= 500)
            return Node.Status.FALIURE;
        return Node.Status.SUCCESS;
    }
    public Node.Status GoToDiamond()
    {
        Node.Status s = GoToLocation(diamond.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            diamond.transform.parent = this.gameObject.transform;
        }
        return s;
        
    }
    public Node.Status GoToMonaLisa()
    {
        Node.Status s = GoToLocation(monaLisa.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            monaLisa.transform.parent = this.gameObject.transform;
        }
        return s;

    }

    public Node.Status GoToBackDoor()
    {
        return GoToDoor(backDoor);
    }
    public Node.Status GoToFrontDoor()
    {
        return GoToDoor(frontDoor);
    }
    public Node.Status GoToVan()
    {
        Node.Status s = GoToLocation(van.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            money += 300;
            diamond.SetActive(false);
            monaLisa.SetActive(false);
        }
        return s;
    }
    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status s = GoToLocation(door.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>().isLocked)
            {
                door.SetActive(false);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FALIURE;
        }
        else
            return s;
    }

    Node.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FALIURE;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }   
    
    void Update()
    {
        if (treeStatus != Node.Status.SUCCESS) 
        treeStatus = tree.Process();
    }
}
