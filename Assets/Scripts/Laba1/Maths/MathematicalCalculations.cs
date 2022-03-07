﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Laba1.Maths
{
    public class MathematicalCalculations
    {
        public Vector2 CalculateNewObjectPosition(List<Vector2> positions)
        {
            Vector2 startPosition = positions[0];
            Vector2 vectorDifference = CalculateVectorsDifference(positions);
            
            float centerX = vectorDifference.x / 2;
            float centerY = vectorDifference.y / 2;

            Vector2 result = new Vector2(startPosition.x + centerX, startPosition.y + centerY);
            return result;
        }
        
        public int CalculateNewObjectSize(List<Vector2> positions, float coefficient, float fault)
        {
            Vector2 vectorDifference = CalculateVectorsDifference(positions);
            double length = Math.Sqrt(Math.Pow(vectorDifference.x, 2) + Math.Pow(vectorDifference.y, 2));

            return (int)((float) length * coefficient - fault);
        }
        
        public double CalculateNewObjectAngle(List<Vector2> positions)
        {
            Vector2 vectorDifference = CalculateVectorsDifference(positions);
            return Math.Atan2(vectorDifference.x, vectorDifference.y) * -180 / Math.PI;
        }
        
        public bool CheckPositionCorrections(Vector2 position)
        {
            return position.x > Screen.width * 0.45f;
        }
        
        public bool CheckDistanceBetweenEachOther(Vector2 position1, Vector2 position2, float minDistance)
        {
            return Math.Abs(position1.x - position2.x) < minDistance &&
                   Math.Abs(position1.y - position2.y) < minDistance;
        }
        
        private Vector2 CalculateVectorsDifference(List<Vector2> positions)
        {
            Vector2 startPosition = positions[0];
            Vector2 endPosition = positions[1];
            
            float diffX = endPosition.x - startPosition.x;
            float diffY = endPosition.y - startPosition.y;

            return new Vector2(diffX, diffY);
        }
    }
}