using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicWander : DynamicSeek
    {
        public DynamicWander()
        {
            this.Target = new KinematicData();
        }
        public override string Name
        {
            get { return "Wander"; }
        }
        public float TurnAngle { get; set; }
        public float WanderOffset { get; set; }
        public float WanderRadius { get; set; }
        public float WanderRate { get; set; }
        protected float WanderOrientation { get; set; }
        public Vector3 CircleCenter { get; private set; }
        public GameObject DebugTarget { get; set; }

        public override MovementOutput GetMovement()
        {
            // I wander (get it) if there's something missing here...
            this.TurnAngle += RandomHelper.RandomBinomial() * this.WanderRate;

            this.WanderOrientation = this.TurnAngle + this.Character.orientation;

            this.CircleCenter = this.Character.position + this.WanderOffset * MathHelper.ConvertOrientationToVector(this.Character.orientation);

            base.Target.position = this.CircleCenter + this.WanderRadius * MathHelper.ConvertOrientationToVector(this.WanderOrientation);

            // Debug.Log("Target Pos: " + base.Target.position);
            ////////////////////////

            if (this.DebugTarget != null)
            {
                this.DebugTarget.transform.position = this.Target.position;
            }

            return base.GetMovement();
        }
    }
}
