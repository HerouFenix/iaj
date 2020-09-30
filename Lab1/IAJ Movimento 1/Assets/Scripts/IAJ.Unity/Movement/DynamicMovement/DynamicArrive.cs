using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {
        public float StopRadius { get; set; }
        public float SlowRadius { get; set; }

        public DynamicArrive()
        {
            base.MovingTarget = new KinematicData();
        }

        public override string Name
        {
            get { return "Arrive"; }
        }

        public override MovementOutput GetMovement()
        {
            Vector3 direction = this.Target.position - this.Character.position;
            float distance = direction.magnitude;

            // By default we'll stop
            float desiredSpeed = 0;

            // If we're outside the slow radius go at max speed
            if (distance > this.SlowRadius)
            {
                desiredSpeed = this.MaxAcceleration;
                Debug.Log("Going at full speed");
            } // If we're inside the slow radius but outside the stop radius, slow down
            else if (distance >= this.StopRadius)
            {
                desiredSpeed = this.MaxAcceleration * (distance / this.SlowRadius);
                Debug.Log("Slowing down");
            }

            base.MovingTarget.velocity = direction.normalized * desiredSpeed;

            return base.GetMovement();
        }
    }
}