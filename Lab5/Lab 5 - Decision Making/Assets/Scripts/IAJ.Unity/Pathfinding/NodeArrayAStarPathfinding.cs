using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Grid;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class NodeArrayAStarPathfinding : AStarPathfinding
    {
        private static int index = 0;
        protected NodeRecordArray NodeRecordArray { get; set; }

        public NodeArrayAStarPathfinding(int width, int height, float cellSize, IOpenSet open, IClosedSet closed, IHeuristic heuristic) : base(width, height, cellSize, null, null, heuristic)
        {
            grid = new Grid<NodeRecord>(width, height, cellSize, (Grid<NodeRecord> global, int x, int y) => new NodeRecord(x, y, index++));
            this.InProgress = false;
            this.Heuristic = heuristic;
            this.NodesPerSearch = uint.MaxValue;

            this.NodeRecordArray = new NodeRecordArray(grid.getAll());
            this.Open = this.NodeRecordArray;
            this.Closed = this.NodeRecordArray;
            NodesPerSearch = 100;

        }
       
        protected override void ProcessChildNode(NodeRecord parentNode, NodeRecord neighbourNode)
        {
            // Check if we found a better path than the one we had before

            float f;
            float g;
            float h;

            var childNodeRecord = this.NodeRecordArray.GetNodeRecord(neighbourNode);

            g = parentNode.gCost + base.CalculateDistanceCost(parentNode, neighbourNode);
            h = 5 * this.Heuristic.H(neighbourNode, this.GoalNode);
            f = g + h;

            // If so lets update it then

            if (childNodeRecord.status == NodeStatus.Closed)
            {
                if (f <= childNodeRecord.fCost)
                {
                    childNodeRecord.gCost = g;
                    childNodeRecord.hCost = h;
                    childNodeRecord.fCost = f;
                    childNodeRecord.status = NodeStatus.Open;
                    childNodeRecord.parent = parentNode;
                    this.NodeRecordArray.AddToOpen(childNodeRecord);
                    grid.SetGridObject(childNodeRecord.x, childNodeRecord.y, childNodeRecord);

                }
            }
            else if (childNodeRecord.status == NodeStatus.Open)
            {
                if (f <= childNodeRecord.fCost)
                {
                    childNodeRecord.gCost = g;
                    childNodeRecord.hCost = h;
                    childNodeRecord.fCost = f;
                    childNodeRecord.parent = parentNode;
                    this.NodeRecordArray.Replace(this.NodeRecordArray.GetNodeRecord(neighbourNode), childNodeRecord);
                    grid.SetGridObject(childNodeRecord.x, childNodeRecord.y, childNodeRecord);
                }
            }
            else
            {
                childNodeRecord.gCost = g;
                childNodeRecord.hCost = h;
                childNodeRecord.fCost = f;
                childNodeRecord.status = NodeStatus.Open;
                childNodeRecord.parent = parentNode;
                this.NodeRecordArray.AddToOpen(childNodeRecord);
                grid.SetGridObject(childNodeRecord.x, childNodeRecord.y, childNodeRecord);
                this.MaxOpenNodes += 1;
            }
        }
               
            
        }


       
}
