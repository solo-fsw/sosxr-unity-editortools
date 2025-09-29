using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace EventVisualizer.Base
{
    [Serializable]
    public class EventsGraph : Graph
    {
        private GameObject[] selectedRoots;
        private bool searchingHierarchy;
        private const int WARNING_CALLS_THRESOLD = 1000;


        public static EventsGraph Create()
        {
            var graph = CreateInstance<EventsGraph>();
            graph.hideFlags = HideFlags.HideAndDontSave;

            return graph;
        }


        public EventsGraphGUI GetEditor()
        {
            var gui = CreateInstance<EventsGraphGUI>();
            gui.graph = this;
            gui.hideFlags = HideFlags.HideAndDontSave;

            return gui;
        }


        public void RebuildGraph(GameObject[] roots, bool searchHierarchy)
        {
            selectedRoots = roots;
            searchingHierarchy = searchHierarchy;
            BuildGraph(selectedRoots, searchHierarchy);
            SortGraph(nodes, false);
        }


        public void RefreshGraphConnections()
        {
            var positions = new Dictionary<string, Rect>();
            var adriftNodes = new List<Node>();

            foreach (NodeGUI node in nodes)
            {
                positions.Add(node.name, node.position);
            }

            BuildGraph(selectedRoots, searchingHierarchy);

            foreach (NodeGUI node in nodes)
            {
                if (positions.ContainsKey(node.name))
                {
                    node.position = positions[node.name];
                }
                else
                {
                    adriftNodes.Add(node);
                }
            }

            SortGraph(adriftNodes, true);
        }


        private void BuildGraph(GameObject[] roots, bool searchHierarchy)
        {
            NodeData.ClearAll();
            Clear(true);
            var calls = EventsFinder.FindAllEvents(roots, searchHierarchy);

            if (calls.Count > WARNING_CALLS_THRESOLD)
            {
                var goAhead = EditorUtility.DisplayDialog("Confirm massive graph",
                    "You are about to generate a graph with " + calls.Count + " events.\n"
                    + "Tip: You can select some gameobjects and search events in just those or their children instead.",
                    "Go ahead",
                    "Abort");

                if (goAhead)
                {
                    GenerateGraphFromCalls(calls);
                }
            }
            else
            {
                GenerateGraphFromCalls(calls);
            }
        }


        private void GenerateGraphFromCalls(List<EventCall> calls)
        {
            foreach (var call in calls)
            {
                NodeData.RegisterEvent(call);
            }

            foreach (var data in NodeData.Nodes)
            {
                var node = NodeGUI.Create(data);

                if (!nodes.Contains(node))
                {
                    AddNode(node);
                }
            }

            foreach (NodeGUI node in nodes)
            {
                node.PopulateEdges();
            }
        }


        #region sorting

        private HashSet<Node> positionedNodes = new();
        private const float VERTICAL_SPACING = 80f;
        private const float HORIZONTAL_SPACING = 400f;


        private void SortGraph(List<Node> nodes, bool skipParents)
        {
            positionedNodes.Clear();

            var sortedNodes = new List<Node>(nodes); //cannot sort the original collection so a clone is needed

            sortedNodes.Sort((x, y) =>
            {
                var xScore = x.outputEdges.Count() - x.inputEdges.Count();
                var yScore = y.outputEdges.Count() - y.inputEdges.Count();

                return yScore.CompareTo(xScore);
            });

            var position = Vector2.zero;

            foreach (var node in sortedNodes)
            {
                if (!positionedNodes.Contains(node))
                {
                    positionedNodes.Add(node);
                    position.y += PositionNodeHierarchy(node, position, skipParents);
                }
            }
        }


        private float PositionNodeHierarchy(Node currentNode, Vector2 masterPosition, bool skipParents)
        {
            var height = VERTICAL_SPACING;

            if (!skipParents)
            {
                foreach (var outputEdge in currentNode.outputEdges)
                {
                    var node = outputEdge.toSlot.node;

                    if (!positionedNodes.Contains(node))
                    {
                        positionedNodes.Add(node);

                        height += PositionNodeHierarchy(node, masterPosition
                                                              + Vector2.right * HORIZONTAL_SPACING
                                                              + Vector2.up * height, skipParents);
                    }
                }
            }

            currentNode.position = new Rect(masterPosition + Vector2.up * height * 0.5f, currentNode.position.size);

            return height;
        }

        #endregion
    }
}