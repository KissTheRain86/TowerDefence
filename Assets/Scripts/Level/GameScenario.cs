using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu]
public class GameScenario : ScriptableObject
{
    [SerializeField]
    EnemyWave[] waves = { };

    [SerializeField]
    int cycles = 1;

    [SerializeField, Range(0f, 1f)]
    float cycleSpeedUp;//每一轮后加速
    public State Begin() => new State(this);

    [Serializable]
    public struct State
    {
        GameScenario scenario;
        int cycle, index;
        EnemyWave.State wave;
        float timeScale;

        public State(GameScenario scenario)
        {
            this.scenario = scenario;
            index = 0;
            cycle = 0;
            timeScale = 1;
            Debug.Assert(scenario.waves.Length > 0, "Empty scenatio");
            wave = scenario.waves[0].Begin();
        }

        public bool Progress()
        {
            //执行当前wave 得到剩余时间
            float deltaTime = wave.Progress(timeScale * Time.deltaTime);
            while (deltaTime >= 0)
            {
                if (++index >= scenario.waves.Length)
                {
                    if(++cycle>=scenario.cycles && scenario.cycles > 0)
                    {
                        return false;
                    }
                    index = 0;
                    timeScale += scenario.cycleSpeedUp;
                }
                wave = scenario.waves[index].Begin();
                deltaTime = wave.Progress(deltaTime);
            }
            return true;
        }
    }
}