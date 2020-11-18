using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class ShieldOfFaith : WalkToTargetAndExecuteAction
    {

        public ShieldOfFaith(AutonomousCharacter character) : base("ShieldOfFaith", character)
        {
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change -= 5.0f;
            return change;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return base.CanExecute() && (int)worldModel.GetProperty(Properties.MANA) >= 5;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.ShieldOfFaith();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);

            int curShieldHP = (int)worldModel.GetProperty(Properties.ShieldHP);
            int extraHP = (int)worldModel.GetProperty(Properties.MAXHP) - curShieldHP;
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, goalValue - extraHP);

            // Set hp to max hp
            worldModel.SetProperty(Properties.ShieldHP, 5);

            // Reduce mana
            worldModel.SetProperty(Properties.MANA, (int)worldModel.GetProperty(Properties.MANA)-5);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            return base.GetHValue(worldModel);
        }
    }
}
