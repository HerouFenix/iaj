namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public abstract class DynamicMovement
    {
        public abstract string Name { get; }
        public KinematicData Character { get; set; }
        public abstract KinematicData Target { get; set; }
        public float MaxAcceleration { get; set; }

        public DynamicMovement()
        {
            
        }
        public abstract MovementOutput GetMovement();

    }
}
