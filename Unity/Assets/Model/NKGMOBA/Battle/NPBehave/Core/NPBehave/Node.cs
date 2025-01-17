﻿using System.Diagnostics;

namespace NPBehave
{
    public abstract class Node
    {
        public enum State
        {
            INACTIVE,
            ACTIVE,
            STOP_REQUESTED,
        }

        public State currentState = State.INACTIVE;

        public State CurrentState
        {
            get { return currentState; }
        }

        public Root RootNode;

        public Container parentNode;

        public Container ParentNode
        {
            get { return parentNode; }
        }

        public string label;

        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        public string name;

        public string Name
        {
            get { return name; }
        }

        public virtual Blackboard Blackboard
        {
            get { return RootNode.Blackboard; }
        }

        public virtual Clock Clock
        {
            get { return RootNode.Clock; }
        }

        public bool IsStopRequested
        {
            get { return this.currentState == State.STOP_REQUESTED; }
        }

        public bool IsActive
        {
            get { return this.currentState == State.ACTIVE; }
        }

        public Node(string name)
        {
            this.name = name;
        }

        public virtual void SetRoot(Root rootNode)
        {
            this.RootNode = rootNode;
        }

        public void SetParent(Container parent)
        {
            this.parentNode = parent;
        }

        public void Start()
        {
            // Assert.AreEqual(this.currentState, State.INACTIVE, "can only start inactive nodes, tried to start: " + this.Name + "! PATH: " + GetPath());
            Debug.Assert(this.currentState == State.INACTIVE, $"can only start inactive nodes  PATH: {GetPath()}");
            this.currentState = State.ACTIVE;
            DoStart();
        }

        /// <summary>
        /// 取消当前节点的执行，但并不返回状态结果
        /// </summary>
        public void CancelWithoutReturnResult()
        {
            //Assert.AreEqual(this.currentState, State.ACTIVE, "can only stop active nodes, tried to stop " + this.Name + "! PATH: " + GetPath());
            Debug.Assert(this.currentState == State.ACTIVE, $"can only stop active nodes, tried to stop  PATH: {GetPath()}");
            this.currentState = State.STOP_REQUESTED;
            DoCancel();
        }

        protected virtual void DoStart()
        {
        }

        protected virtual void DoCancel()
        {
        }

        /// <summary>
        /// 节点被终止，内含状态，成功或失败
        /// </summary>
        /// <param name="success"></param>
        protected virtual void Stopped(bool success)
        {
            // Assert.AreNotEqual(this.currentState, State.INACTIVE, "The Node " + this + " called 'Stopped' while in state INACTIVE, something is wrong! PATH: " + GetPath());
            Debug.Assert(this.currentState != State.INACTIVE,
                $"Called 'Stopped' while in state INACTIVE, something is wrong!  PATH: {GetPath()}");
            this.currentState = State.INACTIVE;
            if (this.ParentNode != null)
            {
                this.ParentNode.ChildStopped(this, success);
            }
        }

        public virtual void ParentCompositeStopped(Composite composite)
        {
            DoParentCompositeStopped(composite);
        }

        /// THIS IS CALLED WHILE YOU ARE INACTIVE, IT's MEANT FOR DECORATORS TO REMOVE ANY PENDING
        /// OBSERVERS
        protected virtual void DoParentCompositeStopped(Composite composite)
        {
            /// be careful with this!
        }

        // public Composite ParentComposite
        // {
        //     get
        //     {
        //         if (ParentNode != null && !(ParentNode is Composite))
        //         {
        //             return ParentNode.ParentComposite;
        //         }
        //         else
        //         {
        //             return ParentNode as Composite;
        //         }
        //     }
        // }

        override public string ToString()
        {
            return !string.IsNullOrEmpty(Label) ? (this.Name + "{" + Label + "}") : this.Name;
        }

        protected string GetPath()
        {
            if (ParentNode != null)
            {
                return ParentNode.GetPath() + "/" + Name;
            }
            else
            {
                return Name;
            }
        }
    }
}