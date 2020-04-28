using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;   //this thing is new! we need it to access all NavMesh related Components!

//This is a helper script that will set the destination of our NavMeshAgent to a Gameobject's positon
//We assume that the NavMeshAgent is on the PlayerData Object, a Parent of this Object

public class SetNavMeshTarget : MonoBehaviour
{
    
    public Transform navMeshTarget; //THe NavMeshAgent should always navigate to this target

    public NavMeshAgent myAgent;           //we need a connection to this, because we want to change its destination


    //Get our connection to the NacMeshAgent Script
    private void Start()
    {
        myAgent = GetComponentInParent<NavMeshAgent>();
    }

    //check if we are close to the destination, otherwise, set our Destination again!
    //this is a bit lazy. But it takes of updating the destination, 
    //no matter if our navMeshTarget or our NavMeshAgent have moved
    private void Update()
    {
        //check if we actually have a GameObject assigned as a NavMeshTarget before we start using it
        if (!navMeshTarget)
        {
            return;
        }

        if (Vector3.Distance(navMeshTarget.position, myAgent.destination) > 0.1f)
        {
            myAgent.SetDestination(navMeshTarget.position);
        }   
    }




}
