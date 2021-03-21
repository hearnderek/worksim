using System;
using System.Collections.Generic;
using System.Linq;

namespace pmgame
{
    public partial class Worker
    {
        // Configuration
        HashSet<DayOfWeek> workDays = Defaults.workDays;
        DateTime clockIn = Defaults.clockIn;
        DateTime clockOut = Defaults.clockOut;
        TimeSpan contextSwitchPenalty = Defaults.contextSwitchPenalty;

        // State members
        public TaskList personalTaskList = new TaskList();
        public Schedule personalSchedule = new Schedule();
        public State state;
        
        /// <summary>
        /// The time when entering the state. DateTime Min is use to represent NULL.
        /// </summary>
        public DateTime stateEntered = DateTime.MinValue;

        /// <summary>
        /// The time when beginning a task action. DateTime Min is use to represent NULL. 
        /// </summary>
        public DateTime currentActionStarted = DateTime.MinValue;

        public bool IsAtWork(DateTime time)
        {
            if (workDays.Contains(time.DayOfWeek))
            {
                var todayClockIn = new DateTime(time.Year, time.Month, time.Day, clockIn.Hour, clockIn.Minute, clockIn.Second);
                var todayClockOut = new DateTime(time.Year, time.Month, time.Day, clockOut.Hour, clockOut.Minute, clockOut.Second);
                
                if(todayClockIn <= time && time < todayClockOut)
                {
                    return true;
                }
            }

            return false;
        }

        public void Work(DateTime time, TimeSpan timestep)
        {

            if (! IsAtWork(time))
            {
                if(state != State.Idle)
                {
                    state = State.Idle;
                    stateEntered = time;
                }
                return;
            }

            if (stateEntered == DateTime.MinValue)
            {
                stateEntered = time;
            }

            switch (state)
            {
                case State.Idle:
                    FromIdle(time, timestep);
                    break;
                case State.Switching:
                    break;
                case State.Working:
                    FromWork(time, timestep);
                    break;
                default:
                    break;
            }
        }

        private void FromWork(DateTime time, TimeSpan timestep)
        {
            //Assert that we have a task within schedule

            var currentTask = personalSchedule.Peek();
            var i = currentTask.process.actionIndex;
            if(currentTask.process.Count > i)
            {
                if(currentActionStarted == DateTime.MinValue)
                {
                    currentActionStarted = time;
                    Console.WriteLine(this.GetHashCode() + ": Started Action - " + time.ToString("yyyy-MM-dd hh:mm:ss"));

                }

                var currectAction = currentTask.process[i];
                if(time + timestep > currentActionStarted + currectAction.untilComplete)
                {
                    currentTask.process.actionIndex += 1;
                    currentActionStarted = DateTime.MinValue;

                    Console.WriteLine(this.GetHashCode() + ": Finished Action - " + time.ToString("yyyy-MM-dd hh:mm:ss"));

                    if (currentTask.process.Count == currentTask.process.actionIndex)
                    {
                        var finishedTask = personalSchedule.Dequeue();
                        finishedTask.complete = true;
                        Console.WriteLine(this.GetHashCode() + ": Finished Task - " + time.ToString("yyyy-MM-dd hh:mm:ss"));

                        if (!personalSchedule.Any())
                        {

                            // !! State Change !!
                            state = State.Idle;
                            stateEntered = DateTime.MinValue;
                        }
                    }
                }
            }
        }

        public void FromIdle(DateTime time, TimeSpan timestep)
        {

            // Making the schedule simple and dumb
            if(personalSchedule.Count != personalTaskList.Count(task=>!task.complete))
            {
                var schedule = personalSchedule.ToHashSet();
                var missingTasks = personalTaskList.Where(task => !task.complete && !schedule.Contains(task));

                foreach (var task in missingTasks)
                {
                    personalSchedule.Enqueue(task);
                }
            }

            if(personalSchedule.Any())
            {
                if(time + timestep > stateEntered + contextSwitchPenalty)
                {
                    // !! State Change !!
                    state = State.Working;
                    stateEntered = DateTime.MinValue;
                }
            }
        }

        public void HandleInterruption(DateTime time, TimeSpan timestep)
        {
            // We are deciding that everything in the current timestep will complete
        }
    }

}
