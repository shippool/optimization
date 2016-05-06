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

using Itinero.Logistics.Routes;
using Itinero.Logistics.Solvers;
using System.Collections.Generic;

namespace Itinero.Logistics.Solutions.TSP
{
    /// <summary>
    /// Implements a brute force solver by checking all possible combinations.
    /// </summary>
    public class BruteForceSolver : SolverBase<ITSP, ITSPObjective, IRoute>
    {
        /// <summary>
        /// Returns a new for this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "BF";
            }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <returns></returns>
        public override IRoute Solve(ITSP problem, ITSPObjective objective, out double fitness)
        {
            // initialize.
            var solution = new List<int>();
            for (int customer = 0; customer < problem.Weights.Length; customer++)
            { // add each customer again.
                if (customer != problem.First &&
                    customer != problem.Last)
                {
                    solution.Add(customer);
                }
            }

            if (solution.Count < 2)
            { // a tiny problem.
                // build route.
                var withFirst = new List<int>(solution);
                withFirst.Insert(0, problem.First);
                if (problem.Last.HasValue && problem.First != problem.Last)
                { // the special case of a fixed last customer.
                    withFirst.Add(problem.Last.Value);
                }
                var route = new Logistics.Routes.Route(withFirst, problem.Last);
                fitness = objective.Calculate(problem, route);
                return route;
            }

            // keep on looping until all the permutations 
            // have been considered.
            var enumerator = new PermutationEnumerable<int>(
                solution.ToArray());
            IRoute bestSolution = null;
            var bestFitness = double.MaxValue;
            foreach (var permutation in enumerator)
            {
                // build route from permutation.
                var withFirst = new List<int>(permutation);
                withFirst.Insert(0, problem.First);
                if (problem.Last.HasValue && problem.First != problem.Last)
                { // the special case of a fixed last customer.
                    withFirst.Add(problem.Last.Value);
                }
                var localRoute = new Logistics.Routes.Route(withFirst, problem.Last);

                // calculate fitness.
                var localFitness = objective.Calculate(problem, localRoute);
                if (localFitness < bestFitness)
                { // the best weight has improved.
                    bestFitness = localFitness;
                    bestSolution = localRoute;
                }
            }
            fitness = bestFitness;
            return bestSolution;
        }
    }
}