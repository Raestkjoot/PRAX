using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolingEnem : Enemy
{
    [SerializeField] private Transform[] _patrolingTargets;
    private int _patrolingIndex = 0;

    // move from patroling target to patroling target..
    protected override void Update()
    {
        if (base._fov.DetectedPlayer)
        {
            base.Update();
            return;
        }

        if (base._actionState != ActionState.Idle)
        {
            base.Update();
            return;
        }

        base.SetFOV();

        base._navAgent.SetDestination(_patrolingTargets[_patrolingIndex].position);

        Vector3 vecToTarget = _patrolingTargets[_patrolingIndex].position - transform.position;

        if (vecToTarget.sqrMagnitude < 0.1f)
        {
            _patrolingIndex++;
            _patrolingIndex = _patrolingIndex % _patrolingTargets.Length;
        }
    }
}