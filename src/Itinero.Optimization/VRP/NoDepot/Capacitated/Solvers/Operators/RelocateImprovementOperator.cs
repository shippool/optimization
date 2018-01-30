﻿/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
 
using System;
using System.Collections.Generic;
using System.Text;
using Itinero.Optimization.Algorithms.CheapestInsertion;
using Itinero.Optimization.Algorithms.Random;
using Itinero.Optimization.Algorithms.Solvers;
using Itinero.Optimization.Tours;

namespace Itinero.Optimization.VRP.NoDepot.Capacitated.Solvers.Operators
{
    /// <summary>
    /// Implements a relocate operator, tries to improve the existing tours by re-inserting a visit from one tour into another.
    /// </summary>
    /// <remarks>
    /// This follows stop on first-improvement strategy. The algorithm works as follows:
    /// 
    /// - Select 2 random tours.
    /// - Try relocating from tour1 -> tour2:
    ///   - Loop over all visits in tour1.
    ///   - Check if they are cheaper to visit in tour2.
    /// - Try relocating from tour2 -> tour2:
    ///   - (see above)
    /// 
    /// The search stops from the moment any improvement is found.
    /// </remarks>
    public class RelocateImprovementOperator : IOperator<float, NoDepotCVRProblem, NoDepotCVRPObjective, NoDepotCVRPSolution, float>
    {
        private const float E = 0.001f;

        /// <summary>
        /// Return the name of this improvement.
        /// </summary>
        public string Name
        {
            get
            {
                return "REL";
            }
        }

        /// <summary>
        /// Returns true if the given object is supported.
        /// </summary>
        /// <param name="objective"></param>
        /// <returns></returns>
        public bool Supports(NoDepotCVRPObjective objective)
        {
            return true;
        }

        /// <summary>
        /// Applies this operator.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="objective"></param>
        /// <param name="solution"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public bool Apply(NoDepotCVRProblem problem, NoDepotCVRPObjective objective, NoDepotCVRPSolution solution, out float delta)
        {
            // choose two random routes.
            var random = RandomGeneratorExtensions.GetRandom();
            var tourIdx1 = random.Generate(solution.Count);
            var tourIdx2 = random.Generate(solution.Count - 1);
            if (tourIdx2 >= tourIdx1)
            {
                tourIdx2++;
            }

            // try relocation from 1->2 and 2->1.
            if (this.RelocateFromTo(problem, solution, tourIdx1, tourIdx2, out delta))
            {
                return true;
            }
            if (this.RelocateFromTo(problem, solution, tourIdx2, tourIdx1, out delta))
            {
                return true;
            }

            delta = 0;
            return false;
        }

        /// <summary>
        /// Tries a relocation of the visits in tour1 to tour2.
        /// </summary>
        private bool RelocateFromTo(NoDepotCVRProblem problem, NoDepotCVRPSolution solution,
            int tourIdx1, int tourIdx2, out float delta)
        {
            int previous = -1;
            int current = -1;

            var tour1 = solution.Tour(tourIdx1);
            var tour2 = solution.Tour(tourIdx2);

            var tour2Weight = solution.Weights[tourIdx2];
            foreach (int next in tour1)
            {
                if (previous >= 0 && current >= 0)
                { // consider the next customer.

                    int countBefore1 = tour1.Count;
                    int countBefore2 = tour2.Count;

                    if (this.TryVisit(problem, tour2, previous, current, next, tour2Weight, out delta))
                    {
                        tour1.ReplaceEdgeFrom(previous, next);
                        return true;
                    }
                }

                previous = current;
                current = next;
            }
            
            delta = 0;
            return false;
        }

        /// <summary>
        /// Considers one visit for relocation.
        /// </summary>
        private bool TryVisit(NoDepotCVRProblem problem, ITour tour, int previous, int current, int next, 
            float routeWeight, out float delta)
        {
            // calculate the removal gain of the customer.
            var max = problem.Max;
            var removalGain = problem.Weights[previous][current] + problem.Weights[current][next]
                - problem.Weights[previous][next];
            if (removalGain > E)
            { // calculate cheapest placement.
                Pair location;
                var result = tour.CalculateCheapest(problem.Weights, current, out location);
                if (result < removalGain - E && routeWeight + result < max)
                { // there is a gain in relocating this visit.
                    tour.ReplaceEdgeFrom(location.From, current);
                    tour.ReplaceEdgeFrom(current, location.To);
                    delta = result - removalGain;
                    return true;
                }
            }
            delta = 0;
            return false;
        }
    }
}