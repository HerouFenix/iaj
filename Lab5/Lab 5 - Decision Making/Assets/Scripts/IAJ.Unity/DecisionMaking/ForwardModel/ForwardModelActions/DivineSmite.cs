using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class DivineSmite : WalkToTargetAndExecuteAction
    {
        private float expectedXPChange;
        private int xpChange;

        public DivineSmite(AutonomousCharacter character, GameObject target) : base("DivineSmite", character, target)
        {
            this.xpChange = 3;
            this.expectedXPChange = 3f;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL)
            {
                change += -this.expectedXPChange;
            }

            return change;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.DivineSmite(this.Target);
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return base.CanExecute() && (int)worldModel.GetProperty(Properties.MANA) >= 2;
        }

        protected override float GetDuration(Vector3 currentPosition)
        {
            var distance = (Target.transform.position - currentPosition).magnitude;

            return distance / this.Character.Character.MaxSpeed - 10f;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            int xp = (int)worldModel.GetProperty(Properties.XP);


            //there was an hit, enemy is destroyed, gain xp
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);

            worldModel.SetProperty(Properties.XP, xp + this.xpChange);
            var xpValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_LEVEL_GOAL, xpValue - this.xpChange);

            // Reduce Mana
            worldModel.SetProperty(Properties.MANA, (int)worldModel.GetProperty(Properties.MANA)-2);

        }

        public override float GetHValue(WorldModel worldModel)
        {
            var position = (Vector3)worldModel.GetProperty(Properties.POSITION);

            var distance = (this.Target.transform.position - position).magnitude - 10f;
            //  var distance = this.Character.AStarPathFinding.Heuristic.H(position, this.Target.transform.position);
            return distance * 1 / 25.0f;
        }
    }
}
