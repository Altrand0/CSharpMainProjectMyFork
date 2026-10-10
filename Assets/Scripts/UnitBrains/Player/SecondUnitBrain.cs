using Codice.Client.Common.FsNodeReaders;
using Model;
using Model.Runtime.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using static UnityEngine.GraphicsBuffer;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public List<Vector2Int> mostDangerousTargets = new();       // Новое поле для хранения целей, к которым нужно идти, но которые вне зоны досягаемости
        public List<Vector2Int> CurrentTargets = new();
        public override string TargetUnitName => "Cobra Commando";  //Имя юнита
        static public int CounterID = 0;                            // Счётчик ID
        public int unitID = CounterID++ % MaxUnitID;                // Логика ID: юниты получают ID от 0 до 2-х
    
        //public int unitID = CounterID < MaxUnitID ? CounterID++ : (CounterID = 0); // Моя авторская реализация, тут сам Counter циклится, переделал выше  :D
        const int MaxUnitID = 3;                                    // Ограничитель для ID
        private const float OverheatTemperature = 3f;               // Для механики перегрева
        private const float OverheatCooldown = 2f;                  // Для механики перегрева
        private float _temperature = 0f;                            // Для механики перегрева
        private float _cooldownTime = 0f;                           // Для механики перегрева
        private bool _overheated;                                   // Для механики перегрева

        
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList) //Метод генерации снарядов юнита
        {
            float overheatTemperature = OverheatTemperature;    // Для механики перегрева
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////     
            int currentTemperature = GetTemperature();  // Для механики перегрева
            if (currentTemperature >= overheatTemperature)  // 
            {                                               //
                return;                                     //
            }                                               //
            else                                            //
            {                                               //
                IncreaseTemperature();                      // Механика перегрева
            }                                               //
            currentTemperature = GetTemperature();          //
            for (int i = 0; i < currentTemperature; i++)    //
            {                                               //
                var projectile = CreateProjectile(forTarget);//
                AddProjectileToList(projectile, intoList);  //
            }                                               //
            ///////////////////////////////////////         // 
        }

        public override Vector2Int GetNextStep() // Поиск пути
        {
            //Получить цель из списка целей
            if ((CurrentTargets.Count == 0) || isReachable(CurrentTargets[unitID % CurrentTargets.Count])) // деления на ноль не будет, если CurrentTargets.Count равно нулю, вторая половина выражения уже не выполняется
            {
                return unit.Pos;
            }
            else
            {
                return unit.Pos.CalcNextStepTowards(CurrentTargets[unitID % CurrentTargets.Count]); //Идти к следующей цели
            }            
            //return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets() //Логика выбора следующей цели
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            Debug.Log(unitID);
            CurrentTargets.Clear();                                                 // Очищаем список
            List<Vector2Int> result = new List<Vector2Int>();                       // Новый список для найденных целей
            CurrentTargets = GetAllTargets().ToList<Vector2Int>();                  // Заполняем новый список       
            if (CurrentTargets.Any())                                               // Удобная функция из приложения к домашке для проверки, есть ли в списке элементы
            {
                SortByDistanceToOwnBase(CurrentTargets);                            //Эта функция из видео к домашке заменяет предыдущий код из поиска минимальной дистанции
                if (isReachable(CurrentTargets[unitID % CurrentTargets.Count]))     //Проверяем что количество целей не меньше чем текущий ID 
                {
                    result.Add(CurrentTargets[unitID % CurrentTargets.Count]);      //Номер цели - ID юнита, если пройдёт проверку как выше^
                }
                else if(isReachable(CurrentTargets[0]))
                { 
                    result.Add(CurrentTargets[0]);      // Если целей, совпадающих с ID не осталось, (Например id = 2, а целей две: 0-ая и 1-ая) добавляем любую ближайшую к базе
                }

            }else         //перестраховка, вдруг базы не будет среди целей
            {
                Vector2Int enemyBase = runtimeModel.RoMap.Bases[IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];
                CurrentTargets.Add(enemyBase);      
            }
            return result;
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }
            bool isReachable(Vector2Int target)
            {
            List<Vector2Int> reachableTargets = GetReachableTargets();//Получаем все цели в зоне досягаемости
            foreach(Vector2Int reachableTarget in reachableTargets)
                {
                    if (reachableTarget == target)// Если переданная цель совпадает с одной из тех что получаем как досягаемые
                    {
                        return true; // Значит она тоже досягаемая
                    }
                }
                return false;
            }

            ///////////////////////////////////////
        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}