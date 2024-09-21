using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("Movement")]
    [TaskDescription("Rotate the cannon towards a designer-specified target position")]

    public class RotateToTarget : Action
    {
        private EnemyCannon _myCannon;
        private bool _startedRotation;

        public override void OnStart()
        {
            _myCannon = gameObject.GetComponentInChildren<EnemyCannon>(true);

            _startedRotation = false;
        }

        public override TaskStatus OnUpdate()
        {
            // No cannon found
            if (!_myCannon)
            {
                return TaskStatus.Failure;
            }
            // Rotation has started
            else if (_startedRotation)
            {
                // Rotation is in progress
                if (_myCannon.Rotating)
                {
                    return TaskStatus.Running;
                }
                // Rotation is finished
                else
                {
                    return TaskStatus.Success;
                }
            }
            // Waiting for another rotation to finish
            else if (_myCannon.Rotating)
            {
                return TaskStatus.Running;
            }
            // Begin rotation
            else
            {
                Vector3 target = CannonTargets.GetTarget();

                _startedRotation = true;

                _myCannon.Rotate(target);

                return TaskStatus.Running;
            }
        }

    }
}


