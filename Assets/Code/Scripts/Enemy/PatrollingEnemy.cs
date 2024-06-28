using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : Enemy
{
    [SerializeField] private Transform[] _patrolingTargets;
    [SerializeField] private float _speed;
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

        // Using vec2 to ignore the y difference
        Vector2 vecToTarget = new Vector2(
            _patrolingTargets[_patrolingIndex].position.x - transform.position.x,
            _patrolingTargets[_patrolingIndex].position.z - transform.position.z);

        if (vecToTarget.sqrMagnitude < 0.1f)
        {
            _patrolingIndex++;
            _patrolingIndex = _patrolingIndex % _patrolingTargets.Length;
        }
    }
}