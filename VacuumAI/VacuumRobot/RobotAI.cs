﻿using Environment;
using MainProgram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VacuumRobot
{
    /// <summary>
    /// Create a new RobotAI.
    /// </summary>
    public partial class RobotAI
    {
        /// <summary>
        /// Name of the robot.
        /// </summary>
        private string m_sName;
        /// <summary>
        /// Nomber of points of the robot. Robot dies if it reaches 0.
        /// Each action deplete this number by one unit.
        /// </summary>
        private int m_iPointsCount;
        /// <summary>
        /// Path the robot will follow through the environment.
        /// </summary>
        private Square[] m_asPathArray = null;
        /// <summary>
        /// Current index in the m_asPathArray array.
        /// </summary>
        private int m_iCurrentPositionIndex = 0;

        /// <summary>
        /// Create a new robot AI.
        /// </summary>
        /// <param name="p_sName"> Robot name. </param>
        /// <param name="p_iPointsCount"> Robot point count. </param>
        /// <param name="p_asPathArray"> Path that will be followed by the robot. </param>
        public RobotAI(string p_sName, int p_iPointsCount, Square[] p_asPathArray)
        {
            m_iPointsCount = p_iPointsCount;
            m_sName = p_sName;
            m_asPathArray = p_asPathArray;
        }

        /// <summary>
        /// Finds out if the robot is still alive, i.e. if its PointsCount is superior to 0.
        /// </summary>
        /// <returns> Return true if the robot is still alive. </returns>
        public bool AmIAlive()
        {
            bool result = false;
            if (m_iPointsCount > 0)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Return the current state of the Environment from the robot's point of view.
        /// The state of the Environment can return the following values :
        /// 0 : Nothing on the Square.
        /// 1 : Only dust on the Square.
        /// 2 : Only jewels on the Square.
        /// 3 : Both dust and jewel on the Square.
        /// </summary>
        /// <returns> Return an int array as followed [Index of the current Square, State of the current Square]. </returns>
        public int[] GetEnvironmentState()
        {
            bool bDust = Sensor.HasDust(m_asPathArray[m_iCurrentPositionIndex]);
            bool bJewel = Sensor.HasJewel(m_asPathArray[m_iCurrentPositionIndex]);

            int iResultState = 0;

            if ((bDust == true) && (bJewel == false))
            {
                iResultState = 1;
            }
            if ((bDust == false) && (bJewel == true))
            {
                iResultState = 2;
            }
            if ((bDust == true) && (bJewel == true))
            {
                iResultState = 3;
            }

            int[] aiResult = { m_iCurrentPositionIndex, iResultState };
            return aiResult;
        }

        /// <summary>
        /// Move the robot to the next square in m_asPathArray.
        /// </summary>
        public void Move()
        {
            if (m_asPathArray != null)
            {
                m_asPathArray[m_iCurrentPositionIndex].HasVacuum = false;

                if (m_iCurrentPositionIndex < (m_asPathArray.Length - 1))
                {
                    m_iCurrentPositionIndex++;
                    Console.Write("Robot moving from square " + m_asPathArray[m_iCurrentPositionIndex - 1].NumSquare +
                    " to square " + m_asPathArray[m_iCurrentPositionIndex].NumSquare + "\n");
                }
                else
                {
                    Console.Write("Robot moving from square " + m_asPathArray[m_iCurrentPositionIndex].NumSquare +
                    " to square " + m_asPathArray[0].NumSquare + "\n");
                    m_iCurrentPositionIndex = 0;
                }
                m_iPointsCount--;
                Console.Write("Arrived in new room. New points count : " + m_iPointsCount + "\n");
                m_asPathArray[m_iCurrentPositionIndex].HasVacuum = true;
            }
        }

        /// <summary>
        /// Remove dust from the current square.
        /// </summary>
        public void RemoveDust()
        {
            Console.Write("Hoovering in progress...\n");
            Thread.Sleep(500);
            Actuator.PickUpDust(m_asPathArray[m_iCurrentPositionIndex]);
            Actuator.PickUpJewel(m_asPathArray[m_iCurrentPositionIndex]);
            // Add 2 points and remove 1 for action.
            //m_iPointsCount--;
            m_iPointsCount++;
            Console.Write("Hoovering done. New points count : "+ m_iPointsCount + "\n");
        }

        /// <summary>
        /// Remove jewel from the current square.
        /// </summary>
        public void RemoveJewel()
        {
            Console.Write("Picking up jewels...\n");
            Thread.Sleep(500);
            Actuator.PickUpJewel(m_asPathArray[m_iCurrentPositionIndex]);
            // Add 4 points and remove 1 for action.
            //m_iPointsCount--;
            m_iPointsCount = m_iPointsCount + 3;
            Console.Write("Picking jewels done. New points count : " + m_iPointsCount + "\n");
        }

        /// <summary>
        /// Processes the action the agent MAY undertake, given a state of the environment.
        /// </summary>
        /// <param name="p_iStateEnv"> The current state of the environment. </param>
        /// <returns> The list of every action possible for the agent. </returns>
        public List<ActionPossible> ActionDeclenchable(int[] p_iStateEnv)
        {
            // Declares the four possible actions, and add them to the list of possible actions.
            ActionPossible apActToSuck = new Aspirate(this);
            ActionPossible apActToMove = new MoveRobot(this);
            ActionPossible apActToGrab = new Grab(this);
            ActionPossible apActToDoNothing = new DoNothing(this);
            List<ActionPossible> aListActionPossible = new List<ActionPossible>();
            aListActionPossible.Add(apActToSuck);
            aListActionPossible.Add(apActToMove);
            aListActionPossible.Add(apActToGrab);
            aListActionPossible.Add(apActToDoNothing);

            // If the environment is composed of dust only.
            if (p_iStateEnv[1] == 0)
            {
                /* Here we would delete the actions that the agent
                 * can't undertake in the current environment. 
                 * In our situation, all action are theoritically possible however. */
            }
            // If the environment is composed of jewels only
            if (p_iStateEnv[1] == 1)
            {
                /* Here we would delete the actions that the agent
                 * can't undertake in the current environment. 
                 * In our situation, all action are theoritically possible however. */
            }
            // If the environment is composed of dust and jewels.
            if (p_iStateEnv[1] == 2)
            {
                /* Here we would delete the actions that the agent
                 * can't undertake in the current environment. 
                 * In our situation, all action are theoritically possible however. */
            }
            // If the environment is composed of neither dust or jewels
            if (p_iStateEnv[1] == 3)
            {
                /* Here we would delete the actions that the agent
                 * can't undertake in the current environment. 
                 * In our situation, all action are theoritically possible however. */
            }
            return aListActionPossible;
        }

        /// <summary>
        /// Choose an action for the agent to perform, based on its goal and environment.
        /// </summary>
        /// <param name="p_iStateEnv"> Current state of the environment surrounding the agent. </param>
        /// <param name="p_iMyGoal"> Current goal the agent is trying to achieve. </param>
        /// <returns> The best choice action in the current context, that will help the agent to achieve its goal. </returns>
        public ActionPossible DetermineActionUponMyGoal(int[] p_iStateEnv, int p_iMyGoal)
        {
            // This list contains each possible action for the agent, regardless of their relevance
            List<ActionPossible> lapListActionPossible = ActionDeclenchable(p_iStateEnv);

            // Initialises the index used to keep track of the best action to perform.
            int iIndexActionToDo = -1;

            for (int i = 0; i < lapListActionPossible.Count; i++)
            {
                // Each iteration, initialises the worthiness of the action currently evaluated.
                int iWorthiness = -1;

                if (iWorthiness < CalculateWorthiness(lapListActionPossible[i], p_iMyGoal, p_iStateEnv))
                {
                    // If the current action is the most relevant, we keep its index in the list.
                    iIndexActionToDo = i;
                }
            }
            // When we went through the whole list, the index returned is the one of the most relevant action.
            return lapListActionPossible[iIndexActionToDo];
        }

        public void DoAction(ActionPossible p_apAction )
        {
            p_apAction.Act();
        }

        /// <summary>
        /// Calculate the worthiness of an action, based on the parameters given.
        /// </summary>
        /// <param name="p_apAction"> Input action that needs to be evaluated. </param>
        /// <param name="p_iMyGoal"> Current goal the agent is trying to achieve. </param>
        /// <param name="p_aiStateEnv"> Current state of the environment surrounding the agent. </param>
        /// <returns> Returns the optimal worthiness calculated for the specified action. </returns>
        public int CalculateWorthiness(ActionPossible p_apAction, int p_iMyGoal, int[] p_aiStateEnv)
        {
            // Initialises several counter for worthiness, each associated with a different goal
            int iWorthinessRegardingJewelsAndCleanliness = -1;
            int iWorthinessRegardingSpeed = -1;
            int iWorthinessRegardingElectricitySave = -1;

            /* Depending on the state of the environment and the goal, we logically attibute a number of point to each counter
             * This way, a particularly interesting action to perform will be set a high value of worthiness */
            switch (p_aiStateEnv[1])
            {
                // If there is nothing on the Square.
                case 0:
                    if (p_apAction.Name() == "Aspirate")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 0; iWorthinessRegardingElectricitySave = 0; iWorthinessRegardingSpeed = 0;
                    }
                    if (p_apAction.Name() == "MoveRobot")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 3; iWorthinessRegardingElectricitySave = 0; iWorthinessRegardingSpeed = 3;
                    }
                    if (p_apAction.Name() == "Grab")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 0; iWorthinessRegardingElectricitySave = 0; iWorthinessRegardingSpeed = 0;
                    }
                    if (p_apAction.Name() == "DoNothing")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 0; iWorthinessRegardingElectricitySave = 5; iWorthinessRegardingSpeed = 0;
                    }
                    break;
                // If there is dust only.
                case 1:
                    if (p_apAction.Name() == "Aspirate")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 0; iWorthinessRegardingElectricitySave = 1; iWorthinessRegardingSpeed = 3;
                    }
                    if (p_apAction.Name() == "MoveRobot")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 0; iWorthinessRegardingElectricitySave = 0; iWorthinessRegardingSpeed = 0;
                    }
                    if (p_apAction.Name() == "Grab")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 5; iWorthinessRegardingElectricitySave = 1; iWorthinessRegardingSpeed = 3;
                    }
                    if (p_apAction.Name() == "DoNothing")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 0; iWorthinessRegardingElectricitySave = 5; iWorthinessRegardingSpeed = 0;
                    }
                    break;
                // If there are jewels only.
                case 2:
                    if (p_apAction.Name() == "Aspirate")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 0; iWorthinessRegardingElectricitySave = 1; iWorthinessRegardingSpeed = 5;
                    }
                    if (p_apAction.Name() == "MoveRobot")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 0; iWorthinessRegardingElectricitySave = 0; iWorthinessRegardingSpeed = 0;
                    }
                    if (p_apAction.Name() == "Grab")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 5; iWorthinessRegardingElectricitySave = 0; iWorthinessRegardingSpeed = 1;
                    }
                    if (p_apAction.Name() == "DoNothing")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 0; iWorthinessRegardingElectricitySave = 5; iWorthinessRegardingSpeed = 0;
                    }
                    break;
                // If there are both jewels and dust.
                case 3:
                    if (p_apAction.Name() == "Aspirate")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 5; iWorthinessRegardingElectricitySave = 1; iWorthinessRegardingSpeed = 5;
                    }
                    if (p_apAction.Name() == "MoveRobot")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 0; iWorthinessRegardingElectricitySave = 0; iWorthinessRegardingSpeed = 0;
                    }
                    if (p_apAction.Name() == "Grab")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 1; iWorthinessRegardingElectricitySave = 1; iWorthinessRegardingSpeed = 3;
                    }
                    if (p_apAction.Name() == "DoNothing")
                    {
                        iWorthinessRegardingJewelsAndCleanliness = 0; iWorthinessRegardingElectricitySave = 5; iWorthinessRegardingSpeed = 0;
                    }
                    break;
                default:
                    break;
            }
            // Then, we return the counter associated with the current goal of the agent.
            int result = -1;
            if (p_iMyGoal == 0)
            {
                result = iWorthinessRegardingJewelsAndCleanliness;
            }
            else if (p_iMyGoal == 1)
            {
                result = iWorthinessRegardingSpeed;
            }
            else if (p_iMyGoal == 2)
            {
                result = iWorthinessRegardingElectricitySave;
            }
            return result;
        }

    }
}
