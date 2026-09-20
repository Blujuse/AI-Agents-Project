using UnityEngine;
using UnityEngine.AI;

using BehaviourTree;
using System.Collections.Generic;
using System.Linq;

// Follow schecdule node for animal behaviour tree, animals daily routine
public class FollowShedule : Node
{
    [Header("Animal References")]
    private NavMeshAgent _agent;
    private WalkingArea _walkingArea;
    private AnimalKnowledge _animalKnowledge;

    [Header("Activity Management")]
    private List<ActivityManager> _activities;

    // Class to manage activities
    private class ActivityManager
    {
        public float startTime;
        public System.Action activityAction;
        public string debugText;
    }

    // Constructor for FollowShedule
    public FollowShedule(NavMeshAgent agent, WalkingArea walkingArea, AnimalKnowledge animalKnowledge)
    {
        _agent = agent;
        _walkingArea = walkingArea;
        _animalKnowledge = animalKnowledge;

        // Define the daily activities based on animal knowledge
        _activities = new List<ActivityManager>
        {
            new() { startTime = animalKnowledge.activityTimes[0], activityAction = SetRandomDest, debugText = "Early morning wandering" },
            new() { startTime = animalKnowledge.activityTimes[1], activityAction = GoToCatchingSpot, debugText = "Going to bug catching spot" },
            new() { startTime = animalKnowledge.activityTimes[2], activityAction = GoPicnicing, debugText = "Going picnicing" },
            new() { startTime = animalKnowledge.activityTimes[3], activityAction = GoToShops, debugText = "Going to shops" },
            new() { startTime = animalKnowledge.activityTimes[4], activityAction = GoHome, debugText = "Going home" },
            new() { startTime = animalKnowledge.activityTimes[5], activityAction = BeachWalk, debugText = "Beach walk" },
            new() { startTime = animalKnowledge.activityTimes[6], activityAction = GoHome, debugText = "Going home for the night" },
            new() { startTime = animalKnowledge.activityTimes[7], activityAction = GoHome, debugText = "Staying home for the night" },
            new() { startTime = animalKnowledge.activityTimes[8], activityAction = SetRandomDest, debugText = "Early morning wandering" }
        };

        // Sort activities by start time
        _activities = _activities.OrderBy(a => a.startTime).ToList();
    }

    bool HasArrived()
    {
        return _agent.remainingDistance <= _agent.stoppingDistance;
    }

    void SetRandomDest()
    {
        _animalKnowledge.currentActivityArea = null;
        _agent.SetDestination(_walkingArea.GetRandomPoint());
    }

    // Helpers for activities
    // Explore within the current activity area
    void ExploreArea(ActivityArea activeArea)
    {
        if (!IsInActivityArea()) return; // Only explore if in the activity area

        // Get a random point within the activity area bounds
        Vector3 randomPoint = new Vector3(
            Random.Range(activeArea.areaBounds.min.x, activeArea.areaBounds.max.x),
            _agent.transform.position.y,
            Random.Range(activeArea.areaBounds.min.z, activeArea.areaBounds.max.z)
            );

        // Find a valid NavMesh position near the random point
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position); // Set the agent's destination to the valid position
            return;
        }
    }

    // Go to the closest activity area from a list
    void GoToActivityArea(IEnumerable<ActivityArea> activityAreas)
    {
        _animalKnowledge.currentActivityArea = null; // Reset current activity area
        float closestDistance = Mathf.Infinity; // Initialize closest distance

        foreach (ActivityArea area in activityAreas) // Iterate through activity areas
        {
            if (area == null)
                continue; // Skip null areas
            float distance = _agent.remainingDistance; // Get distance to area
            if (distance < closestDistance)
            {
                closestDistance = distance; // Update closest distance
                _animalKnowledge.currentActivityArea = area; // Set current activity area
                _agent.SetDestination(area.transform.position); // Set agent destination to area
            }
        }

        ExploreArea(_animalKnowledge.currentActivityArea); // Explore within the selected activity area
    }

    // Activity methods
    // Using the helper to go to specific activity areas
    void GoToCatchingSpot()
    {
        GoToActivityArea(_animalKnowledge.animalData.bugCatchingSpots);
    }

    void GoPicnicing()
    {
        GoToActivityArea(_animalKnowledge.animalData.picnicSpots);
    }

    void BeachWalk()
    {
        GoToActivityArea(_animalKnowledge.animalData.beachSpots);
    }

    // More specific activity methods
    // Very similar to the helper but for a specific transform list
    void GoToShops()
    {
        GoToActivityArea(_animalKnowledge.animalData.shopSpots);
        /*
        _animalKnowledge.currentActivityArea = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform trans in _animalKnowledge.animalData.shopSpots)
        {
            if (trans == null)
                continue;

            float distance = _agent.remainingDistance;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                _agent.SetDestination(trans.transform.position);
            }
        }
        */
    }

    // Go home to animals individual home area
    void GoHome()
    {
        _animalKnowledge.currentActivityArea = null;

        _animalKnowledge.currentActivityArea = _animalKnowledge.homeArea;
        _agent.SetDestination(_animalKnowledge.homeArea.transform.position);

        ExploreArea(_animalKnowledge.currentActivityArea);
    }

    // Evaluate method to determine current activity based on time of day
    public override NodeState Evaluate()
    {
        // Go back to moving
        if (_agent.isStopped) _agent.isStopped = false;

        float currentTime = _animalKnowledge.GetTimeOfDay();
        ActivityManager currentActivity = _activities.LastOrDefault(a => a.startTime <= currentTime) ?? _activities.First();

        if (HasArrived())
        {
            currentActivity.activityAction.Invoke();
        }
        else
        {
            // Re-trigger the destination if not reached, after greeting
            if (!_agent.hasPath || _agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                currentActivity.activityAction.Invoke();
            }
        }

        state = NodeState.RUNNING;
        return state;
    }

    // Check if the agent is within the current activity area
    bool IsInActivityArea()
    {
        return _animalKnowledge.currentActivityArea.areaBounds.Contains(_agent.transform.position);
    }
}