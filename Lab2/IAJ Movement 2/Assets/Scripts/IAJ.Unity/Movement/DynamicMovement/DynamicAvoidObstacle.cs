using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidObstacle : DynamicSeek
    {
        public override string Name
        {
            get { return "Avoid Obstacle"; }
        }

        private GameObject obstacle;

        public GameObject Obstacle
        {
            get { return this.obstacle; }
            set
            {
                this.obstacle = value;
                this.ObstacleCollider = value.GetComponent<Collider>();
            }
        }

        private Collider ObstacleCollider { get; set; }
        public float MaxLookAhead { get; set; }

        public float AvoidMargin { get; set; }

        public float FanAngle { get; set; }

        public DynamicAvoidObstacle(GameObject obstacle)
        {
            this.Obstacle = obstacle;
            this.Target = new KinematicData();
        }

        public override MovementOutput GetMovement()
        {
            RaycastHit hit;
            Color color = Color.white;

            Collider col = this.Obstacle.GetComponent<Collider>();

            Ray mainRay = new Ray(this.Character.Position, this.Character.velocity.normalized);

            /* ASK HOW TO CREATE WHISKERS */
            Vector3 leftWhisker = (Quaternion.Euler(0, -45, 0) * this.Character.velocity);
            Vector3 rightWhisker = (Quaternion.Euler(0, 45, 0) * this.Character.velocity);

            Ray leftRay = new Ray(this.Character.Position, leftWhisker.normalized);
            Ray rightRay = new Ray(this.Character.Position, rightWhisker.normalized);

            Debug.DrawRay(this.Character.Position, new Vector3(1.0f, 0.0f, 1.0f), color);

            //Check Collisions
            if (col.Raycast(mainRay, out hit, this.MaxLookAhead) || col.Raycast(leftRay, out hit, this.MaxLookAhead/2) || col.Raycast(rightRay, out hit, this.MaxLookAhead/2))
            {
                //Debug.Log("HIT");
                base.Target.Position = hit.point + hit.normal * this.AvoidMargin;
                return base.GetMovement();

            }
            else
            {
                return new MovementOutput();
            }

        }
    }
}
