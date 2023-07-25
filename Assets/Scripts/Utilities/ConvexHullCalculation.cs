using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvexHullCalculation : MonoBehaviour
{
    Vector2[] convexHullPoints;

    public Vector2[] CalculateConvexHull(Vector2[] allPoints)
    {
        Vector2 pointOnHull = allPoints[LeftMostPoint(allPoints)];
        Vector2 endPoint = new Vector2(0, 0);

        convexHullPoints = new Vector2[allPoints.Length];

        convexHullPoints[0] = pointOnHull;
        int i = 0;
        
        while (endPoint != convexHullPoints[0])
        {
            if(i == allPoints.Length)
            {
                return convexHullPoints;
            }

            endPoint = allPoints[0];
            double currentNextPointPolarAngle = 0;

            for (int j = 0; j < allPoints.Length; j++)
            {

                float possibleNewPolarAngle;

                if (i == 0)
                {
                    possibleNewPolarAngle = CalculatePolarAngle(convexHullPoints[i], convexHullPoints[i] + new Vector2(0, -5), allPoints[j]);
                }
                else
                {
                    possibleNewPolarAngle = CalculatePolarAngle(convexHullPoints[i], convexHullPoints[i - 1], allPoints[j]);
                }

                if (possibleNewPolarAngle > currentNextPointPolarAngle && possibleNewPolarAngle > 0)
                {
                    currentNextPointPolarAngle = possibleNewPolarAngle;
                    endPoint = allPoints[j];
                }
            }

            i++;
            pointOnHull = endPoint;
            convexHullPoints[i] = pointOnHull;
        }

        Vector2[] convexHull = FillConvexHull();
        return convexHull;
    }

    private int LeftMostPoint(Vector2[] allPoints)
    {
        int currentMostLeftpoint = 0;
        for (int i = 1; i < allPoints.Length; i++)
        {
            if (allPoints[currentMostLeftpoint].x > allPoints[i].x)
            {
                currentMostLeftpoint = i;
            }
        }
        return currentMostLeftpoint;
    }

    private float CalculatePolarAngle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        Vector2 line1 = point1 - point2;
        Vector2 line2 = point1 - point3;

        return Vector2.Angle(line1, line2);
    }

    private Vector2[] FillConvexHull()
    {
        Vector2[] newConvexHull = new Vector2[CountNumberOfNodesInConvexHull()];
        for (int i = 0; i < newConvexHull.Length; i++)
        {
            newConvexHull[i] = convexHullPoints[i];
        }
        return newConvexHull;
    }

    private int CountNumberOfNodesInConvexHull()
    {
        int numberOfNodes = 0;
        Vector2 currentNode = convexHullPoints[0];
        while (currentNode != new Vector2(0, 0))
        {
            numberOfNodes++;
            currentNode = convexHullPoints[numberOfNodes];
        }
        return numberOfNodes;
    }
}
