using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class ShieldOfFaith : Action
    {
        public AutonomousCharacter Character { get; private set; }

        public ShieldOfFaith(AutonomousCharacter character) : base("ShieldOfFaith")
        {
            this.Character = character;
        }

        public override bool CanExecute()
        {
            return this.Character.GameManager.characterData.Mana >= 5;
        }


        public override bool CanExecute(WorldModel worldModel)
        {
            return (int)worldModel.GetProperty(Properties.MANA) >= 5;
        }

        public override void Execute()
        {
            this.Character.GameManager.ShieldOfFaith();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var expectedHPGain = 5 - (int)worldModel.GetProperty(Properties.ShieldHP);

            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue - expectedHPGain);

            worldModel.SetProperty(Properties.ShieldHP, 5);
            worldModel.SetProperty(Properties.MANA, (int)worldModel.GetProperty(Properties.MANA)-5);
        }

        public float GetGoalChange(Goal goal)
        {
            var expectedHPGain = 5 - this.Character.GameManager.characterData.ShieldHP;

            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change -= expectedHPGain;
            return change;
        }

        public override float GetHValue(WorldModel worldModel)
        {
            int addedHP = 5 - (int)worldModel.GetProperty(Properties.ShieldHP);

            if (addedHP == 0)
            { // Makes no sense to try to go get a health potion when you're at max shield HP (i.e addedHP is 0)
                return 100f;
            }
            return base.GetHValue(worldModel) * 1 / addedHP; //The more HP we add, the smaller the HValue
        }
    }
}
