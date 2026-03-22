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
    NavMeshAgent agent;

    public enum ActionState { IDLE, WORKING};
    ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.RUNNING;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        
        
        tree = new BehaviorTree();
        Sequence steal = new Sequence("Steal Something");
        Leaf goToDiamond = new Leaf("Go to Diamond", GoToDiamond);
        Leaf goTobackDoor = new Leaf("Go to Backdoor", GoToBackDoor);
        Leaf goTofrontDoor = new Leaf("Go to Frontdoor", GoToFrontDoor);
        Leaf goToVan = new Leaf("Go to Van", GoToVan);
        Selector opendoor = new Selector("Open Door");

        opendoor.AddChild(goTofrontDoor);
        opendoor.AddChild(goTobackDoor);
        
        
        steal.AddChild(opendoor);
        steal.AddChild(goToDiamond);
        //steal.AddChild(goTobackDoor);
        steal.AddChild(goToVan);
        tree.AddChild(steal);

        tree.PrintTree();

        
        
    }

    public Node.Status GoToDiamond()
    {
        return GoToLocation(diamond.transform.position);
    }
    
    public Node.Status GoToBackDoor()
    {
        return GoToLocation(backDoor.transform.position);
    }
    public Node.Status GoToFrontDoor()
    {
        return GoToLocation(frontDoor.transform.position);
    }
    public Node.Status GoToVan()
    {
        return GoToLocation(van.transform.position);
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
        if (treeStatus == Node.Status.RUNNING) 
        treeStatus = tree.Process();
    }
}
