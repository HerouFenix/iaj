using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameManager
{
    public class GameManager : MonoBehaviour
    {
        private const float UPDATE_INTERVAL = 2.0f;
        //public fields, seen by Unity in Editor
        public GameObject character { get; private set; }
        public AutonomousCharacter autonomousCharacter { get; private set; }
        public PathfindingManager pathfindingManager;

        public Text HPText;
        public Text ShieldHPText;
        public Text ManaText;
        public Text TimeText;
        public Text XPText;
        public Text LevelText;
        public Text MoneyText;
        public Text DiaryText;
        public GameObject GameEnd;
        public bool StochasticWorld;
        public bool SleepingNPCs;

        //fields
        public List<GameObject> chests { get; set; }
        public List<GameObject> skeletons { get; set; }
        public List<GameObject> orcs { get; set; }
        public List<GameObject> dragons { get; set; }
        public List<GameObject> enemies { get; set; }
        public Dictionary<string, List<GameObject>> disposableObjects { get; set; }

        public CharacterData characterData { get; private set; }
        public bool WorldChanged { get; set; }
        private DynamicCharacter enemyCharacter;
        private GameObject currentEnemy;
 
        private float nextUpdateTime = 0.0f;
        private Vector3 previousPosition;
        public bool gameEnded = false;
        private float cellSize;

        public void Start()
        {
           

            pathfindingManager.Initialize();
            
            UpdateDisposableObjects();
            this.character = pathfindingManager.character;
            autonomousCharacter = character.GetComponent<AutonomousCharacter>();
            this.WorldChanged = false;
            this.characterData = new CharacterData(this.character);
            this.previousPosition = this.character.transform.position;
            cellSize = pathfindingManager.cellSize;
        }

        public void UpdateDisposableObjects()
        {
            this.enemies = new List<GameObject>();
            this.disposableObjects = new Dictionary<string, List<GameObject>>();

            this.chests = GameObject.FindGameObjectsWithTag("Chest").ToList();
            this.skeletons = GameObject.FindGameObjectsWithTag("Skeleton").ToList();
            this.enemies.AddRange(this.skeletons);
            this.orcs = GameObject.FindGameObjectsWithTag("Orc").ToList();
            this.enemies.AddRange(this.orcs);
            this.dragons = GameObject.FindGameObjectsWithTag("Dragon").ToList();
            this.enemies.AddRange(this.dragons);

            //adds all enemies to the disposable objects collection
            foreach (var enemy in this.enemies)
            {
                if (disposableObjects.ContainsKey(enemy.name))
                {
                    this.disposableObjects[enemy.name].Add(enemy);
                }
                else this.disposableObjects.Add(enemy.name, new List<GameObject>() { enemy });
            }
            //add all chests to the disposable objects collection
            foreach (var chest in this.chests)
            {
                if (disposableObjects.ContainsKey(chest.name))
                {
                    this.disposableObjects[chest.name].Add(chest);
                }
                else this.disposableObjects.Add(chest.name, new List<GameObject>() { chest });
            }
            //adds all health potions to the disposable objects collection
            foreach (var potion in GameObject.FindGameObjectsWithTag("HealthPotion"))
            {
                if (disposableObjects.ContainsKey(potion.name))
                {
                    this.disposableObjects[potion.name].Add(potion);
                }
                else this.disposableObjects.Add(potion.name, new List<GameObject>() { potion });
            }
            //adds all mana potions to the disposable objects collection
            foreach (var potion in GameObject.FindGameObjectsWithTag("ManaPotion"))
            {
                if (disposableObjects.ContainsKey(potion.name))
                {
                    this.disposableObjects[potion.name].Add(potion);
                }
                else this.disposableObjects.Add(potion.name, new List<GameObject>() { potion });
            }
        }

        public void Update()
        {

            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + UPDATE_INTERVAL;
                this.characterData.Time += UPDATE_INTERVAL;
            }

            if (!this.SleepingNPCs)
            {
                
                if (enemyCharacter != null && currentEnemy != null && currentEnemy.activeSelf)
                {
                    if (this.enemyCharacter.Movement != null)
                    {
                        this.enemyCharacter.Update();
                        this.SwordAttack(currentEnemy);
                    }
                }
                else
                {
                    foreach (var enemy in this.enemies)
                    {
                        if ((enemy.transform.position - this.character.transform.position).sqrMagnitude <= 1000)
                        {
                            this.currentEnemy = enemy;
                            this.enemyCharacter = new DynamicCharacter(enemy)
                            {
                                MaxSpeed = 50
                            };
                            List<NodeRecord> enemySolution = new List<NodeRecord>();
                            this.pathfindingManager.InitializeSearch(this.enemyCharacter.KinematicData.position, character.transform.position, "Enemy");
                            var finished = this.pathfindingManager.pathfinding.Search(out enemySolution, true);

                            var currentSolution = pathfindingManager.CalculateSolution(enemySolution);
                            var smoothPath = pathfindingManager.smoothPath(enemy.transform.position, currentSolution);
                            smoothPath.CalculateLocalPathsFromPathPositions(enemy.transform.position);
                            pathfindingManager.finished = false;

                            enemyCharacter.Movement = new DynamicFollowPath(enemyCharacter.KinematicData, smoothPath)
                            {
                                MaxAcceleration = 150.0f,
                                MaxSpeed = 30.0f
                            };

                            enemy.GetComponent<NPC>().character = enemyCharacter;

                            break;


                        }
                           
                           

                        
                    }
                }
            }

            this.HPText.text = "HP: " + this.characterData.HP;
            this.XPText.text = "XP: " + this.characterData.XP;
            this.ShieldHPText.text = "Shield HP: " + this.characterData.ShieldHP;
            this.LevelText.text = "Level: " + this.characterData.Level;
            this.TimeText.text = "Time: " + this.characterData.Time;
            this.ManaText.text = "Mana: " + this.characterData.Mana;
            this.MoneyText.text = "Money: " + this.characterData.Money;

            if(this.characterData.HP <= 0 || this.characterData.Time >= 200)
            {
                this.GameEnd.SetActive(true);
                this.gameEnded = true;
                this.GameEnd.GetComponentInChildren<Text>().text = "You Died";
            }
            else if(this.characterData.Money >= 25)
            {
                this.GameEnd.SetActive(true);
                this.gameEnded = true;
                this.GameEnd.GetComponentInChildren<Text>().text = "Victory \n GG EZ";
            }
        }

        public void SwordAttack(GameObject enemy)
        {
            int damage = 0;

            NPC enemyData = enemy.GetComponent<NPC>();

            if (enemy != null && enemy.activeSelf && InMeleeRange(enemy))
            {
                this.autonomousCharacter.DiaryText.text += Time.time + " I Sword Attacked " + enemy.name + "\n";

                if (this.StochasticWorld)
                {
                    damage = enemyData.dmgRoll.Invoke();
 
                    //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
                    int attackRoll = RandomHelper.RollD20() + 7;

                    if (attackRoll >= enemyData.AC)
                    {
                        //there was an hit, enemy is destroyed, gain xp
                        this.enemies.Remove(enemy);
                        this.disposableObjects[enemy.name].Remove(enemy);
                        enemy.SetActive(false);
                        Object.Destroy(enemy);
                    }
                }
                else
                {
                    damage = enemyData.simpleDamage;
                    this.enemies.Remove(enemy);
                    this.disposableObjects[enemy.name].Remove(enemy);
                    enemy.SetActive(false);
                    Object.Destroy(enemy);
                }

                this.characterData.XP += enemyData.XPvalue;

                int remainingDamage = damage - this.characterData.ShieldHP;
                this.characterData.ShieldHP = Mathf.Max(0, this.characterData.ShieldHP - damage);

                if (remainingDamage > 0)
                {
                    this.characterData.HP -= remainingDamage;
                }

                this.WorldChanged = true;
            }
        }

        public void DivineSmite(GameObject enemy)
        {
            //Debug.Log((enemy.transform.position - this.characterData.CharacterGameObject.transform.position).sqrMagnitude / cellSize);
            if (enemy != null && enemy.activeSelf && InDivineSmiteRange(enemy) && this.characterData.Mana >= 2)
            {
                if (enemy.tag.Equals("Skeleton"))
                {
                    this.characterData.XP += 3;
                    this.autonomousCharacter.DiaryText.text += Time.time + " I Smited " + enemy.name + ".\n";
                    this.enemies.Remove(enemy);
                    this.disposableObjects[enemy.name].Remove(enemy);
                    enemy.SetActive(false);
                    Object.Destroy(enemy);
                }
                this.characterData.Mana -= 2;

                this.WorldChanged = true;
            }
        }

        public void ShieldOfFaith()
        {
            if (this.characterData.Mana >= 5)
            {
                this.characterData.ShieldHP = 5;
                this.characterData.Mana -= 5;
                this.autonomousCharacter.DiaryText.text += Time.time + " My Shield of Faith will protect me!\n";
                this.WorldChanged = true;
            }
        }

        public void PickUpChest(GameObject chest)
        {
            if (chest != null && chest.activeSelf && InChestRange(chest))
            {
                this.autonomousCharacter.DiaryText.text += Time.time + " I opened  " + chest.name + "\n";
                this.chests.Remove(chest);
                this.disposableObjects[chest.name].Remove(chest);
                Object.Destroy(chest);
                this.characterData.Money += 5;
                this.WorldChanged = true;
            }
        }


        public void GetManaPotion(GameObject manaPotion)
        {
            if (manaPotion != null && manaPotion.activeSelf && InPotionRange(manaPotion))
            {
                this.autonomousCharacter.DiaryText.text += Time.time + " I drank  " + manaPotion.name + "\n";
                this.disposableObjects[manaPotion.name].Remove(manaPotion);
                Object.Destroy(manaPotion);
                this.characterData.Mana = 10;
                this.WorldChanged = true;
            }
        }

        public void GetHealthPotion(GameObject potion)
        {
            if (potion != null && potion.activeSelf && InPotionRange(potion))
            {
                this.autonomousCharacter.DiaryText.text += Time.time + " I drank  " + potion.name + "\n";
                this.disposableObjects[potion.name].Remove(potion);
                Object.Destroy(potion);
                this.characterData.HP = this.characterData.MaxHP;
                this.WorldChanged = true;
            }
        }


        private bool CheckRange(GameObject obj, float maximumSqrDistance)
        {
            var distance = (obj.transform.position - this.characterData.CharacterGameObject.transform.position).sqrMagnitude / cellSize;
            return distance <= maximumSqrDistance;
        }


        public bool InMeleeRange(GameObject enemy)
        {
            return this.CheckRange(enemy, 10.0f);
        }

        public bool InDivineSmiteRange(GameObject enemy)
        {
            return this.CheckRange(enemy, 10.0f);
        }

        public bool InChestRange(GameObject chest)
        {
            return this.CheckRange(chest, 10.0f);
        }

        public bool InPotionRange(GameObject potion)
        {
            return this.CheckRange(potion, 10.0f);
        }
    }
}
