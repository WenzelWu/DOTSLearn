using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace LearnJobs
{
    public class FindNearest : MonoBehaviour
    {
        private NativeArray<float3> TargetPositions;
        private NativeArray<float3> SeekerPositions;
        private NativeArray<float3> NearestTargetPositions;

        public void Start()
        {
            Spawner spawner = FindObjectOfType<Spawner>();
            TargetPositions = new NativeArray<float3>(spawner.NumTargets, Allocator.Persistent);
            SeekerPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
            NearestTargetPositions = new NativeArray<float3>(spawner.NumSeekers, Allocator.Persistent);
        }

        public void OnDestroy()
        {
            TargetPositions.Dispose();
            SeekerPositions.Dispose();
            NearestTargetPositions.Dispose();
        }

        public void Update()
        {
            for(int i = 0; i < TargetPositions.Length; i++)
            {
                TargetPositions[i] = Spawner.TargetTransform[i].localPosition;
            }
            
            for(int i = 0; i < SeekerPositions.Length; i++)
            {
                SeekerPositions[i] = Spawner.SeekerTransform[i].localPosition;
            }

            SortJob<float3, AxisComparer> sortJob = TargetPositions.SortJob(new AxisComparer { });

            FindNearestJob findJob = new FindNearestJob
            {
                TargetPositon = TargetPositions,
                SeekerPosition = SeekerPositions,
                NearestTargetPosition = NearestTargetPositions
            };
            
            JobHandle sortHandle = sortJob.Schedule();
            JobHandle findHandler = findJob.Schedule(SeekerPositions.Length, 100, sortHandle);
            
            findHandler.Complete();
            
            for(int i = 0; i < NearestTargetPositions.Length; i++)
            {
                Debug.DrawLine(SeekerPositions[i], NearestTargetPositions[i]);
            }
        }
    }

    [BurstCompile]
    public struct FindNearestJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> TargetPositon;
        [ReadOnly] public NativeArray<float3> SeekerPosition;
        public NativeArray<float3> NearestTargetPosition;

        public void Execute(int index)
        {
            float3 seekerPos = SeekerPosition[index];

            int startIndex = TargetPositon.BinarySearch(seekerPos, new AxisComparer());
            
            // When no precise match is found, BinarySearch returns the bitwise negation of the last-searched offset.
            // So when startIdx is negative, we flip the bits again, but we then must ensure the index is within bounds.
            if(startIndex < 0) startIndex = ~startIndex;
            if(startIndex >= TargetPositon.Length) startIndex = TargetPositon.Length - 1;
            
            float3 nearestTargetPos = TargetPositon[startIndex];
            float nearestDistSq = math.distancesq(seekerPos, nearestTargetPos);
            
            Search(seekerPos, startIndex + 1, TargetPositon.Length, 1, ref nearestTargetPos, ref nearestDistSq);
            Search(seekerPos, startIndex - 1, -1, -1, ref nearestTargetPos, ref nearestDistSq);
            
            NearestTargetPosition[index] = nearestTargetPos;
        }

        private void Search(float3 seekerPos, int startIndex, int endIndex, int step,
            ref float3 nearestTargetPos, ref float nearestDistSq)
        {
            for(int i = startIndex; i != endIndex; i += step)
            {
                float3 targetPos = TargetPositon[i];
                float xDiff = seekerPos.x - targetPos.x;
                
                if(xDiff * xDiff > nearestDistSq) break;
                
                float distSq = math.distancesq(seekerPos, targetPos);
                
                if(distSq < nearestDistSq)
                {
                    nearestDistSq = distSq;
                    nearestTargetPos = targetPos;
                }
            }
        }
    }

    public struct AxisComparer : IComparer<float3>
    {
        public int Compare(float3 a, float3 b)
        {
            return a.x.CompareTo(b.x);
        }
    }
}