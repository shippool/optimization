﻿// Itinero.Logistics - Route optimization for .NET
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of Itinero.
// 
// Itinero is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Itinero is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Itinero. If not, see <http://www.gnu.org/licenses/>.

using Itinero.Logistics.Solutions.Algorithms;
using System.Collections.Generic;

namespace Itinero.Logistics.Solutions.STSP
{
    /// <summary>
    /// A STSP.
    /// </summary>
    public class STSPProblem : ISTSP
    {
        /// <summary>
        /// Creates a new STSP 'open' STSP with only a start customer.
        /// </summary>
        public STSPProblem(int first, float[][] weights, float max)
        {
            this.First = first;
            this.Last = null;
            this.Weights = weights;
            this.Max = max;

            for (var x = 0; x < this.Weights.Length; x++)
            {
                this.Weights[x][first] = 0;
            }
        }

        /// <summary>
        /// Creates a new STSP, 'closed' when first equals last.
        /// </summary>
        public STSPProblem(int first, int last, float[][] weights, float max)
        {
            this.First = first;
            this.Last = last;
            this.Weights = weights;
            this.Max = max;

            this.Weights[first][last] = 0;
        }

        /// <summary>
        /// An empty constructor used just to clone stuff.
        /// </summary>
        protected STSPProblem()
        {

        }

        /// <summary>
        /// Gets the first customer.
        /// </summary>
        public int First
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the last customer.
        /// </summary>
        public int? Last
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the maximum weight.
        /// </summary>
        public float Max
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the weights.
        /// </summary>
        public float[][] Weights
        {
            get;
            protected set;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<int, INearestNeighbours[]> _forwardNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public INearestNeighbours GetNNearestNeighboursForward(int n, int customer)
        {
            if (_forwardNearestNeighbours == null)
            { // not there yet, create.
                _forwardNearestNeighbours = new Dictionary<int, INearestNeighbours[]>();
            }
            INearestNeighbours[] nearestNeighbours = null;
            if (!_forwardNearestNeighbours.TryGetValue(n, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new INearestNeighbours[this.Weights.Length];
                _forwardNearestNeighbours.Add(n, nearestNeighbours);
            }
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighboursAlgorithm.Forward(this.Weights, n, customer);
                nearestNeighbours[customer] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<int, INearestNeighbours[]> _backwardNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        public INearestNeighbours GetNNearestNeighboursBackward(int n, int customer)
        {
            if (_backwardNearestNeighbours == null)
            { // not there yet, create.
                _backwardNearestNeighbours = new Dictionary<int, INearestNeighbours[]>();
            }
            INearestNeighbours[] nearestNeighbours = null;
            if (!_backwardNearestNeighbours.TryGetValue(n, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new INearestNeighbours[this.Weights.Length];
                _backwardNearestNeighbours.Add(n, nearestNeighbours);
            }
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighboursAlgorithm.Backward(this.Weights, n, customer);
                nearestNeighbours[customer] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<double, ISortedNearestNeighbours[]> _forwardSortedNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public ISortedNearestNeighbours GetNearestNeighboursForward(double weight, int customer)
        {
            if (_forwardSortedNearestNeighbours == null)
            { // not there yet, create.
                _forwardSortedNearestNeighbours = new Dictionary<double, ISortedNearestNeighbours[]>();
            }
            ISortedNearestNeighbours[] nearestNeighbours = null;
            if (!_forwardSortedNearestNeighbours.TryGetValue(weight, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new ISortedNearestNeighbours[this.Weights.Length];
                _forwardSortedNearestNeighbours.Add(weight, nearestNeighbours);
            }
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighboursAlgorithm.Forward(this.Weights, weight, customer);
                nearestNeighbours[customer] = result;
            }
            return result;
        }

        /// <summary>
        /// Holds the nearest neighbours.
        /// </summary>
        private Dictionary<double, ISortedNearestNeighbours[]> _backwardSortedNearestNeighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <returns></returns>
        public ISortedNearestNeighbours GetNearestNeighboursBackward(double weight, int customer)
        {
            if (_backwardSortedNearestNeighbours == null)
            { // not there yet, create.
                _backwardSortedNearestNeighbours = new Dictionary<double, ISortedNearestNeighbours[]>();
            }
            ISortedNearestNeighbours[] nearestNeighbours = null;
            if (!_backwardSortedNearestNeighbours.TryGetValue(weight, out nearestNeighbours))
            { // not found for n, create.
                nearestNeighbours = new ISortedNearestNeighbours[this.Weights.Length];
                _backwardSortedNearestNeighbours.Add(weight, nearestNeighbours);
            }
            var result = nearestNeighbours[customer];
            if (result == null)
            { // not found, calculate.
                result = NearestNeighboursAlgorithm.Backward(this.Weights, weight, customer);
                nearestNeighbours[customer] = result;
            }
            return result;
        }

        /// <summary>
        /// Converts this problem to it's closed equivalent.
        /// </summary>
        /// <returns></returns>
        public virtual ISTSP ToClosed()
        {
            if (this.Last == null)
            { // 'open' problem, just set weights to first to 0.
                // REMARK: weights already set in constructor.
                return new STSPProblem(this.First, this.First, this.Weights, this.Max);
            }
            else if (this.First != this.Last)
            { // 'open' problem but with fixed weights.
                var weights = new float[this.Weights.Length - 1][];
                for (var x = 0; x < this.Weights.Length; x++)
                {
                    if (x == this.Last)
                    { // skip last edge.
                        continue;
                    }
                    var xNew = x;
                    if (x > this.Last)
                    { // decrease new index.
                        xNew = xNew - 1;
                    }

                    weights[xNew] = new float[this.Weights[x].Length - 1];

                    for (var y = 0; y < this.Weights[x].Length; y++)
                    {
                        if (y == this.Last)
                        { // skip last edge.
                            continue;
                        }
                        var yNew = y;
                        if (y > this.Last)
                        { // decrease new index.
                            yNew = yNew - 1;
                        }

                        if (yNew == xNew)
                        { // make not sense to keep values other than '0' and to make things easier to understand just use '0'.
                            weights[xNew][yNew] = 0;
                        }
                        else if (y == this.First)
                        { // replace -> first with -> last.
                            weights[xNew][yNew] = this.Weights[x][this.Last.Value];
                        }
                        else
                        { // nothing special about this connection, yay!
                            weights[xNew][yNew] = this.Weights[x][y];
                        }
                    }
                }
                return new STSPProblem(this.First, this.First, weights, this.Max);
            }
            return this; // problem already closed with first==last.
        }

        /// <summary>
        /// Creates a deep-copy of this problem.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            var weights = new double[this.Weights.Length][];
            for (var i = 0; i < this.Weights.Length; i++)
            {
                weights[i] = this.Weights[i].Clone() as double[];
            }
            var clone = new STSPProblem();
            clone.First = this.First;
            clone.Last = this.Last;
            clone.Weights = this.Weights;
            clone.Max = this.Max;
            return clone;
        }
    }
}