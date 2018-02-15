/*
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
using System.IO;

namespace Itinero.Optimization.Test.Functional.VRP.NoDepot.Capacitated
{
    public static class NoDepotCVRPTests
    {
        public static void Run()
        {
            // WECHELDERZANDE - LILLE
            // build routerdb and save the result.
            var lille = Staging.RouterDbBuilder.Build("query3");
            var vehicle = lille.GetSupportedVehicle("car");
            var router = new Router(lille);

            // build problem1.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem1.geojson"));

            // 
            var func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, 900));
            var routes = func.TestPerf("Testing No-Depot Capacitated VRP (0->0)");

#if DEBUG
            routes.WriteGeoJson("lille-{0}.geojson");
#endif

            // SPIJKENISSE
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            vehicle = spijkenisse.GetSupportedVehicle("car");
            router = new Router(spijkenisse);

            // build problem.
            locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem2-spijkenisse.geojson"));        

            // 
            var max = 5400;
            func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max));
            routes = func.TestPerf("Testing No-Depot Capacitated VRP (0->0)");

#if DEBUG
            routes.WriteGeoJson("spijkenisse-{0}.geojson");
#endif

            // DE-HAGUE
            // build routerdb and save the result.
            var dehague = Staging.RouterDbBuilder.Build("query5");
            vehicle = dehague.GetSupportedVehicle("car");
            router = new Router(dehague);

            // build problem.
            locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem3-de-hague.geojson"));        

            // 
            max = 3800;
            func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max));
            routes = func.TestPerf("Testing No-Depot Capacitated VRP (0->0)");

#if DEBUG
            routes.WriteGeoJson("de-hague-{0}.geojson");
#endif

            // ROTTERDAM
            // build routerdb and save the result.
            var rotterdam = Staging.RouterDbBuilder.Build("query6");
            vehicle = rotterdam.GetSupportedVehicle("car");
            router = new Router(rotterdam);

            // build problem.
            locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("data.NoDepotCVRP.problem4-rotterdam.geojson"));        

            // 
            max = 4500;
            func = new Func<List<Route>>(() => router.CalculateNoDepotCVRP(vehicle.Fastest(), locations, max));
            routes = func.TestPerf("Testing No-Depot Capacitated VRP (0->0)");

#if DEBUG
            routes.WriteGeoJson("rotterdam-{0}.geojson");
#endif 
        }
    }
}