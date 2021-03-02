namespace LGOAPDemo
{
    public unsafe delegate byte LGOAPTransitionApplicabilityCheckDelegate(void* blackboard);

    public unsafe delegate float LGOAPGoalInsistenceCheckDelegate(void* blackboard);

    public unsafe delegate ushort LGOAPGoalHeuristicCheckDelegate(void* blackboard);

    public unsafe delegate float LGOAPCostCheckDelegate(void* blackboard);

    public unsafe delegate void LGOAPModificationDelegate(void* blackboard);
}