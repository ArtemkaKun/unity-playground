using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ArtemkaKun.Scripts.RangeChancesSystem
{
    /// <summary>
    /// Test class for the RangeChancesController.
    /// </summary>
    public class RandomObjectSpawner : RangeChancesController<GameObject>
    {
        private void Start()
        {
            StartCoroutine(SpawnStream());
        }

        private IEnumerator SpawnStream()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                var objectToSpawn = GetRandomUnitObject();

                if (!objectToSpawn)
                {
                    continue;
                }
                
                var newObject = Instantiate(objectToSpawn);

                newObject.transform.position = Random.insideUnitSphere * 30;
            }
        }
    }
}