using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace AISystem.Runtime.Tactics
{
    public struct TacticalPointData
    {
        public Vector3 Position;
        public Vector3 Normal;
        public int CoverType;
        public bool IsOccupied;
    }

    public class TacticalPointManager : MonoBehaviour
    {
        public static TacticalPointManager Instance { get; private set; }

        private readonly List<TacticalPoint> _points = new();

        public List<TacticalPoint> AllPoints => _points;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Gather existing points in the scene that might have missed registration due to execution order
                var scenePoints = FindObjectsOfType<TacticalPoint>();
                foreach (var pt in scenePoints)
                {
                    RegisterPoint(pt);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RegisterPoint(TacticalPoint point)
        {
            if (!_points.Contains(point))
            {
                _points.Add(point);
            }
        }

        public void UnregisterPoint(TacticalPoint point)
        {
            _points.Remove(point);
        }

        public NativeArray<TacticalPointData> GetPointData(Allocator allocator, GameObject queryingAgent)
        {
            var dataArray = new NativeArray<TacticalPointData>(_points.Count, allocator);
            for (int i = 0; i < _points.Count; i++)
            {
                var pt = _points[i];
                // A point is considered occupied if occupied by someone else, but not by the querying agent itself.
                bool occupiedByOther = pt.IsOccupied && pt.Occupier != queryingAgent;
                dataArray[i] = new TacticalPointData
                {
                    Position = pt.Position,
                    Normal = pt.Normal,
                    CoverType = (int)pt.Type,
                    IsOccupied = occupiedByOther
                };
            }
            return dataArray;
        }

        public void ReservePoint(TacticalPoint point, GameObject agent)
        {
            if (point != null)
            {
                point.Occupier = agent;
            }
        }

        public void ReleasePoint(TacticalPoint point, GameObject agent)
        {
            if (point != null && point.Occupier == agent)
            {
                point.Occupier = null;
            }
        }

        public void ReleaseAllPointsForAgent(GameObject agent)
        {
            foreach (var pt in _points)
            {
                if (pt.Occupier == agent)
                {
                    pt.Occupier = null;
                }
            }
        }
    }
}
