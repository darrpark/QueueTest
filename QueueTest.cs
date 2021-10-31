// Testing class QueueInheritance.
using System;
using System.Collections.Generic;
using QueueInheritanceLibrary;
using System.Linq;

// demonstrate functionality of class QueueInheritance
class QueueTest
{
   public static void Main( string[] args )
   {
        Process P0 = new Process("P0", 0, 5, 1);
        Process P1 = new Process("P1", 1, 3, 2);
        Process P2 = new Process("P2", 2, 8, 1);
        Process P3 = new Process("P3", 3, 6, 3);
        Process P4 = new Process("P4", 4, 5, 4);
        Process[] Parray = { P0, P1, P2, P3, P4 };
        List<Process> Processes = Parray.ToList();
        QueueAlgorithms.FCFS(Processes);
        QueueAlgorithms.SJN(Processes);
        QueueAlgorithms.Priority(Processes);
        QueueAlgorithms.ShortestRemaining(Processes);
        QueueAlgorithms.RoundRobin(Processes, 3);

        Console.WriteLine("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");

        P0 = new Process("P0", 0, 6, 1);
        P1 = new Process("P1", 1, 3, 2);
        P2 = new Process("P2", 2, 3, 1);
        P3 = new Process("P3", 3, 2, 3);
        P4 = new Process("P4", 4, 6, 4);
        Process[] Parray2 = { P0, P1, P2, P3, P4 };
        Processes = Parray2.ToList();
        QueueAlgorithms.FCFS(Processes);
        QueueAlgorithms.SJN(Processes);
        QueueAlgorithms.Priority(Processes);
        QueueAlgorithms.ShortestRemaining(Processes);
        QueueAlgorithms.RoundRobin(Processes, 3);

    } // end Main
} // end class QueueTest