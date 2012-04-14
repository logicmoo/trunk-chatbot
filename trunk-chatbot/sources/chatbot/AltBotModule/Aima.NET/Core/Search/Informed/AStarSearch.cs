﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aima.Core.Search.Informed
{
    using Aima.Core.Search.Framework;

    /// <summary>
    /// Artificial Intelligence A Modern Approach (3rd Edition): page 93.
    /// 
    /// The most widely known form of best-first search is called A* Search (pronounced
    /// "A-star search"). It evaluates nodes by combining g(n), the cost to reach the node,
    /// and h(n), the cost to get from the node to the goal:<br />
    ///   f(n) = g(n) + h(n).<br />
    /// Since g(n) gives the path cost from the start node to node n, and h(n) is the
    /// estimated cost of the cheapest path from n to the goal, we have<br />
    ///   f(n) = estimated cost of the cheapest solution through n.
    /// </summary>
    public class AStarSearch : BestFirstSearch {

        public AStarSearch(QueueSearch search, IHeuristicFunction hf):base(search, new AStarEvaluationFunction(hf)) 
        {
        }
    }
}
