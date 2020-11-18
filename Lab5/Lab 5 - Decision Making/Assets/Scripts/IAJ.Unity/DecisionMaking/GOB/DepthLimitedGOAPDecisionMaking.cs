using Assets.Scripts.GameManager;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 3;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModel InitialWorldModel { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth { get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.Goals = goals;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionPerLevel = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.MaxValue;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {
            var processedActions = 0;
            var startTime = Time.realtimeSinceStartup;

            var currentValue = 0.0f;

            while (this.CurrentDepth >= 0)
            {
                // Calculate discontentment
                currentValue = this.Models[this.CurrentDepth].CalculateDiscontentment(this.Goals);

                // Check if at max depth
                if (this.CurrentDepth >= MAX_DEPTH)
                {
                    // New action combo generated, increment debug
                    processedActions++;

                    // If current value is the best, store it
                    if (currentValue < this.BestDiscontentmentValue)
                    {
                        this.BestDiscontentmentValue = currentValue;
                        this.BestAction = this.ActionPerLevel[0];

                        // New Best action found, set best action sequence
                        this.BestActionSequence = this.ActionPerLevel;
                    }

                    // Check if we processed the maximum number of actions per frame
                    if(processedActions >= this.ActionCombinationsProcessedPerFrame)
                    {
                        this.TotalActionCombinationsProcessed += processedActions;
                        this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
                        return null;
                    }

                    //We're done at this depth, drop back
                    this.CurrentDepth--;
                    continue;
                }

                var nextAction = this.Models[this.CurrentDepth].GetNextAction();
                
                if (nextAction != null)
                {
                    // Copy the current model
                    this.Models[this.CurrentDepth + 1] = this.Models[this.CurrentDepth].GenerateChildWorldModel();

                    // Apply action to copy
                    this.ActionPerLevel[this.CurrentDepth] = nextAction;
                    nextAction.ApplyActionEffects(this.Models[this.CurrentDepth + 1]);

                    // Process it on the next iteration
                    this.CurrentDepth++;
                }
                else
                {
                    // No action to try - We're done at this level
                    this.CurrentDepth--;
                }
            }

            this.TotalActionCombinationsProcessed += processedActions;
            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
            this.InProgress = false;
            return this.BestAction;
        }



    }
}

