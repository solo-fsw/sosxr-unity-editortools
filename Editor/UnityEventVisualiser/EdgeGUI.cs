using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace EventVisualizer.Base
{
    // Specialized edge drawer class
    public class EdgeGUI : IEdgeGUI
    {
        public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var u = 1.0f - t;
            var tt = t * t;
            var uu = u * u;
            var uuu = uu * u;
            var ttt = tt * t;

            var p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }


        #region Public members

        public GraphGUI host { get; set; }
        public List<int> edgeSelection { get; set; }


        public EdgeGUI()
        {
            edgeSelection = new List<int>();
        }

        #endregion

        #region IEdgeGUI implementation

        public void DoEdges()
        {
            // Draw edges on repaint.
            if (Event.current.type == EventType.Repaint)
            {
                foreach (var edge in host.graph.edges)
                {
                    if (edge == _moveEdge)
                    {
                        continue;
                    }

                    var indexes = FindSlotIndexes(edge);
                    DrawEdge(edge, indexes, ColorForIndex(edge.fromSlotName));
                }
            }
        }


        private Vector2Int FindSlotIndexes(Edge edge)
        {
            var indexes = Vector2Int.zero;

            var totalOutputs = 0;
            var found = false;

            foreach (var slot in edge.fromSlot.node.outputSlots)
            {
                if (slot != edge.fromSlot && !found)
                {
                    indexes.x++;
                }
                else
                {
                    found = true;
                }

                totalOutputs++;
            }

            indexes.x = totalOutputs - indexes.x - 1;

            foreach (var slot in edge.toSlot.node.inputSlots)
            {
                if (slot != edge.toSlot)
                {
                    indexes.y++;
                }
                else
                {
                    break;
                }
            }

            return indexes;
        }


        public static Color ColorForIndex(string name)
        {
            var hash = Math.Abs(Animator.StringToHash(name));

            return Color.HSVToRGB((float)(hash / (double)int.MaxValue), 1f, 1f);
        }


        public void DoDraggedEdge()
        {
        }


        public void BeginSlotDragging(Slot slot, bool allowStartDrag, bool allowEndDrag)
        {
        }


        public void SlotDragging(Slot slot, bool allowEndDrag, bool allowMultiple)
        {
        }


        public void EndSlotDragging(Slot slot, bool allowMultiple)
        {
        }


        public void EndDragging()
        {
        }


        public Edge FindClosestEdge()
        {
            return null;
        }

        #endregion

        #region Private members

        private Edge _moveEdge;
        private Slot _dragSourceSlot;
        private Slot _dropTarget;

        #endregion

        #region Edge drawer

        public const float kEdgeWidth = 6;


        private static void DrawEdge(Edge edge, Vector2Int indexes, Color color)
        {
            var p1 = GetPositionAsFromSlot(edge.fromSlot, indexes.x);
            var p2 = GetPositionAsToSlot(edge.toSlot, indexes.y);
            DrawEdge(p1, p2, color * edge.color, EdgeTriggersTracker.GetTimings(edge));
        }


        private static void DrawEdge(Vector2 p1, Vector2 p2, Color color, List<float> triggers)
        {
            var prevColor = Handles.color;
            Handles.color = color;

            var l = Mathf.Min(Mathf.Abs(p1.y - p2.y), 50);
            var p3 = p1 + new Vector2(l, 0);
            var p4 = p2 - new Vector2(l, 0);
            var texture = (Texture2D)Styles.selectedConnectionTexture.image;
            Handles.DrawBezier(p1, p2, p3, p4, color, texture, kEdgeWidth);


            foreach (var trigger in triggers)
            {
                var pos = CalculateBezierPoint(trigger, p1, p3, p4, p2);
                Handles.DrawSolidArc(pos, Vector3.back, pos + Vector3.up, 360, kEdgeWidth);
            }

            Handles.color = prevColor;
        }

        #endregion

        #region Utilities to access private members

        private const float kEdgeBottomMargin = 4;
        private const float kNodeTitleSpace = 36;
        private const float kNodeEdgeSeparation = 12;


        private static Vector2 GetPositionAsFromSlot(Slot slot, int index)
        {
            var node = slot.node as NodeGUI;
            var pos = node.position.position;

            pos.y = node.position.yMax - kEdgeBottomMargin;
            pos.y -= kNodeEdgeSeparation * index;

            pos.x = node.position.xMax;

            return pos;
        }


        private static Vector2 GetPositionAsToSlot(Slot slot, int index)
        {
            var node = slot.node as NodeGUI;
            var pos = node.position.position;
            pos.y += kNodeTitleSpace;
            pos.y += kNodeEdgeSeparation * index;
            pos.x = node.position.x;

            return pos;
        }

        #endregion
    }
}