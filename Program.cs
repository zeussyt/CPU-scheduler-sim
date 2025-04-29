using System;
using System.Collections.Generic;
using System.Linq;


//process class that holds details on each process
class Process
{
    public int id { get; set; } 
    public int arrivalTime { get; set; } 
    public int burstTime { get; set; }  
    public int timeRemaining { get; set; } 
    public int timeStart { get; set; } 
    public int timeComplete { get; set; } 
    public int timeWaiting => tat - burstTime; 
    public int tat => timeComplete - arrivalTime; // total time from arrival to completion
    public int responseTime => timeStart - arrivalTime; //time from arrival to execution


    //method to clone processes for each algorithm to ensure a clean copy
    public Process Clone()
    {
        return new Process
        {
            id = this.id,
            arrivalTime = this.arrivalTime,
            burstTime = this.burstTime,
            timeRemaining = this.burstTime, //resets timeRemaining to full burstTime
            timeStart = -1, //set time to -1 to show it has not started yet
            timeComplete = -1 //shows processes have not been completed
        };
    }
}

//class that contains metrics and algorithms to be displayed
class Scheduler
{

    //fcfs algorithm
    public List<Process> FCFSsim(List<Process> processes)
    {
        var time = 0;
        var sorted = processes.OrderBy(x => x.arrivalTime).ToList(); //sorts processes by arrival time

        foreach (var process in sorted)
        {
            if (time < process.arrivalTime)
                time = process.arrivalTime; //waits if cpu is idle

            process.timeStart = time; 
            time += process.burstTime; //simulates execution
            process.timeComplete = time; 
        }

        return sorted; //updates fcfs list
    }

    //srtf algorithm
    public List<Process> SRTFsim(List<Process> processes)
    {
        var time = 0;
        var remaining = new List<Process>(processes); //makes copy of processes 
        var completed = new List<Process>(); //stores completed processes

        while (remaining.Count > 0)
        {
            var ready = remaining.Where(x => x.arrivalTime <= time).ToList(); //filters ready processes

            if (ready.Count > 0)
            {
                var shortest = ready.OrderBy(x => x.timeRemaining).First(); //picks process with shortest time 
                shortest.timeRemaining--; 

                if (shortest.timeStart == -1)
                    shortest.timeStart = time; //records first time of execution

                if (shortest.timeRemaining == 0)
                {
                    shortest.timeComplete = time + 1; //records time complete
                    completed.Add(shortest);
                    remaining.Remove(shortest);
                }
            }

            time++; 
        }

        return completed; //return completed processes as list
    }

    //hrrn algorithm
    public List<Process> HRRNsim(List<Process> processes)
    {
        var time = 0;
        var remaining = new List<Process>(processes);
        var completed = new List<Process>();

        while (remaining.Count > 0)
        {
            var ready = remaining.Where(x => x.arrivalTime <= time).ToList();

            if (ready.Count > 0)
            {
                //calculates response ratio and picks the one with the highest ratio
                var hrrn = ready.OrderByDescending(p => (double)(time - p.arrivalTime + p.burstTime) / p.burstTime).First();

                hrrn.timeStart = time; 
                hrrn.timeComplete = time + hrrn.burstTime;
                completed.Add(hrrn);
                remaining.Remove(hrrn);

                time += hrrn.burstTime; //simulates execution
            }
            else
            {
                time++; //shows idle time if no process is ready
            }
        }

        return completed;
    }

    //makes deep copy of process list
    public static List<Process> CloneWorkload(List<Process> original)
    {
        return original.Select(p => p.Clone()).ToList();
    }
   

    //method that computes/prints simulation metrics
    public void Metrics(List<Process> processes)
    {
        int totalTime = processes.Max(p => p.timeComplete); 
        int totalBurst = processes.Sum(p => p.burstTime); 
        double totalWT = processes.Sum(p => p.timeWaiting); 
        double totalTAT = processes.Sum(p => p.tat); 
        double totalRT = processes.Sum(p => p.responseTime); 

        int n = processes.Count;

        //prints averages 
        Console.WriteLine("Average waiting time is : " + (totalWT / n) + " sec");
        Console.WriteLine("Average response time is : " + (totalRT / n) + " sec");
        Console.WriteLine("Average turnaround time is : " + (totalTAT / n) + " sec");
        Console.WriteLine();
        Console.WriteLine("CPU Utilization was : " + ((double)totalBurst / totalTime) * 100 + "%");
        Console.WriteLine("Throughput : " + ((double)n / totalTime) + " processes / unit time");
    }
    //sample used for testing small sample
    public List<Process> sampleLoad()
    {
        return new List<Process>
        {
            new Process {id = 1, arrivalTime = 0, burstTime = 5, timeRemaining = 5},
            new Process {id = 2, arrivalTime = 1, burstTime = 2, timeRemaining = 2},
            new Process {id = 3, arrivalTime = 2, burstTime = 1, timeRemaining = 1},
            new Process {id = 4, arrivalTime = 3, burstTime = 3, timeRemaining = 3},
            new Process {id = 5, arrivalTime = 4, burstTime = 7, timeRemaining = 7},


        };
    }
    //sample pool used for large sample
    public List<Process> sampleLoad2()
    {
        Random r = new Random();
        var processes = new List<Process>();

        for (int i = 1; i <= 50; i++)
        {
            int burst = r.Next(1, 51); 
            int arrival = r.Next(50);  

            processes.Add(new Process
            {
                id = i, arrivalTime = arrival, burstTime = burst, timeRemaining = burst //set burst and time remaining to prevent cpu idling / overclocking
            });
        }
        return processes;
    }
    public List<Process> sampleLoadE()
    {
        Random r = new Random();
        var processes = new List<Process>();

        for (int i = 1; i <= 50; i++)
        {
            int burst = r.Next(1, 51); 
            int arrival = r.Next(50);  

            processes.Add(new Process
            {
                id = i, arrivalTime = 0,burstTime = burst, timeRemaining = burst //set burst and time remaining to prevent cpu idling / overclocking
            });
        }
        return processes;
    }

    //class that contains main method, also Ashleigh is my girlfriend, I told her she would be in this program in some way <3
    class ILoveAshleigh
    {
        static void Main(string[] args)
        {
            var scheduler = new Scheduler();
            var load = scheduler.sampleLoad();
            var load2 = scheduler.sampleLoad2();
            var load3 = scheduler.sampleLoadE();
            
            Console.WriteLine("What kind of load would you like to test? Enter 'S' for small, 'L' for large, or 'E' for edge cases : ");
            string answer = Console.ReadLine();

            //small load for testing
            if(answer == "S")
            {
                //fcfs simulation
                Console.WriteLine("FCFS Sim (small load):");
                var fcfsRes = scheduler.FCFSsim(Scheduler.CloneWorkload(load));
                scheduler.Metrics(fcfsRes);

                //srtf simulation
                Console.WriteLine("\nSRTF Sim (small load):");
                var srtfRes = scheduler.SRTFsim(Scheduler.CloneWorkload(load));
                scheduler.Metrics(srtfRes);

                //hrrn simulation
                Console.WriteLine("\nHRRN Sim (small load):");
                var hrrnRes = scheduler.HRRNsim(Scheduler.CloneWorkload(load));
                scheduler.Metrics(hrrnRes);
            }

            //large load
            else if(answer == "L")
            {
                Console.Write("\nFCFS Sim (large load):");
                var fcfsRes2 = scheduler.FCFSsim(Scheduler.CloneWorkload(load2));
                Console.WriteLine();
                scheduler.Metrics(fcfsRes2);

                Console.WriteLine("\nSRTF Sim (large load):");
                var srtfRes2 = scheduler.SRTFsim(Scheduler.CloneWorkload(load2));
                scheduler.Metrics(srtfRes2);

                Console.WriteLine("\nHRRN Sim (large load):");
                var hrrnRes2 = scheduler.HRRNsim(Scheduler.CloneWorkload(load2));
                scheduler.Metrics(hrrnRes2);
            }

            //edge case
            else if( answer == "E")
            {
                Console.Write("\nFCFS Sim (edge cases):");
                var fcfsRes3 = scheduler.FCFSsim(Scheduler.CloneWorkload(load3));
                Console.WriteLine();
                scheduler.Metrics(fcfsRes3);

                Console.WriteLine("\nSRTF Sim (edge cases):");
                var srtfRes3 = scheduler.SRTFsim(Scheduler.CloneWorkload(load3));
                scheduler.Metrics(srtfRes3);

                Console.WriteLine("\nHRRN Sim (large load):");
                var hrrnRes3 = scheduler.HRRNsim(Scheduler.CloneWorkload(load3));
                scheduler.Metrics(hrrnRes3);
            }
            else if(answer != "S" || answer != "L")
            {
                Console.WriteLine("Please enter a valid answer.");
            }

        }
    }
}
