using System;
using System.Collections;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

public class CatmullRom
    {
        [Serializable]
        public struct CatmullRomPoint
        {
            public Vector3 position,tangent ,normal;
            public CatmullRomPoint(Vector3 position, Vector3 tangent, Vector3 normal)
            {
                this.position = position;
                this.tangent = tangent;
                this.normal = normal;
            }
        }
        private int resolution; 
        private bool closedLoop;
        private NativeArray<CatmullRomPoint> splinePoints;
        private Vector3[] controlPoints;

        // public NativeArray<CatmullRomPoint> GetPoints()
        // {
        //     if(splinePoints == null) throw new NullReferenceException("Spline ist noch null");
        //    // NativeArray<CatmullRomPoint> splinePjusttoReturn = new NativeArray<CatmullRomPoint>(splinePoints, Allocator.Temp);
        //   
        //        return splinePoints;
        //
        // }
        public CatmullRom(Transform[] controlPoints, int resolution, bool closedLoop)
        {
            if(controlPoints == null || controlPoints.Length <= 2 || resolution < 2)
                throw new ArgumentException("Mehr Controlpoints oder höhere Resolution");
            this.controlPoints = new Vector3[controlPoints.Length];
            for(int i = 0; i < controlPoints.Length; i++)
                this.controlPoints[i] = controlPoints[i].position;
            this.resolution = resolution; this.closedLoop = closedLoop;
            GenerateSplinePoints();
        }
        public CatmullRom(TransformAccessArray controlPoints, int resolution, bool closedLoop)
        {
          
            this.controlPoints = new Vector3[controlPoints.length];
            for(int i = 0; i < controlPoints.length; i++)
                this.controlPoints[i] = controlPoints[i].position;
            this.resolution = resolution; this.closedLoop = closedLoop;
            GenerateSplinePoints();
        }
        public void Update(Transform[] controlPoints)
        {
            if(controlPoints.Length <= 0 || controlPoints == null)
                throw new ArgumentException("Illegale Aktion, Controlpoints stimmen nicht");
                this.controlPoints = new Vector3[controlPoints.Length];
            for(int i = 0; i < controlPoints.Length; i++) 
                this.controlPoints[i] = controlPoints[i].position;
            GenerateSplinePoints();
        }
        public void Update(int resolution, bool closedLoop)
        {
            if(resolution < 2) throw new ArgumentException("Invalid Resolution. Make sure it's >= 1");
            this.resolution = resolution;
            this.closedLoop = closedLoop;
            GenerateSplinePoints();
        }
        public void DrawSpline(Color color, CatmullRomPoint[] splinePoints)
        {
            if(ValidatePoints())
            {
                for(int i = 0; i < splinePoints.Length; i++)
                {
                    if(i == splinePoints.Length - 1 && closedLoop)
                        Debug.DrawLine(splinePoints[i].position, splinePoints[0].position, color);
                    else if(i < splinePoints.Length - 1)
                        Debug.DrawLine(splinePoints[i].position, splinePoints[i+1].position, color);
                }
            }
        }
        public void DrawNormals(float extrusion, Color color, CatmullRomPoint[] splinePoints)
        {
            if(ValidatePoints())
            {
                for(int i = 0; i < splinePoints.Length; i++)
                    Debug.DrawLine(splinePoints[i].position, splinePoints[i].position + splinePoints[i].normal * extrusion, color);
            }
        }
        public void DrawTangents(float extrusion, Color color, CatmullRomPoint[] splinePoints)
        {
            if (!ValidatePoints()) return;
            for (int i = 0; i < splinePoints.Length; i++)
                Debug.DrawLine(splinePoints[i].position,splinePoints[i].position + splinePoints[i].tangent * extrusion, color);

        }
        private bool ValidatePoints()
        {
            if (splinePoints == null) throw new NullReferenceException("Spline not initialized!");
            return true;
        }

        
        public CatmullRomPoint[]  GenerateSplinePoints()
        {
            
            //------------ Calculate points.  First part is not Multithreaded because
            // and also it needs a double buffer, so write only is no longer possible
            
            Vector3 p0, p1, m0, m1;
            float pointStep;
            int closedAdjustment = closedLoop ? 0 : 1;
            int arrayCounter = 0;
            //pointsToCalculateDataForJob[] hasPointCalculationDataForJob = new pointsToCalculateDataForJob[controlPoints.Length- closedAdjustment]; 
            NativeArray<PointsToCalculateDataForJob> hasPointCalculationDataForJob =
                new NativeArray<PointsToCalculateDataForJob>(resolution*(controlPoints.Length - closedAdjustment), Allocator.TempJob);
            for (int currentPoint = 0; currentPoint < controlPoints.Length - closedAdjustment; currentPoint++)
            {
                bool closedLoopFinalPoint = (closedLoop && currentPoint == controlPoints.Length - 1);
                p0 = controlPoints[currentPoint];
                if (closedLoopFinalPoint) p1 = controlPoints[0];
                else p1 = controlPoints[currentPoint + 1];
                if (currentPoint == 0)
                {
                    if (closedLoop) m0 = p1 - controlPoints[controlPoints.Length - 1];
                    else m0 = p1 - p0;
                }
                else m0 = p1 - controlPoints[currentPoint - 1];

                if (closedLoop)
                {
                    if (currentPoint == controlPoints.Length - 1)
                        m1 = controlPoints[(currentPoint + 2) % controlPoints.Length] - p0;
                    else if (currentPoint == 0) m1 = controlPoints[currentPoint + 2] - p0;
                    else m1 = controlPoints[(currentPoint + 2) % controlPoints.Length] - p0;
                }
                else
                {
                    if (currentPoint < controlPoints.Length - 2)
                        m1 = controlPoints[(currentPoint + 2) % controlPoints.Length] - p0;
                    else m1 = p1 - p0;
                }

                m0 *= 0.5f; m1 *= 0.5f;
                pointStep = 1.0f / resolution;
                if (currentPoint == controlPoints.Length - 2 && !closedLoop || closedLoopFinalPoint)
                    pointStep = 1.0f / (resolution - 1);
        
                for (int tesselatedPoint = 0; tesselatedPoint < resolution; tesselatedPoint++)
                {
                    hasPointCalculationDataForJob[arrayCounter] = new PointsToCalculateDataForJob(p0, p1, m0, m1, tesselatedPoint * pointStep);
                    arrayCounter++;
                }
            }
            //------------ Job sheduling so the calulations are done in parallel ------------//
            splinePoints = new NativeArray<CatmullRomPoint>(arrayCounter, Allocator.TempJob);
            EvaluateJob evaluateJob = new EvaluateJob
                 {
                      splinePoints =  splinePoints,
                      hasPointCalculationDataForJob = hasPointCalculationDataForJob.AsReadOnly()
                 };
                 evaluateJob.Schedule(arrayCounter, 4).Complete();
                 hasPointCalculationDataForJob.Dispose();
                  CatmullRomPoint[] returnPooints = new CatmullRomPoint[arrayCounter];
                 splinePoints.CopyTo(returnPooints);
                 splinePoints.Dispose();
                 return returnPooints;
    }
        }


public struct PointsToCalculateDataForJob
{
    public Vector3 start, end, tan1, tan2;
    public readonly float tValue;
    public PointsToCalculateDataForJob(Vector3 start, Vector3 end, Vector3 tan1, Vector3 tan2,  float tValue)
    {
        this.start= start;
        this.end = end;
        this.tan1 = tan1;
        this.tan2 = tan2;
        this.tValue = tValue;
    }
}
[BurstCompile(FloatPrecision.Low,FloatMode.Fast)] 
public struct EvaluateJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<PointsToCalculateDataForJob>.ReadOnly hasPointCalculationDataForJob;
   [WriteOnly] public NativeArray<CatmullRom.CatmullRomPoint> splinePoints;
    private CatmullRom.CatmullRomPoint point;
    private float t;
    public void Execute(int index)
    {
        t = hasPointCalculationDataForJob[index].tValue;
        
        point.position =(2.0f * t * t * t - 3.0f * t * t + 1.0f) * hasPointCalculationDataForJob[index].start
                   + (t * t * t - 2.0f * t * t + t) * hasPointCalculationDataForJob[index].tan1
                   + (-2.0f * t * t * t + 3.0f * t * t) * hasPointCalculationDataForJob[index].end
                   + (t * t * t - t * t) * hasPointCalculationDataForJob[index].tan2;
        
        point.tangent =((6 * t * t - 6 * t) *hasPointCalculationDataForJob[index].start
                    + (3 * t * t - 4 * t + 1) * hasPointCalculationDataForJob[index].tan1
                    + (-6 * t * t + 6 * t) * hasPointCalculationDataForJob[index].end
                    + (3 * t * t - 2 * t) * hasPointCalculationDataForJob[index].tan2).normalized;

        point.normal= Vector3.Cross(point.tangent, Vector3.up).normalized / 2;
        splinePoints[index] = new CatmullRom.CatmullRomPoint(point.position, point.tangent, point.normal);
    }
}