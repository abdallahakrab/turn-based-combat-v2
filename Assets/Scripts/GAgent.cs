﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;
public class SubGoal
{
    public Dictionary<string, int> sgoals;
    public bool remove;

    public SubGoal(string s, int i, bool r)
    {
        sgoals = new Dictionary<string, int>();
        sgoals.Add(s, i);
        remove = r;
    }
}
public class GAgent : MonoBehaviour
{
    public List<GAction> actions = new List<GAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();

    GPlanner planner;
    Queue<GAction> actionQueue;
    public GAction currentAction;
    SubGoal currentGoal;

    Seeker seeker;
    GameObject player;
    Unit playerUnit;
    private PlayerController playerController;

    Animator anim;
    BattleSystem BS;
    // Start is called before the first frame update
    public void Start()
    {
        BS = BattleSystem.instance;
        anim = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();
        playerController = GetComponent<PlayerController>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerUnit = player.GetComponent<Unit>();
        //enemy = GameObject.FindGameObjectWithTag("Enemy");
        GAction[] acts = GetComponents<GAction>();
        foreach(GAction a in acts)
        {
            actions.Add(a);
            //Debug.Log(a.actionName);
        }

    }

    bool invoked = false;
    void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
    }



    void Move(Vector3 current, Vector3 next)
    {
        //Debug.Log(current);
        //Debug.Log(next);
        float xOld = current.x + 0.05f;
        float yOld = current.y + 0.05f;
        float xNew = next.x;
        float yNew = next.y;
        float dy = Mathf.Abs(yNew - yOld);
        float dx = Mathf.Abs(xNew - xOld);

        //Vector3 _move = new Vector3(0, 0, 0);

        //Left
        if (xNew < xOld && dy < dx)
        {
            //if (transform.localScale.x > 0)
            //{
            //    _move = new Vector3(10, 0, 0);
            //}

            //if (transform.localScale.x < 0)
            //{
            //    _move = new Vector3(-10, 0, 0);
            //}
            //anim.SetTrigger("MoveLeft");
            //transform.Translate(new Vector3(-1f,0,0));
            //playerController.moveLeft();
            anim.SetTrigger("MoveLeft");
            transform.Translate(new Vector3(-1f, 0, 0));
        }

        //////Right
        ////// Debug.Log(xNew);

        ////// Debug.Log(xOld);
        ////// Debug.Log(yNew);
        ////// Debug.Log(yOld);

        if (xNew > xOld && dy < dx)
        {
            anim.SetTrigger("MoveRight");
            transform.Translate(new Vector3(1f, 0, 0));
            //playerController.moveRight();
        }
        //Up
        if (yNew > yOld && dx < dy)
        {
            anim.SetTrigger("MoveUp");
            transform.Translate(new Vector3(0, 1f, 0));
            //playerController.moveUp();
        }

        //Down
        if (yNew < yOld && dx < dy)
        {
            anim.SetTrigger("MoveDown");
            transform.Translate(new Vector3(0, -1f, 0));
            //playerController.moveDown();
        }
        //BS.state = BattleState.ENEMYTURN;
    }


    bool moved = false;
    void LateUpdate()
    {
        if (currentAction != null && currentAction.running)
        {
            if (BS.state == BattleState.PLAYERTURN)
            {
                //Debug.Log("PLayerTurn");
                if (currentAction.actionName == "Move")
                {
                    //Debug.Log("Move");
                    var path = seeker.StartPath(player.transform.position, currentAction.target.transform.position);
                    AstarPath.BlockUntilCalculated(path);
                    float distanceToTarget = Vector3.Distance((Vector3)path.path[1].position, currentAction.target.transform.position);
                    //Debug.Log("DistanceToTarget" + distanceToTarget);
                    //Debug.Log(path);
                    if (path.path.Count > 2 && distanceToTarget >= 1.1f)
                    {
                        //Debug.Log("Path exists");

                        //if (!invoked)
                        //{
                        //    Invoke("CompleteAction", currentAction.duration);
                        //    invoked = true;
                        //}
                        Move(player.transform.position, currentAction.target.transform.position);
                        //BS.SwitchTurn();
                    }
                    else
                    {
                        if (!invoked)
                        {
                            Invoke("CompleteAction", currentAction.duration);
                            invoked = true;
                        }
                    }

                }
                else if (currentAction.actionName == "Attack")
                {
                    //Debug.Log("Attacking");
                    playerUnit.spells[2].CastSpell(playerUnit, currentAction.target.GetComponent<Unit>());

                    if (!invoked)
                    {
                        Invoke("CompleteAction", currentAction.duration);
                        invoked = true;
                    }
                }
                else if (currentAction.actionName == "Heal")
                {
                    playerUnit.spells[3].CastSpell(playerUnit, currentAction.target.GetComponent<Unit>());

                    if (!invoked)
                    {
                        Invoke("CompleteAction", currentAction.duration);
                        invoked = true;
                    }
                }
                else if(currentAction.actionName == "Kill")
                {
                    if (currentAction.target.GetComponent<Unit>().currentHP == 0)
                    {
                        if (!invoked)
                        {
                            Invoke("CompleteAction", currentAction.duration);
                            invoked = true;
                        }
                    }
                }
                else if (currentAction.actionName == "MoveBackAndHeal")
                {
                    //Debug.Log("MoveBackAndHeal");
                    if (!moved)
                    {
                        Vector3 playerPosition = transform.position;
                        Vector3 enemyPosition = currentAction.target.transform.position;
                        float absoluteDifferenceX = Mathf.Abs(playerPosition.x - enemyPosition.x);
                        float absoluteDifferenceY = Mathf.Abs(playerPosition.y - enemyPosition.y);
                        if (absoluteDifferenceX > absoluteDifferenceY)
                        {
                            if (playerPosition.x > enemyPosition.x)
                            {
                                anim.SetTrigger("MoveRight");
                                transform.Translate(new Vector3(1f, 0, 0));
                            }
                            else
                            {
                                anim.SetTrigger("MoveLeft");
                                transform.Translate(new Vector3(-1f, 0, 0));
                            }
                        }
                        else
                        {
                            if (playerPosition.y > enemyPosition.y)
                            {
                                anim.SetTrigger("MoveUp");
                                transform.Translate(new Vector3(0, 1f, 0));
                            }
                            else
                            {
                                anim.SetTrigger("MoveDown");
                                transform.Translate(new Vector3(0, -1f, 0));
                            }
                        }
                        moved = true;
                    }
                    else
                    {
                        playerUnit.spells[3].CastSpell(playerUnit, currentAction.target.GetComponent<Unit>());
                        if (!invoked)
                        {
                            Invoke("CompleteAction", currentAction.duration);
                            invoked = true;
                            moved = false;
                        }
                        
                    }
                    
                }
                BS.SwitchTurn();
            }
      

            return;
        }
        if(planner == null || actionQueue == null)
        {
            planner = new GPlanner();
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;
            foreach(KeyValuePair<SubGoal, int> sg in sortedGoals)
            {
                actionQueue = planner.plan(actions, sg.Key.sgoals, null);
                if (actionQueue != null)
                {
                    currentGoal = sg.Key;
                    break;
                }
            }
        }
        if(actionQueue != null && actionQueue.Count == 0)
        {
            if(currentGoal.remove)
            {
                goals.Remove(currentGoal);
            }
            planner = null;
        }
        if(actionQueue != null && actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();
            if (currentAction.PrePerform())
            {
                if (currentAction.target == null && currentAction.targetTag != "")
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);

                if(currentAction.target != null)
                {
                    currentAction.running = true;
                    //Move(player.transform.position, currentAction.target.transform.position);
                    //Debug.Log("HHHHHEEEELLLLOOOO");
                    
                }
            }
            else
            {
                actionQueue = null;
            }
        }
    }
}
