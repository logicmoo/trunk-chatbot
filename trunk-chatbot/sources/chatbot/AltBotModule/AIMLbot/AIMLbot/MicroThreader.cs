﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.IO;

namespace AltAIMLbot
{
    //see http://mjhutchinson.com/journal/2010/02/01/iteratorbased_microthreading

    //tasks may move between lists but they may only be in one list at a time
    public class TaskItem
    {
        public readonly IEnumerator<RunStatus > Task;
        public TaskItem Next;
        public Scheduler Scheduler;
        public long Data;
        public string name;

        public TaskItem(IEnumerator<RunStatus> task, Scheduler scheduler)
        {
            this.Task = task;
            this.Scheduler = scheduler;
        }
        public TaskItem(IEnumerator<RunStatus> task, Scheduler scheduler,string myName)
        {
            this.Task = task;
            this.Scheduler = scheduler;
            this.name = myName;
        }
    }

    class TaskList
    {
        public readonly Scheduler Scheduler;

        public TaskItem First { get; private set; }
        public TaskItem Last { get; private set; }

        public TaskList(Scheduler scheduler)
        {
            this.Scheduler = scheduler;
        }

        public void Append(TaskItem task)
        {
            Debug.Assert(task.Next == null);
            if (First == null)
            {
                Debug.Assert(Last == null);
                First = Last = task;
            }
            else
            {
                Debug.Assert(Last.Next == null);
                Last.Next = task;
                Last = task;
            }
        }
        public int Count
        {
            get
            {
                int count=0;
                var en = GetEnumerator();
                while (en.MoveNext())
                {
                    count++;
                }
                return count;
            }
        }

        public void Remove(TaskItem task, TaskItem previous)
        {
            if (previous == null)
            {
                Debug.Assert(task == First);
                First = task.Next;
            }
            else
            {
                Debug.Assert(previous.Next == task);
                previous.Next = task.Next;
            }

            if (task.Next == null)
            {
                Debug.Assert(Last == task);
                Last = previous;
            }
            task.Next = null;
        }

        public TaskEnumerator GetEnumerator()
        {
            return new TaskEnumerator(this);
        }

        public sealed class TaskEnumerator
        {
            TaskList list;
            TaskItem current, previous;

            public TaskEnumerator(TaskList list)
            {
                this.list = list;
                previous = current = null;
            }

            public TaskItem Current { get { return current; } }

            public bool MoveNext()
            {
                TaskItem next;
                if (current == null)
                {
                    if (previous == null)
                        next = list.First;
                    else
                        next = previous.Next;
                }
                else
                {
                    next = current.Next;
                }

                if (next != null)
                {
                    if (current != null)
                        previous = Current;
                    current = next;
                    return true;
                }
                return false;
            }

            public void MoveCurrentToList(TaskList otherList)
            {
                otherList.Append(RemoveCurrent());
            }

            public TaskItem RemoveCurrent()
            {
                Debug.Assert(current != null);
                TaskItem ret = current;
                list.Remove(current, previous);
                current = null;
                return ret;
            }
        }
    }

    public sealed class Scheduler
    {
        TaskList active, sleeping;
        Servitor servitor;
        public bool singular = true; // only one process

        public Scheduler(Servitor myServitor)
        {
            servitor = myServitor;
            active = new TaskList(this);
            sleeping = new TaskList(this);
        }

        public void ActivateBehaviorTask(string name)
        {
            // if its already running or sleeping 
            string status =taskStatus(name) ;
            if (status == "running") return;
            if (status == "sleeping")
            {
                AwakenTask(name);
                return;
            }
            // start up a new one
            if (!servitor.curBot.myBehaviors.definedBehavior(name))
            {
                return;
            }
            IEnumerator<RunStatus> iterator = servitor.curBot.myBehaviors.getBehaviorEnumerator(name);
            
            if ((singular ==false) || (active.Count ==0))
            {
                active.Append(new TaskItem(iterator, this,name));
            }
            else
            {
                //Put in background if we are single minded
                sleeping.Append(new TaskItem(iterator, this, name));

            }

        }

        public void EnqueueEvent(string evnt)
        {
            string evntBehavior = servitor.curBot.myBehaviors.getEventHandler(evnt);
            if (evntBehavior == "")
            {
                return;
            }
            ActivateBehaviorTask(evntBehavior);
        }

        public void RemoveBehaviorTask(string name)
        {
            var en = sleeping.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current.name == name)
                {
                    en.RemoveCurrent();
                }
            }
            en = active.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current.name == name)
                {
                    en.RemoveCurrent();
                }
            }
        }
        public void SleepBehaviorTask(string name)
        {
            var en = active.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current.name == name)
                {
                    en.MoveCurrentToList(sleeping);
                }
            }
        }
        public void SleepBehaviorTask(string name, long msec)
        {
            long nowTicks = DateTime.Now.Ticks;
            long timeout = nowTicks + (msec * 10000);

            var en = active.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current.name == name)
                {
                    en.Current.Data = timeout;
                    en.MoveCurrentToList(sleeping);
                }
            }
        }
        public void SleepAllTasks()
        {
            var en = active.GetEnumerator();
            while (en.MoveNext())
            {
                    en.MoveCurrentToList(sleeping);
            }
        }

        public void SleepAllTasks(long msec)
        {
            long nowTicks = DateTime.Now.Ticks;
            long timeout = nowTicks + (msec * 10000);

            var en = active.GetEnumerator();
            while (en.MoveNext())
            {
                en.Current.Data = timeout;
                en.MoveCurrentToList(sleeping);
            }
            

        }
        public void ReviveAllTasks()
        {
            var en = sleeping.GetEnumerator();
            while (en.MoveNext())
            {
                en.MoveCurrentToList(active);
            }
        }

        public string idStatus(string nodeID)
        {
            string report ="non";
            if (!servitor.curBot.myBehaviors.runState.ContainsKey(nodeID))
            {
                report = "non";
            }
            else
            {
                report = servitor.curBot.myBehaviors.runState[nodeID].ToString();
            }
            return report;
         }

        public string taskStatus(string nodeID)
        {
            string report = "unknown";
            var en = sleeping.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current.name == nodeID)
                {
                    report="sleeping";
                    return report;
                }
            }
            en = active.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Current.name == nodeID)
                {
                    report = "active";
                    return report;
                }
            }
            return report;
        }

        public void AddTask(IEnumerator<RunStatus> task)
        {
            active.Append(new TaskItem(task, this));
        }

        public void AwakenTask(string taskName)
        {
            var en = sleeping.GetEnumerator();
            while (en.MoveNext())
                if (en.Current.name == taskName)
                    en.MoveCurrentToList(active);
        }

        public void Run()
        {
            //cache this, it's expensive to access DateTime.Now
            int sleepCount = sleeping.Count;
            int activeCount = active.Count;
            long nowTicks = DateTime.Now.Ticks;

            //move woken tasks back into the active list
            var en = sleeping.GetEnumerator();
            if ((singular == false) || (activeCount == 0))
            {
                while (en.MoveNext())
                    if (en.Current.Data < nowTicks)
                        en.MoveCurrentToList(active);
            }
            //run all the active tasks
            en = active.GetEnumerator();
            while (en.MoveNext())
            {
                //run each task's enumerator for one yield iteration
                IEnumerator<RunStatus>  t = en.Current.Task;
                if (!t.MoveNext())
                {
                    //it finished, so remove it
                    en.RemoveCurrent();
                    continue;
                }

                //check the current state
                object state = t.Current;
                if (state == null)
                {
                    //it's just cooperatively yielding, state unchanged
                    continue;
                }
                else if (state is RunStatus)
                {
                    if (t.Current == RunStatus.Running)
                    {
                        //it's just cooperatively yielding, state unchanged
                        continue;
                    }
                    if (t.Current == RunStatus.Failure )
                    {
                        //We're done, just not a positive outcome
                        en.RemoveCurrent();
                        continue;
                    }
                    if (t.Current == RunStatus.Success )
                    {
                        //We're done, and success!
                        en.RemoveCurrent();
                        continue;
                    }


                }
                else if (state is TimeSpan)
                {
                    //it wants to sleep, move to the sleeping list. we use the Data property for the wakeup time
                    en.Current.Data = nowTicks + ((TimeSpan)state).Ticks;
                    en.MoveCurrentToList(sleeping);
                }
                else if (state is IEnumerable<RunStatus>)
                {
                    throw new NotImplementedException("Nested tasks are not supported yet");
                }
                else if (state is Signal)
                {
                    TaskItem task = en.RemoveCurrent();
                    task.Data = 0;
                    ((Signal)state).Add(task);
                }
                else if (state is ICollection<Signal>)
                {
                    TaskItem task = en.RemoveCurrent();
                    task.Data = 0;
                    foreach (Signal s in ((ICollection<Signal>)state))
                        s.Add(task);
                }

                else
                {
                    throw new InvalidOperationException("Unknown task state returned:" + state.GetType().FullName);
                }
            }
        }

        internal void AddToActive(TaskItem task)
        {
            active.Append(task);
        }

        public void performAction(StreamWriter writer, string action, string query, string behaviorName)
        {
            string ids = "";
            string tsk = "";
            TaskList.TaskEnumerator  en = null;
            switch (action)
            {
                case "activate":
                    ActivateBehaviorTask(behaviorName);
                     ids = idStatus(behaviorName);
                     tsk = taskStatus(behaviorName);
                     writer.WriteLine("<status id=\"{0}\" idStatus=\"{1}\" taskStatus=\"{2}\" />", behaviorName, ids, tsk);
                    break;
                case "deactivate":
                    RemoveBehaviorTask(behaviorName);
                     ids = idStatus(behaviorName);
                     tsk = taskStatus(behaviorName);
                     writer.WriteLine("<status id=\"{0}\" idStatus=\"{1}\" taskStatus=\"{2}\" />", behaviorName, ids, tsk);
                    break;
                case "sleep":
                    SleepBehaviorTask(behaviorName);
                    ids = idStatus(behaviorName);
                    tsk = taskStatus(behaviorName);
                    writer.WriteLine("<status id=\"{0}\" idStatus=\"{1}\" taskStatus=\"{2}\" />", behaviorName, ids, tsk);
                    break;
                case "sleepall":
                    SleepAllTasks();
                    ids = idStatus(behaviorName);
                    tsk = taskStatus(behaviorName);
                    writer.WriteLine("<status id=\"{0}\" idStatus=\"{1}\" taskStatus=\"{2}\" />", behaviorName, ids, tsk);
                    break;
                case "reviveall":
                    ReviveAllTasks();
                    ids = idStatus(behaviorName);
                    tsk = taskStatus(behaviorName);
                    writer.WriteLine("<status id=\"{0}\" idStatus=\"{1}\" taskStatus=\"{2}\" />", behaviorName, ids, tsk);
                    break;

                case "status":
                     ids = idStatus(behaviorName);
                     tsk = taskStatus(behaviorName);
                     writer.WriteLine("<status id=\"{0}\" idStatus=\"{1}\" taskStatus=\"{2}\" />", behaviorName, ids, tsk);
                    break;
                case "liststatus":
                    en = sleeping.GetEnumerator();
                    while (en.MoveNext())
                    {
                            writer.WriteLine("<status id=\"{0}\" taskStatus=\"{1}\" />", en.Current.name, "sleeping");
                    }
                    en = active.GetEnumerator();
                    while (en.MoveNext())
                    {
                        writer.WriteLine("<status id=\"{0}\" taskStatus=\"{1}\" />", en.Current.name, "active");
                    }
                    break;
                case "listidstatus":
                    foreach (string key in servitor.curBot.myBehaviors.runState.Keys)
                    {
                        string status= servitor.curBot.myBehaviors.runState[key].ToString();
                        writer.WriteLine("<status id=\"{0}\" idStatus=\"{1}\" />", key, status);
                    }

                    break;
                case "stopall":
                    en = sleeping.GetEnumerator();
                    while (en.MoveNext())
                    {
                        writer.WriteLine("<status id=\"{0}\" taskStatus=\"{1}\" />", en.Current.name, "terminating");
                        en.RemoveCurrent();
                    }
                    en = active.GetEnumerator();
                    while (en.MoveNext())
                    {
                        writer.WriteLine("<status id=\"{0}\" taskStatus=\"{1}\" />", en.Current.name, "terminating");
                        en.RemoveCurrent();
                    }
                    break;

                 default  :
                    writer.WriteLine("<error action=\"{0}\" query=\"{1}\" behaviorName=\"{2}\" />", action, query, behaviorName);
 
                break;
            }
            writer.WriteLine("<fin/>");
            writer.Close();
        }
    }

    public class Signal
    {
        static int nextId = int.MinValue;

        int id = nextId++;
        List<TaskItem> tasks = new List<TaskItem>();
        bool isSet = true;

        public void Set()
        {
            if (isSet)
                return;
            isSet = true;
            //decrement the wait count of all tasks waiting for thsi signal
            foreach (TaskItem task in tasks)
                if (--task.Data == 0)
                    //if the wait count is zero, the task isn't waiting for any more signals, so re-schedule it
                    task.Scheduler.AddToActive(task);
            tasks.Clear();
        }

        internal void Add(TaskItem task)
        {
            //signal only becomes unset when it has tasks
            if (isSet)
                isSet = false;
            //the signal keeps a list of tasks that are waiting for it
            tasks.Add(task);
            //use the task's data for tracking the number of signals it's still waiting for
            task.Data++;
        }
    }



}
