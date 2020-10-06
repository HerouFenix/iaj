using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidCharacter : DynamicMovement
    {
        public override string Name
        {
            get { return "Avoid Character"; }
        }

        public float MaxTimeLookAhead { get; set; }
        public float AvoidMargin { get; set; }


        public DynamicAvoidCharacter(KinematicData target)
        {
            this.Target = target;
            this.Output = new MovementOutput();
        }

        public override MovementOutput GetMovement()
        {
            this.Output.Clear();
            
            return this.Output;
            
        }
    }
}
