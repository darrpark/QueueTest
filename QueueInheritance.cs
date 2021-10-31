// Implementing queue-based process scheduling algorithms
using LinkedListLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QueueInheritanceLibrary
{
    public class Process
    {
        public string name;
        public int arrivalTime;
        public int executeTime;
        public int totalExecTime;
        public List<int> serviceTime;
        public int priority;
        public int waitTime;
        public int completionTime;

        public Process( string name, int arrivalTime, int executeTime, int priority )
        {
            this.name = name;
            this.arrivalTime = arrivalTime;
            this.executeTime = this.totalExecTime = executeTime;
            this.serviceTime = new List<int>();
            this.priority = priority;
            this.waitTime = 0;
            this.completionTime = 0;
        }
    }
    public class ByArrivalTime : Comparer<Process>
    {
        public override int Compare(Process x, Process y)
        {
            return x.arrivalTime.CompareTo(y.arrivalTime);
        }
    }
    public class ByExecuteTime : Comparer<Process>
    {
        public override int Compare(Process x, Process y)
        {
            return x.executeTime.CompareTo(y.executeTime);
        }
    }
    public class ByPriority : Comparer<Process>
    {
        public override int Compare(Process x, Process y)
        {
            return x.priority.CompareTo(y.priority);
        }
    }

    public class ProcessOrder
    {
        private List<string> processes;
        public int size;
        public string name;

        public ProcessOrder( string name )
        {
            processes = new List<string>();
            size = 0;
            this.name = name;
        }

        public void AddNextProcess(Process p)
        {
            processes.Add(p.name);
            size++;
        }

        public void DisplayOrder(List<Process> opcs)
        {
            //Process[] Parray = new Process[size];

            //int index = 0;
            //while (opcs.Count > 0)
            //{
            //    Parray[index] = opcs[index++];
            //}
            Process[] Parray = opcs.ToArray();

            /* Calculate waitTime and serviceTime */
            int totalWaitTime = 0;
            Parray[0].waitTime = 0;
            for (int i = 1; i < Parray.Length; i++)
            {
                Process curr = Parray[i];
                Process prev = Parray[i - 1];
                // For FCFS, SJN, Prio, only one service time (non-preemptive)
                curr.waitTime = curr.serviceTime[0] - curr.arrivalTime;
                totalWaitTime += curr.waitTime;
            }
            
            Console.Write("The {0} order for these processes is: " + string.Join(" => ", processes), name);
            Console.WriteLine();
            Console.WriteLine(String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15}", "Process", "ArrivalTime", "ExecuteTime", "ServiceTime", "Priority", "WaitTime"));
            for (int i = 0; i < Parray.Length; i++)
            {
                Console.WriteLine(String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15}", Parray[i].name, Parray[i].arrivalTime, Parray[i].executeTime, Parray[i].serviceTime[0], Parray[i].priority, Parray[i].waitTime));
            }
            Console.WriteLine("Total Wait Time: {0}\n", totalWaitTime);
        }

        public void DisplayOrderNoPre(List<Process> opcs)
        {
            Process[] Parray = opcs.ToArray();

            int totalWaitTime = 0;
            foreach (Process p in Parray)
            {
                p.waitTime = p.completionTime - p.totalExecTime - p.arrivalTime;
                totalWaitTime += p.waitTime;
            }

            Console.Write("The {0} order for these processes is: " + string.Join(" => ", processes), name);
            Console.WriteLine();
            Console.WriteLine(String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15} {6, -15}", "Process", "ArrivalTime", "ExecuteTime", "ServiceTime", "Priority", "WaitTime", "CompTime"));
            for (int i = 0; i < Parray.Length; i++)
            {
                Console.WriteLine(String.Format("{0, -15} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15} {6, -15}", Parray[i].name, Parray[i].arrivalTime, Parray[i].totalExecTime, string.Join(",", Parray[i].serviceTime), Parray[i].priority, Parray[i].waitTime, Parray[i].completionTime));
            }
            Console.WriteLine("Total Wait Time: {0}\n", totalWaitTime);
        }
    }

    public class ProcessQueue : List
    {
        public ProcessQueue()
            : base("process queue")
        {
        }
        public void Enqueue(Process p)
        {
            InsertAtBack(p);
        }
        public Process Dequeue()
        {
            return (Process) RemoveFromFront();
        }
        public void ListElements()
        {
            this.Display();
        }
    }

    /* Note: Input process list is in order by arrival time. */
    public class QueueAlgorithms
    {
        private static void resetProcesses(List<Process> processes)
        {
            foreach (Process p in processes)
            {
                p.executeTime = p.totalExecTime;
                p.serviceTime = new List<int>();
                p.waitTime = p.completionTime = 0;
            }
        }
        public static void FCFS(List<Process> processes)
        {
            ProcessOrder order = new ProcessOrder("FCFS-Based");

            /* Iterate through processes, service them in order of arrival */
            processes.Sort(new ByArrivalTime());
            int time = 0;
            for (int i = 0; i < processes.Count; i++)
            {
                Process curr = processes[i];
                curr.serviceTime.Add(time);
                time += curr.executeTime;
                order.AddNextProcess(curr);
            }

            order.DisplayOrder(processes);
            resetProcesses(processes);
        }

        public static void SJN(List<Process> processes)
        {
            ProcessOrder order = new ProcessOrder("SJN-Based");
            int totalProcesses = processes.Count;

            processes.Sort(new ByArrivalTime());
            int time = processes[0].arrivalTime;
            List<Process> batch = new List<Process>();

            /* Initialize batch */
            int index = 0;
            while (index < totalProcesses)
            {
                Process curr = processes[index];
                if (curr.arrivalTime == time)
                {
                    batch.Add(curr);
                    index++;
                } else
                {
                    break;
                }
            }

            while (index < totalProcesses || batch.Count > 0)
            {
                /* Take the shortest execute time Process from current batch */
                batch.Sort(new ByExecuteTime());
                Process next = batch[0];
                batch.RemoveAt(0);

                order.AddNextProcess(next);
                next.serviceTime.Add(time);

                /* Take next Process execute time, increment time and get all Processes that arrived during its execution */
                time += next.executeTime;

                while (index < totalProcesses)
                {
                    Process curr = processes[index];
                    if (curr.arrivalTime <= time)
                    {
                        batch.Add(curr);
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }

                /* If batch empty and there are remaining Processes, increment time */
                if (batch.Count == 0 && index < totalProcesses)
                {
                    time = processes[index].arrivalTime;
                    while (index < totalProcesses)
                    {
                        Process curr = processes[index];
                        if (curr.arrivalTime <= time)
                        {
                            batch.Add(curr);
                            index++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            order.DisplayOrder(processes);
            resetProcesses(processes);
        }

        public static void Priority(List<Process> processes)
        {
            ProcessOrder order = new ProcessOrder("Priority-Based");
            int totalProcesses = processes.Count();

            processes.Sort(new ByArrivalTime());
            int time = processes[0].arrivalTime;
            List<Process> batch = new List<Process>();

            /* Initialize batch */
            int index = 0;
            while (index < totalProcesses)
            {
                Process curr = processes[index];
                if (curr.arrivalTime == time)
                {
                    batch.Add(curr);
                    index++;
                }
                else
                {
                    break;
                }
            }

            while (index < totalProcesses || batch.Count > 0)
            {
                /* Take the shortest execute time Process from current batch */
                batch.Sort(new ByPriority());
                batch.Reverse();
                Process next = batch[0];
                batch.RemoveAt(0);

                order.AddNextProcess(next);
                next.serviceTime.Add(time);

                /* Take next Process execute time, increment time and get all Processes that arrived during its execution */
                time += next.executeTime;

                while (index < totalProcesses)
                {
                    Process curr = processes[index];
                    if (curr.arrivalTime <= time)
                    {
                        batch.Add(curr);
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }

                /* If batch empty and there are remaining Processes, increment time */
                if (batch.Count == 0 && index < totalProcesses)
                {
                    time = processes[index].arrivalTime;
                    while (index < totalProcesses)
                    {
                        Process curr = processes[index];
                        if (curr.arrivalTime <= time)
                        {
                            batch.Add(curr);
                            index++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            order.DisplayOrder(processes);
            resetProcesses(processes);
        }

        /* DUMB MATH STUFF */
        /* For ShortRem and RR
         * 
         * serviceTime: time of process start (can be multiple (LIST))
         * 
         * waitTime: completionTime - arrivalTime - totalExecTime
         * 
         */
        public static void ShortestRemaining(List<Process> processes)
        {
            ProcessOrder order = new ProcessOrder("SRT-Based");
            ProcessQueue queue = new ProcessQueue();
            List<Process> batch = new List<Process>();
            int time;

            int totalProcesses = processes.Count;
            processes.Sort(new ByArrivalTime());
            time = processes[0].arrivalTime;

            int index = 0;
            while (index < totalProcesses)
            {
                Process curr = processes[index];
                if (curr.arrivalTime == time)
                {
                    batch.Add(curr);
                    index++;
                }
                else
                {
                    break;
                }
            }

            /* yeet */
            Process prevExecuted = null;
            while (index < totalProcesses || batch.Count > 0)
            {
                if (index >= totalProcesses)
                {
                    foreach (Process p in batch)
                    {
                        // MATH HERE FOR TIME
                        if (p != prevExecuted)
                        {
                            p.serviceTime.Add(time);
                            order.AddNextProcess(p);
                        }
                        time += p.executeTime;
                        p.completionTime = time;
                    }
                    break;
                    // do sjn
                }
                else
                {
                    Process curr = batch[0];
                    if (prevExecuted == null || curr != prevExecuted)
                    {
                        curr.serviceTime.Add(time);
                        order.AddNextProcess(curr);
                    }
                    prevExecuted = curr;
                    int nextArrival = processes[index].arrivalTime;
                    while (time < nextArrival)
                    {
                        // Added this loop for math later
                        time++;
                        curr.executeTime--;
                        if (curr.executeTime <= 0)
                        {
                            batch.RemoveAt(0);
                            if (batch.Count > 0)
                            {
                                curr.completionTime = time;
                                curr = batch[0];
                                curr.serviceTime.Add(time);
                                order.AddNextProcess(curr);
                                prevExecuted = curr;
                            }
                            else
                            {
                                // Time increment here; keep in mind for math later
                                time = nextArrival;
                            }
                        }
                    }
                    // NEXT ARRIVAL
                    batch.Add(processes[index]);
                    index++;
                    batch.Sort(new ByExecuteTime());
                }
            }

            order.DisplayOrderNoPre(processes);
            resetProcesses(processes);
        }

        public static void RoundRobin(List<Process> processes, int quantum)
        {
            ProcessOrder order = new ProcessOrder("RoundRobin-Based");
            ProcessQueue queue = new ProcessQueue();
            int time;

            int index = 0;
            int totalProcesses = processes.Count;
            processes.Sort(new ByArrivalTime());
            time = processes[0].arrivalTime;

            /* Initialize our queue */
            while (index < totalProcesses)
            {
                Process curr = processes[index];
                if (curr.arrivalTime <= time)
                {
                    queue.Enqueue(curr);
                    index++;
                }
                else
                {
                    break;
                }
            }

            while (index < totalProcesses || !queue.IsEmpty())
            {
                Process curr = queue.Dequeue();
                order.AddNextProcess(curr);
                curr.serviceTime.Add(time);

                int timeCount = 0;
                while (timeCount < quantum)
                {
                    time++;
                    curr.executeTime--;
                    timeCount++;

                    // Receive incoming processes
                    while (index < totalProcesses)
                    {
                        Process next = processes[index];
                        if (next.arrivalTime <= time)
                        {
                            queue.Enqueue(next);
                            index++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (curr.executeTime <= 0)
                    {
                        curr.completionTime = time;
                        if (queue.IsEmpty())
                        {
                            if (index >= totalProcesses)
                            {
                                // we done
                                order.DisplayOrderNoPre(processes);
                                resetProcesses(processes);
                                return;
                            }
                            time = processes[index].arrivalTime;

                            // Receive incoming processes
                            while (index < totalProcesses)
                            {
                                Process next = processes[index];
                                if (next.arrivalTime <= time)
                                {
                                    queue.Enqueue(next);
                                    index++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // get next proc; reassign curr and timeCount
                            curr = queue.Dequeue();
                            curr.serviceTime.Add(time);
                            order.AddNextProcess(curr);
                            timeCount = 0;
                        }
                    }
                } // end of quantum period

                // curr still needs to execute
                queue.Enqueue(curr);
            }

            order.DisplayOrderNoPre(processes);
            resetProcesses(processes);
        }
    }

    
    

} // end namespace QueueInheritanceLibrary

/**************************************************************************
 * (C) Copyright 1992-2009 by Deitel & Associates, Inc. and               *
 * Pearson Education, Inc. All Rights Reserved.                           *
 *                                                                        *
 * DISCLAIMER: The authors and publisher of this book have used their     *
 * best efforts in preparing the book. These efforts include the          *
 * development, research, and testing of the theories and programs        *
 * to determine their effectiveness. The authors and publisher make       *
 * no warranty of any kind, expressed or implied, with regard to these    *
 * programs or to the documentation contained in these books. The authors *
 * and publisher shall not be liable in any event for incidental or       *
 * consequential damages in connection with, or arising out of, the       *
 * furnishing, performance, or use of these programs.                     *
 *************************************************************************/