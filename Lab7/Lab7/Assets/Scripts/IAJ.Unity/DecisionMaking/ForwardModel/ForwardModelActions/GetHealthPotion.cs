using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class GetHealthPotion : WalkToTargetAndExecuteAction
    {
        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion", character, target)
        {

        }

        public float GetGoalChange(Goal goal)
        {
            var expectedHPGain = base.Character.GameManager.characterData.MaxHP - base.Character.GameManager.characterData.HP;

            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change = expectedHPGain;
            return change;
        }

        public override bool CanExecute()
        {

            if (!base.CanExecute())
                return false;
            return true;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            return true;
        }

        public override void Execute()
        {

            base.Execute();
            this.Character.GameManager.GetHealthPotion(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var expectedHPGain = (int)worldModel.GetProperty(Properties.MAXHP) - (int)worldModel.GetProperty(Properties.HP);

            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue - expectedHPGain);


            worldModel.SetProperty(Properties.HP, (int)worldModel.GetProperty(Properties.MAXHP));

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }



        public override float GetHValue(WorldModel worldModel)
        {
            int addedHP = (int)worldModel.GetProperty(Properties.MAXHP) - (int)worldModel.GetProperty(Properties.HP);

            if (addedHP == 0)
            { // Makes no sense to try to go get a health potion when you're at max HP (i.e addedHP is 0)
                return 100f;
            }
            return base.GetHValue(worldModel) * 1 / addedHP; //The more HP we add, the smaller the HValue
        }
    }
}
