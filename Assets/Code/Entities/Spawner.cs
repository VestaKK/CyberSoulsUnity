using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Spawner<EntityType> : MonoBehaviour
    where EntityType : Entity {

        [SerializeField] int _entityCount;
        [SerializeField] float _radius;

        [SerializeField] private EntityType _prefab;
        [SerializeField] protected Camera _camera;

        public abstract EntityType SpawnEntity(); 

        public void Awake() {
            StartCoroutine(SpawnEntities());
        }

        private IEnumerator SpawnEntities() {
            for(int i = 0; i < _entityCount; i++) {
                Entity e = SpawnEntity();
                yield return new WaitForSeconds(5);
            }
        }

        public EntityType Prefab { get => _prefab; }
}