using System;
using System.Collections.Generic;
using UnityEngine;

/// 有限状态机类：包含状态列表
/// 它持有者NPC的状态集合并且有添加，删除状态的方法，以及改变当前正在执行的状态
public class FSMSystem
{
    private Dictionary<Type, FSMState> statesDict = new Dictionary<Type, FSMState>();
    public FSMState CurrentState { get; private set; }

    /// 添加状态 并返回此状态
    public FSMState AddState<T>() where T : FSMState, new()
    {
        return AddState(new T());
    }

    /// 添加状态 并返回此状态
    public FSMState AddState(FSMState state)
    {
        if (state == null)
            Debug.LogError("FSM ERROR: 不可添加空状态");
        else if (statesDict.ContainsKey(state.GetType()))
            Debug.LogError($"FSM ERROR: 状态 {state.GetType()} 已存在，无法再次添加");
        else
        {
            // 设置初始状态  
            if (statesDict.Count == 0)
            {
                CurrentState = state;
                CurrentState.OnStateEnter();
            }

            // 设置所属状态机
            state.FsmSystem = this;
            statesDict.Add(state.GetType(), state);
        }

        return state;
    }

    /// 删除状态  
    public void DeleteState<T>() where T : FSMState
    {
        if (statesDict.ContainsKey(typeof(T)))
            statesDict.Remove(typeof(T));
        else
            Debug.LogError($"FSM ERROR: 状态 {typeof(T)} 不存在, 无法删除");
    }

    /// 转换状态  
    public void ChangeStateTo<T>() where T : FSMState
    {
        if (CurrentState is T)
        {
            UpdateCurrentState();
            return;
        }
        if (!CurrentState.ContainsTransitionTo<T>())
        {
            Debug.LogError($"FSM ERROR: 状态 {CurrentState} 不存在到状态 {typeof(T)} 的转换");
            return;
        }

        ForceChangeState<T>();
    }

    /// 无条件转换到T
    public void ForceChangeState<T>() where T : FSMState
    {
        if (!statesDict.ContainsKey(typeof(T)))
            Debug.LogError($"FSM ERROR: 状态机中没有状态 {typeof(T)}");
        else // 更新当前状态   
        {
            CurrentState.OnStateExit();
            CurrentState = statesDict[typeof(T)];
            CurrentState.OnStateEnter();
        }
    }

    // 更新状态
    public void UpdateCurrentState()
    {
        CurrentState.OnStateEnter();
    }
}